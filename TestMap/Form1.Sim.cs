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
        private ModelForm modelForm;
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
                int via = result.VisitedViaEdge[step - 1];
                if (via >= 0)
                {
                    double w = graph.Edges[via].Length;
                    sb.Append("\r\nĐi từ đỉnh ").Append(graph.OtherEnd(via, u)).Append(" tới đỉnh ").Append(u)
                      .Append(" qua cạnh có trọng số ").Append(w.ToString("0.#")).Append(" m:  g = ")
                      .Append((g - w).ToString("0.#")).Append(" + ").Append(w.ToString("0.#")).Append(" = ").Append(g.ToString("0.#")).Append(" m");
                }
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
        //  ĐỒ THỊ: ĐỈNH, TRỌNG SỐ, TÊN ĐỊA ĐIỂM
        // =====================================================================
        public bool ShowWeights
        {
            get { return chkWeights.Checked; }
            set { chkWeights.Checked = value; }
        }

        public bool ShowNames
        {
            get { return chkNames.Checked; }
            set { chkNames.Checked = value; }
        }

        private void chkWeights_CheckedChanged(object sender, EventArgs e)
        {
            mapCanvas.ShowWeights = chkWeights.Checked;
            if (chkWeights.Checked && graph != null && mapCanvas.Zoom < 15 && graph.Nodes.Count <= 200) mapCanvas.Zoom = 16;
            mapCanvas.Invalidate();
        }

        private void chkNames_CheckedChanged(object sender, EventArgs e)
        {
            mapCanvas.ShowAllNames = chkNames.Checked;
            mapCanvas.Invalidate();
        }

        // Tên các trạm gắn vào một giao lộ
        private string StopsAtNode(int node)
        {
            var names = new List<string>();
            foreach (BusStop s in graph.Stops) if (s.Node == node) names.Add(s.ToString());
            return string.Join(", ", names);
        }

        public string LookupGraph(string textA, string textB)
        {
            if (graph == null) return "Bản đồ chưa sẵn sàng.";
            int a, b;
            if (!int.TryParse((textA ?? "").Trim(), out a) || a < 0 || a >= graph.Nodes.Count)
                return "Đỉnh 1 không hợp lệ. Hãy gõ số từ 0 tới " + (graph.Nodes.Count - 1) + " (số hiện trong vòng tròn trên bản đồ khi bật \"Hiện đỉnh và trọng số\").";

            var sb = new StringBuilder();
            string place = StopsAtNode(a);
            mapCanvas.SimOpenNodes = new HashSet<int>();
            mapCanvas.SimCurrentNode = a;
            mapCanvas.SimStops = null;
            mapCanvas.Position = new PointLatLng(graph.Nodes[a].Lat, graph.Nodes[a].Lng);

            // Chỉ nhập Đỉnh 1: liệt kê các đỉnh nối trực tiếp
            if (string.IsNullOrWhiteSpace(textB))
            {
                sb.Append("Đỉnh ").Append(a);
                if (place.Length > 0) sb.Append("  (").Append(place).Append(")");
                sb.Append(" nối trực tiếp với:\r\n");
                int count = 0;
                for (int ei = 0; ei < graph.Edges.Count; ei++)
                {
                    RoadEdge e = graph.Edges[ei];
                    if (e.From != a && e.To != a) continue;
                    int other = e.From == a ? e.To : e.From;
                    string dir = !e.OneWay ? "" : (e.From == a ? "  (một chiều, đi được)" : "  (một chiều, chỉ đi vào đỉnh " + a + ")");
                    sb.Append("  • Đỉnh ").Append(other).Append(":  ").Append(e.Length.ToString("0.#")).Append(" m").Append(dir);
                    string pn = StopsAtNode(other);
                    if (pn.Length > 0) sb.Append("  [").Append(pn).Append("]");
                    sb.Append("\r\n");
                    mapCanvas.SimOpenNodes.Add(other);
                    count++;
                }
                if (count == 0) sb.Append("  (không có đoạn đường nào)");
                mapCanvas.Invalidate();
                return sb.ToString();
            }

            if (!int.TryParse(textB.Trim(), out b) || b < 0 || b >= graph.Nodes.Count)
                return "Đỉnh 2 không hợp lệ. Hãy gõ số từ 0 tới " + (graph.Nodes.Count - 1) + ".";
            if (a == b) return "Đỉnh 1 và Đỉnh 2 là cùng một đỉnh.";

            var direct = new List<RoadEdge>();
            foreach (RoadEdge e in graph.Edges)
                if ((e.From == a && e.To == b) || (e.From == b && e.To == a)) direct.Add(e);

            sb.Append("Đỉnh ").Append(a).Append(" — Đỉnh ").Append(b).Append("\r\n");
            mapCanvas.SimOpenNodes.Add(b);
            if (direct.Count > 0)
            {
                sb.Append("Nối TRỰC TIẾP với nhau.\r\n");
                foreach (RoadEdge e in direct)
                {
                    string dir = !e.OneWay ? "hai chiều" : "một chiều " + e.From + " → " + e.To;
                    sb.Append("  • Trọng số = ").Append(e.Length.ToString("0.#")).Append(" m  (").Append(dir).Append(", ").Append(ClassText(e.Class)).Append(")\r\n");
                }
                mapCanvas.Invalidate();
                return sb.ToString();
            }

            sb.Append("KHÔNG nối trực tiếp.\r\n");
            SearchResult r = PathFinder.Find(graph, a, b, true);
            if (!r.Found)
            {
                sb.Append("Cũng không có đường đi nào từ đỉnh ").Append(a).Append(" tới đỉnh ").Append(b).Append(" (có thể do đường bị chặn).");
                mapCanvas.Invalidate();
                return sb.ToString();
            }
            sb.Append("Đường ngắn nhất đi qua ").Append(r.PathNodes.Count).Append(" đỉnh:\r\n");
            double sum = 0;
            for (int i = 0; i < r.PathNodes.Count; i++)
            {
                int n = r.PathNodes[i];
                sb.Append(i == 0 ? "  " : "  →(").Append(i == 0 ? "" : graph.Edges[r.PathEdges[i - 1]].Length.ToString("0") + " m) ").Append("Đỉnh ").Append(n);
                if (i > 0) sum += graph.Edges[r.PathEdges[i - 1]].Length;
                sb.Append("\r\n");
                mapCanvas.SimOpenNodes.Add(n);
            }
            sb.Append("Tổng trọng số = ").Append(sum.ToString("0.#")).Append(" m.");
            mapCanvas.Invalidate();
            return sb.ToString();
        }

        private static string ClassText(int c)
        {
            switch (c) { case 3: return "trục chính"; case 2: return "đường lớn"; case 1: return "đường vừa"; default: return "đường nhỏ"; }
        }

        // =====================================================================
        //  SƠ ĐỒ MÔ HÌNH CÁCH ĐI XE BUÝT (cho người xem dễ hiểu)
        // =====================================================================
        public void OpenModel()
        {
            if (graph == null) return;
            if (itins == null || itins.Count == 0)
            {
                lblInfo.Text = "Chưa có cách đi để mô hình hóa. Hãy chọn trạm A và trạm B ở tab Gợi ý xe buýt trước.";
                return;
            }
            if (modelForm != null && !modelForm.IsDisposed) modelForm.Close();
            modelForm = new ModelForm(BuildModels(), Math.Max(0, lstItins.SelectedIndex));
            modelForm.Show(this);
        }

        private void btnModel_Click(object sender, EventArgs e) { OpenModel(); }

        private List<ModelData> BuildModels()
        {
            // các tuyến liên quan: mọi tuyến xuất hiện trong các cách đi đang hiển thị
            var lineList = new List<BusLine>();
            foreach (Itinerary it in itins)
                foreach (TransitLeg l in it.Legs)
                    if (!l.Walk && !lineList.Contains(l.Line)) lineList.Add(l.Line);

            var res = new List<ModelData>();
            foreach (Itinerary it in itins) res.Add(BuildModel(it, lineList));
            return res;
        }

        private ModelData BuildModel(Itinerary it, List<BusLine> lineList)
        {
            var m = new ModelData { LaneCount = Math.Max(1, lineList.Count) };
            m.Title = it.WalkOnly
                ? "Đi bộ thẳng  ·  " + Minutes(it.Seconds) + " phút"
                : "Tuyến " + RidesText(it) + "  ·  " + Minutes(it.Seconds) + " phút  ·  " + it.Rides + " chuyến";

            var nodeOf = new Dictionary<int, int>();
            Func<int, int> N = stop =>
            {
                int k;
                if (!nodeOf.TryGetValue(stop, out k))
                {
                    k = m.Nodes.Count;
                    nodeOf[stop] = k;
                    m.Nodes.Add(new ModelNode { Name = StopName(stop), Lat = graph.Stops[stop].Lat, Lng = graph.Stops[stop].Lng });
                }
                return k;
            };

            // Cạnh xe của từng tuyến: trọng số = quãng đường / tốc độ xe + thời gian dừng
            var edgeOf = new Dictionary<long, int>();
            var linesAtNode = new Dictionary<int, HashSet<int>>();
            for (int li = 0; li < lineList.Count; li++)
            {
                BusLine l = lineList[li];
                for (int i = 0; i + 1 < l.Stops.Count; i++)
                {
                    int a = N(l.Stops[i]), b = N(l.Stops[i + 1]);
                    edgeOf[li * 100000L + i] = m.Edges.Count;
                    m.Edges.Add(new ModelEdge
                    {
                        A = a, B = b, Color = l.Color, Ref = l.Ref, Lane = li,
                        Seconds = (l.StopDist[i + 1] - l.StopDist[i]) / TransitNet.BusSpeed + TransitNet.DwellSeconds
                    });
                    foreach (int n in new[] { a, b })
                    {
                        HashSet<int> set;
                        if (!linesAtNode.TryGetValue(n, out set)) linesAtNode[n] = set = new HashSet<int>();
                        set.Add(li);
                    }
                }
            }

            // Các bước của cách đi
            foreach (TransitLeg leg in it.Legs)
            {
                if (leg.Walk)
                {
                    int a = N(leg.FromStop), b = N(leg.ToStop);
                    var ml = new ModelLeg
                    {
                        Kind = "walk", Seconds = leg.Seconds, Color = Color.FromArgb(100, 116, 139),
                        Text = "Đi bộ " + Math.Round(leg.Meters) + " m từ " + StopName(leg.FromStop) + " tới " + StopName(leg.ToStop),
                        Note = "quãng đường ÷ tốc độ đi bộ 1,2 m/s"
                    };
                    ml.Edges.Add(m.Edges.Count);
                    m.Edges.Add(new ModelEdge { A = a, B = b, Walk = true, Seconds = leg.Seconds, Color = Color.FromArgb(100, 116, 139), PathLeg = m.Legs.Count });
                    m.Legs.Add(ml);
                }
                else
                {
                    int li = lineList.IndexOf(leg.Line);
                    int boardNode = N(leg.FromStop);
                    m.Legs.Add(new ModelLeg
                    {
                        Kind = "wait", Node = boardNode, Seconds = leg.Line.HeadwayMin * 30.0, Color = leg.Line.Color,
                        Text = "Chờ xe tuyến " + leg.Line.Ref + " tại " + StopName(leg.FromStop),
                        Note = "trung bình nửa khoảng cách " + leg.Line.HeadwayMin + " phút giữa hai chuyến"
                    });
                    var ride = new ModelLeg
                    {
                        Kind = "ride", Seconds = leg.Seconds, Color = leg.Line.Color,
                        Text = "Đi tuyến " + leg.Line.Ref + " từ " + StopName(leg.FromStop) + " tới " + StopName(leg.ToStop) + " (" + leg.StopCount + " trạm)",
                        Note = "mỗi trạm: quãng đường ÷ 5 m/s + 20 giây dừng"
                    };
                    for (int pos = leg.FromPos; pos < leg.ToPos; pos++)
                    {
                        int ei;
                        if (!edgeOf.TryGetValue(li * 100000L + pos, out ei)) continue;
                        m.Edges[ei].PathLeg = m.Legs.Count;
                        ride.Edges.Add(ei);
                    }
                    m.Legs.Add(ride);
                }
            }

            foreach (KeyValuePair<int, HashSet<int>> kv in linesAtNode) m.Nodes[kv.Key].Transfer = kv.Value.Count >= 2;
            if (mapCanvas.StartStop >= 0) m.Nodes[N(mapCanvas.StartStop)].Start = true;
            if (mapCanvas.EndStop >= 0) m.Nodes[N(mapCanvas.EndStop)].End = true;
            return m;
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
