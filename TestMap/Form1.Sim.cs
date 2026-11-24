using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using TestMap.Forms;
using TestMap.Routing;

namespace TestMap
{
    // Phần Form1 lo: bảng Mô phỏng (thuật toán từng bước, gợi ý xe buýt từng bước, chặn đường), lọc địa điểm, chụp ảnh bản đồ.
    public partial class Form1 : ISimHost
    {
        private SimForm simForm;
        private PoiForm poiForm;
        private HashSet<PoiKind> poiFilter;                // null = hiện mọi loại địa điểm

        private bool simActive;                            // đang phát từng bước thuật toán tìm đường
        private int algoStep;
        private bool simBusLegs;                           // đang hiện cách đi của một lượt gợi ý xe buýt
        private List<PlanRound> busRounds = new List<PlanRound>();
        private List<int> busRoundStart = new List<int>();
        private readonly Random rng = new Random();

        public event EventHandler SimInvalidated;

        private void RaiseSimInvalidated()
        {
            if (SimInvalidated != null) SimInvalidated(this, EventArgs.Empty);
        }

        // Xóa các lớp vẽ của mô phỏng (không đụng tới đường bị chặn)
        private void ClearSimOverlay()
        {
            simActive = false;
            simBusLegs = false;
            mapCanvas.SimOpenNodes = null;
            mapCanvas.SimCurrentNode = -1;
            mapCanvas.SimStops = null;
            mapCanvas.SimCurrentStop = -1;
        }

        // =====================================================================
        //  NÚT TRÊN THANH TRÊN
        // =====================================================================
        private void btnSim_Click(object sender, EventArgs e)
        {
            if (simForm != null && !simForm.IsDisposed) { simForm.Activate(); return; }
            simForm = new SimForm(this);
            simForm.Location = new Point(Left + 24, Top + 150);
            simForm.Show(this);
        }

        private void btnPoiFilter_Click(object sender, EventArgs e)
        {
            if (poiForm != null && !poiForm.IsDisposed) { poiForm.Activate(); return; }
            poiForm = new PoiForm(poiFilter, OnPoiFilterChanged, PoiAction);
            poiForm.SetPois(pois);
            poiForm.Location = new Point(Math.Max(Left, Right - poiForm.Width - 24), Top + 140);
            poiForm.Show(this);
        }

        private void OnPoiFilterChanged(HashSet<PoiKind> f)
        {
            poiFilter = f;
            mapCanvas.PoiFilter = f.Count >= Enum.GetValues(typeof(PoiKind)).Length ? null : f;
            if (f.Count > 0 && !chkPoi.Checked) chkPoi.Checked = true;
            mapCanvas.Invalidate();
        }

        // 0 = xem trên bản đồ, 1 = đặt làm điểm đi A, 2 = đặt làm điểm đến B (chọn trạm gần địa điểm nhất)
        private void PoiAction(Poi p, int what)
        {
            if (graph == null || busy) return;
            mapCanvas.Position = new PointLatLng(p.Lat, p.Lng);
            if (mapCanvas.Zoom < 17) mapCanvas.Zoom = 17;
            if (what == 0)
            {
                lblInfo.Text = Poi.KindName(p.Kind) + ": " + p.Name;
                mapCanvas.Invalidate();
                return;
            }

            int st = graph.NearestStop(p.Lat, p.Lng);
            if (st < 0) { lblInfo.Text = p.Name + ": khu vực này chưa có trạm nào."; return; }
            double d = Meters(p.Lat, p.Lng, graph.Stops[st].Lat, graph.Stops[st].Lng);
            pickingStart = what == 1;
            PickPoint(st, graph.Stops[st].Node);
            lblInfo.Text = (what == 1 ? "Điểm đi" : "Điểm đến") + " gần " + p.Name + ": trạm " + graph.Stops[st] + " (cách " + FormatDistance(d) + ").";
        }

        // =====================================================================
        //  MÔ PHỎNG THUẬT TOÁN TÌM ĐƯỜNG (từng bước)
        // =====================================================================
        public AlgoSimInfo AlgoSimPrepare(out string error)
        {
            error = null;
            if (graph == null || busy) { error = "Bản đồ chưa sẵn sàng."; return null; }
            if (!HasValidEndpoints()) { error = "Hãy chọn điểm đi (A) và điểm đến (B) trên bản đồ trước."; return null; }

            timer.Stop();
            StopBus();
            phase = Phase.Idle;
            bool useAStar = cboAlgo.SelectedIndex == 1;
            algoName = useAStar ? "A*" : "Dijkstra";

            result = PathFinder.Find(graph, mapCanvas.StartNode, mapCanvas.EndNode, useAStar);
            mapCanvas.LastResult = result;
            if (result.Found) { route = RoutePath.Build(graph, result); mapCanvas.Route = route; }
            else { route = null; mapCanvas.Route = null; }
            mapCanvas.RouteProgress = 1;

            ClearSimOverlay();
            simActive = true;
            algoStep = 0;
            FitToEndpoints();
            ApplyView();

            var info = new AlgoSimInfo { Algo = algoName, Total = result.Visited.Count, Found = result.Found };
            var sb = new StringBuilder();
            sb.Append("Thuật toán: ").Append(algoName).Append("\r\n");
            sb.Append(useAStar
                ? "A* chọn giao lộ có f = g + h nhỏ nhất (g: đã đi, h: chim bay còn lại tới đích) nên thường xét ít giao lộ hơn Dijkstra."
                : "Dijkstra chọn giao lộ có g nhỏ nhất (g: quãng đường đã đi) nên lan đều ra mọi hướng.");
            sb.Append("\r\nĐường bị chặn: ").Append(graph.BlockedCount).Append(" đoạn (thuật toán sẽ tránh).");
            sb.Append("\r\nTổng ").Append(info.Total).Append(" bước. Bấm \"▶ Chạy\" hoặc \"Tới ▶\".");
            info.Intro = sb.ToString();
            return info;
        }

        public string AlgoSimSeek(int step)
        {
            if (result == null || graph == null) return "";
            int total = result.Visited.Count;
            step = Math.Max(0, Math.Min(total, step));
            algoStep = step;
            simActive = true;

            // Tái dựng tập "đã chốt" và "đang chờ trong hàng đợi" sau `step` bước
            var closed = new HashSet<int>();
            var open = new HashSet<int>();
            if (step == 0) open.Add(mapCanvas.StartNode);
            for (int i = 0; i < step; i++)
            {
                int u = result.Visited[i];
                closed.Add(u);
                open.Remove(u);
                foreach (int ei in graph.Adj[u])
                {
                    if (graph.IsBlocked(ei)) continue;
                    int v = graph.OtherEnd(ei, u);
                    if (!closed.Contains(v)) open.Add(v);
                }
            }

            bool done = step >= total;
            mapCanvas.VisitedShown = step;
            mapCanvas.SimOpenNodes = open;
            mapCanvas.SimCurrentNode = step > 0 ? result.Visited[step - 1] : mapCanvas.StartNode;
            mapCanvas.ShowSearch = true;
            mapCanvas.ShowPath = done && result.Found && route != null;
            mapCanvas.Invalidate();

            var sb = new StringBuilder();
            if (step == 0)
            {
                sb.Append("Chưa bắt đầu.\r\nĐiểm đi là giao lộ #").Append(mapCanvas.StartNode).Append(" (g = 0). Hàng đợi chỉ có điểm đi.");
            }
            else
            {
                int u = result.Visited[step - 1];
                double g = result.VisitedCost[step - 1];
                double h = RoadGraph.Distance(graph.Nodes[u], graph.Nodes[mapCanvas.EndNode]);
                sb.Append("Bước ").Append(step).Append(": ")
                  .Append(result.AStar ? "A* chọn giao lộ có f = g + h nhỏ nhất." : "Dijkstra chọn giao lộ có g nhỏ nhất.").Append("\r\n");
                sb.Append("Chốt giao lộ #").Append(u).Append(":  g = ").Append(FormatDistance(g));
                if (result.AStar) sb.Append(",  h = ").Append(FormatDistance(h)).Append(",  f = ").Append(FormatDistance(g + h));
                sb.Append("\r\nĐang chờ trong hàng đợi: ").Append(open.Count).Append(" giao lộ.");
                double pct = 100.0 * step / Math.Max(1, graph.Nodes.Count);
                sb.Append("\r\nĐã chốt ").Append(step).Append(" / ").Append(graph.Nodes.Count).Append(" giao lộ của bản đồ (").Append(pct.ToString("0.#")).Append("%).");
            }
            if (done)
            {
                sb.Append("\r\n\r\n");
                if (result.Found) sb.Append("→ Tới đích! Đường ngắn nhất dài ").Append(FormatDistance(result.Distance)).Append(", đi qua ").Append(result.PathNodes.Count).Append(" giao lộ.");
                else sb.Append("→ Hàng đợi cạn mà chưa tới đích: không có đường đi (đường có thể đã bị chặn).");
            }
            return sb.ToString();
        }

        // =====================================================================
        //  MÔ PHỎNG BỘ GỢI Ý XE BUÝT (từng bước)
        // =====================================================================
        public BusSimInfo BusSimPrepare(out string error)
        {
            error = null;
            if (graph == null || busy) { error = "Bản đồ chưa sẵn sàng."; return null; }
            if (net == null) { error = "Khu vực này chưa có tuyến xe buýt để gợi ý."; return null; }
            if (mapCanvas.StartStop < 0 || mapCanvas.EndStop < 0 || mapCanvas.StartStop == mapCanvas.EndStop)
            {
                error = "Hãy chọn trạm đi (A) và trạm đến (B) khác nhau trên bản đồ trước.";
                return null;
            }

            busRounds = new List<PlanRound>();
            BusPlanner.Plan(net, mapCanvas.StartStop, mapCanvas.EndStop, cboRides.SelectedIndex + 1, busRounds);

            var info = new BusSimInfo();
            busRoundStart = new List<int>();
            int cum = 0;
            for (int i = 0; i < busRounds.Count; i++)
            {
                busRoundStart.Add(cum);
                info.RoundStarts.Add(cum);
                cum += busRounds[i].Steps.Count;
                info.RoundEnds.Add(cum);
                info.RoundTitles.Add(RoundTitle(i, busRounds[i]));
            }
            info.Total = cum;
            if (cum == 0) { error = "Bộ gợi ý không xét được trạng thái nào."; return null; }

            ClearSimOverlay();
            mapCanvas.SimStops = new HashSet<int>();
            mapCanvas.SimCurrentStop = mapCanvas.StartStop;
            mapCanvas.ItinLegs = new List<MapLeg>();
            FitToEndpoints();
            ApplyView();

            info.Intro = "Bộ gợi ý chạy Dijkstra trên đồ thị gồm các trạng thái: chờ xe / ngồi trên xe / vừa xuống xe. " +
                         "Mỗi bước nó chốt trạng thái có thời gian nhỏ nhất trong hàng đợi. Chạy tới trạm kế = thời gian chạy + 20 giây dừng; " +
                         "lên xe = chờ nửa giãn cách + phạt đổi xe; đi bộ tối đa 250 m.\r\n" +
                         "Có " + busRounds.Count + " lượt, " + cum + " bước. Bấm \"▶ Chạy\".";
            return info;
        }

        private string RoundTitle(int i, PlanRound r)
        {
            var sb = new StringBuilder();
            sb.Append("Lượt ").Append(i + 1).Append(": ").Append(r.Penalty < 300 ? "nhanh nhất" : "ít đổi xe");
            if (r.Banned.Count > 0)
            {
                var refs = new List<string>();
                foreach (int b in r.Banned) refs.Add(net.Lines[b].Ref);
                sb.Append(", cấm tuyến ").Append(string.Join("/", refs));
            }
            sb.Append(" → ").Append(OutcomeShort(r));
            return sb.ToString();
        }

        private static string RidesText(Itinerary it)
        {
            var refs = new List<string>();
            foreach (TransitLeg l in it.Legs) if (!l.Walk) refs.Add(l.Line.Ref);
            return string.Join(" → ", refs);
        }

        private static string OutcomeShort(PlanRound r)
        {
            switch (r.Outcome)
            {
                case "ok": return "tuyến " + RidesText(r.Result) + ", " + Minutes(r.Result.Seconds) + " phút";
                case "dup": return "trùng cách đã có";
                case "tooMany": return "quá nhiều chuyến, bỏ";
                case "tooWalk": return "đi bộ quá xa, bỏ";
                default: return "hết đường";
            }
        }

        private static string Clock(double seconds)
        {
            int s = (int)Math.Round(seconds);
            return s / 60 + " phút " + s % 60 + " giây";
        }

        public string BusSimSeek(int step)
        {
            if (busRounds.Count == 0 || graph == null || net == null) return "";

            int r = -1;
            for (int i = 0; i < busRoundStart.Count; i++) if (busRoundStart[i] < step) r = i;
            simBusLegs = false;
            mapCanvas.ItinLegs = new List<MapLeg>();

            if (r < 0)
            {
                mapCanvas.SimStops = new HashSet<int>();
                mapCanvas.SimCurrentStop = mapCanvas.StartStop;
                ApplyView();
                mapCanvas.Invalidate();
                return "Chưa bắt đầu. Điểm xuất phát là trạm " + StopName(mapCanvas.StartStop) + ".";
            }

            PlanRound round = busRounds[r];
            int local = step - busRoundStart[r];                  // số bước đã chạy trong lượt này
            var seen = new HashSet<int>();
            for (int i = 0; i < local; i++) seen.Add(round.Steps[i].Stop);
            PlanStep cur = round.Steps[local - 1];
            PlanStep prev = local >= 2 ? round.Steps[local - 2] : null;
            bool roundDone = local >= round.Steps.Count;

            mapCanvas.SimStops = seen;
            mapCanvas.SimCurrentStop = cur.Stop;
            if (roundDone && round.Result != null) { mapCanvas.ItinLegs = round.Result.ToMapLegs(graph); simBusLegs = true; }
            ApplyView();
            mapCanvas.Invalidate();

            var sb = new StringBuilder();
            sb.Append(RoundTitle(r, round)).Append("\r\n\r\n");
            string stop = StopName(cur.Stop);
            switch (cur.Kind)
            {
                case PlanKind.Start:
                    sb.Append("Xuất phát tại trạm ").Append(stop).Append("."); break;
                case PlanKind.Wait:
                    sb.Append("Đi bộ tới trạm ").Append(stop).Append(" để chờ xe. Đã tốn ").Append(Clock(cur.Cost)).Append("."); break;
                case PlanKind.Ride:
                    BusLine line = net.Lines[cur.LineIndex];
                    bool boarded = prev == null || prev.Kind != PlanKind.Ride || prev.LineIndex != cur.LineIndex;
                    if (boarded)
                        sb.Append("Lên tuyến ").Append(line.Ref).Append(" (chờ khoảng ").Append(Math.Max(1, line.HeadwayMin / 2))
                          .Append(" phút) rồi chạy tới trạm ").Append(stop).Append(". Tổng ").Append(Clock(cur.Cost)).Append(".");
                    else
                        sb.Append("Tuyến ").Append(line.Ref).Append(" chạy tiếp tới trạm ").Append(stop).Append(". Tổng ").Append(Clock(cur.Cost)).Append(".");
                    break;
                case PlanKind.Alight:
                    sb.Append("Xuống xe ở trạm ").Append(stop).Append(". Từ đây có thể đổi tuyến khác, đi bộ tối đa 250 m, hoặc kết thúc nếu đây là đích. Tổng ")
                      .Append(Clock(cur.Cost)).Append("."); break;
                default:
                    sb.Append("Tới đích: ").Append(stop).Append("! Tổng ").Append(Clock(cur.Cost)).Append("."); break;
            }
            if (roundDone)
            {
                sb.Append("\r\n\r\n→ ");
                switch (round.Outcome)
                {
                    case "ok": sb.Append("Tìm được cách đi: tuyến ").Append(RidesText(round.Result)).Append(", khoảng ").Append(Minutes(round.Result.Seconds)).Append(" phút, ").Append(round.Result.Rides).Append(" chuyến. Thêm vào danh sách gợi ý."); break;
                    case "dup": sb.Append("Trùng một cách đã có (cùng dãy tuyến), bỏ qua."); break;
                    case "tooMany": sb.Append("Cần ").Append(round.Result.Rides).Append(" chuyến, vượt giới hạn đã chọn, bỏ."); break;
                    case "tooWalk": sb.Append("Phải đi bộ ").Append(Math.Round(round.Result.WalkMeters)).Append(" m, quá xa, bỏ."); break;
                    default: sb.Append("Không còn đường nào (sau khi cấm các tuyến trên). Hết lượt."); break;
                }
            }
            return sb.ToString();
        }

        // =====================================================================
        //  CHẶN ĐƯỜNG
        // =====================================================================
        public bool BlockMode
        {
            get { return mapCanvas.BlockMode; }
            set
            {
                mapCanvas.BlockMode = value;
                if (!value) mapCanvas.Cursor = Cursors.Default;
                lblInfo.Text = value ? "Chế độ chặn đường: bấm vào một đoạn đường để chặn hoặc mở lại." : "Đã tắt chế độ chặn đường.";
                mapCanvas.Invalidate();
            }
        }

        public int BlockedCount { get { return graph == null ? 0 : graph.BlockedCount; } }

        private void MapCanvas_EdgeClicked(int edge)
        {
            if (graph == null || busy || graph.Blocked == null || edge < 0 || edge >= graph.Blocked.Length) return;
            graph.Blocked[edge] = !graph.Blocked[edge];
            OnBlockedChanged();
        }

        // Chặn một đoạn nằm trên đường đi đang tìm được (để thấy ngay hiệu quả)
        public void BlockOnRoute()
        {
            if (graph == null || busy) return;
            if (result == null || !result.Found || result.PathEdges.Count == 0)
            {
                lblInfo.Text = "Hãy tìm đường (chọn A và B) trước, rồi mới chặn một đoạn trên đường đi.";
                return;
            }
            int n = result.PathEdges.Count;
            int idx = n <= 2 ? rng.Next(n) : 1 + rng.Next(n - 2);       // tránh sát điểm đầu / cuối
            graph.Blocked[result.PathEdges[idx]] = true;
            OnBlockedChanged();
        }

        public void BlockRandom(int count)
        {
            if (graph == null || busy || graph.Blocked == null) return;
            var free = new List<int>();
            for (int i = 0; i < graph.Blocked.Length; i++) if (!graph.Blocked[i]) free.Add(i);
            for (int k = 0; k < count && free.Count > 0; k++)
            {
                int j = rng.Next(free.Count);
                graph.Blocked[free[j]] = true;
                free.RemoveAt(j);
            }
            OnBlockedChanged();
        }

        public void ClearBlocked()
        {
            if (graph == null) return;
            graph.ClearBlocked();
            OnBlockedChanged();
        }

        // Đường thay đổi: dựng lại đường chạy của xe buýt và mạng gợi ý, rồi tìm lại
        private void OnBlockedChanged()
        {
            ClearSimOverlay();
            RaiseSimInvalidated();
            if (graph == null) return;

            foreach (BusLine l in lines) l.BuildPath(graph);
            net = lines.Count > 0 ? TransitNet.Build(graph, lines) : null;

            if (HasValidEndpoints()) RunSearch();
            else { ApplyView(); mapCanvas.Invalidate(); }

            int n = graph.BlockedCount;
            lblInfo.Text = n == 0 ? "Không còn đường nào bị chặn."
                                  : "Đang chặn " + n + " đoạn đường. Thuật toán và xe buýt tự đi vòng nếu còn đường khác.";
        }

        // =====================================================================
        //  CHỤP ẢNH BẢN ĐỒ / ĐÓNG BẢNG MÔ PHỎNG
        // =====================================================================
        public void SaveMapImage()
        {
            using (var dlg = new SaveFileDialog { Filter = "Ảnh PNG|*.png", FileName = "ban-do.png", Title = "Lưu ảnh bản đồ" })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                string path = dlg.FileName;

                bool simVisible = simForm != null && !simForm.IsDisposed && simForm.Visible;
                bool poiVisible = poiForm != null && !poiForm.IsDisposed && poiForm.Visible;
                if (simVisible) simForm.Hide();
                if (poiVisible) poiForm.Hide();
                Application.DoEvents();
                Thread.Sleep(250);
                try
                {
                    Point p = mapCanvas.PointToScreen(Point.Empty);
                    using (var bmp = new Bitmap(mapCanvas.Width, mapCanvas.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bmp)) g.CopyFromScreen(p, Point.Empty, mapCanvas.Size);
                        bmp.Save(path, ImageFormat.Png);
                    }
                    lblInfo.Text = "Đã lưu ảnh bản đồ: " + path;
                }
                catch (Exception ex) { lblInfo.Text = "Không lưu được ảnh: " + ex.Message; }
                finally
                {
                    if (simVisible) simForm.Show(this);
                    if (poiVisible) poiForm.Show(this);
                }
            }
        }

        // Bảng mô phỏng đóng: bỏ lớp vẽ mô phỏng, tắt chế độ chặn (đường đã chặn vẫn giữ nguyên)
        public void SimClose()
        {
            if (IsDisposed || Disposing || !IsHandleCreated) return;
            ClearSimOverlay();
            mapCanvas.BlockMode = false;
            mapCanvas.Cursor = Cursors.Default;
            if (graph != null && HasValidEndpoints()) RunSearch();
            else { ApplyView(); mapCanvas.Invalidate(); }
        }
    }
}
