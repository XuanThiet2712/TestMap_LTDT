using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using TestMap.Mapping;
using TestMap.Routing;

namespace TestMap
{
    // Giao diện dựng bằng code (không dùng Designer) cho gọn và ít lỗi.
    public class Form1 : Form
    {
        // ----- Các khu vực có thể chọn (hộp tọa độ gần đúng: nam, tây, bắc, đông). Muốn thêm khu vực thì thêm 1 dòng. -----
        private class Area
        {
            public string Key, Name;
            public double South, West, North, East;
            public Area(string key, string name, double s, double w, double n, double e)
            { Key = key; Name = name; South = s; West = w; North = n; East = e; }
            public override string ToString() { return Name; }
        }

        private static readonly Area[] Areas =
        {
            new Area("hcm_q1",  "TP.HCM - Quận 1",  10.7640, 106.6850, 10.7900, 106.7110),
            new Area("hcm_q3",  "TP.HCM - Quận 3",  10.7700, 106.6660, 10.7900, 106.6910),
            new Area("hcm_q5",  "TP.HCM - Quận 5",  10.7490, 106.6500, 10.7640, 106.6720),
            new Area("hcm_q10", "TP.HCM - Quận 10", 10.7620, 106.6560, 10.7800, 106.6790),
            new Area("hn_hk",   "Hà Nội - Hoàn Kiếm", 21.0170, 105.8380, 21.0380, 105.8600),
            new Area("hn_bd",   "Hà Nội - Ba Đình",   21.0280, 105.8090, 21.0480, 105.8400),
            new Area("dn_hc",   "Đà Nẵng - Hải Châu", 16.0400, 108.1900, 16.0800, 108.2300),
        };

        private const double AnimationSeconds = 2.5;   // hoạt ảnh loang kéo dài khoảng bao lâu
        private const double BusSpeedMps = 5.0;        // tốc độ xe buýt giả định: 18 km/h
        private const double SecondsPerStop = 20;      // thời gian dừng mỗi trạm giả định

        private static readonly Color Navy = Color.FromArgb(15, 23, 42);
        private static readonly Color Accent = Color.FromArgb(234, 88, 12);

        // ----- Điều khiển -----
        private readonly MapCanvas mapCanvas = new MapCanvas();
        private readonly ComboBox cboArea = new ComboBox();
        private readonly ComboBox cboAlgo = new ComboBox();
        private readonly Button btnReload = new Button();
        private readonly Button btnRun = new Button();
        private readonly Button btnCompare = new Button();
        private readonly Button btnSwap = new Button();
        private readonly Button btnReset = new Button();
        private readonly CheckBox chkStops = new CheckBox();
        private readonly Label lblInfo = new Label();
        private readonly Label lblFrom = new Label();
        private readonly Label lblTo = new Label();
        private Label lblDist, lblTime, lblStopCount, lblVisited;
        private readonly ListBox lstStops = new ListBox();

        // ----- Trạng thái -----
        private readonly Timer timer = new Timer();
        private RoadGraph graph;
        private SearchResult result;
        private bool pickingStart = true;
        private bool busy;
        private int step;
        private int stepsPerTick = 1;
        private string algoName = "";
        private readonly List<int> listStopIndex = new List<int>();   // lstStops[i] ứng với trạm số mấy

        public Form1()
        {
            Text = "TestMap - Gợi ý lộ trình xe buýt";
            ClientSize = new Size(1280, 760);
            MinimumSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);

            BuildUi();

            timer.Interval = 30;
            timer.Tick += Timer_Tick;
            Shown += Form1_Shown;
        }

        // =====================================================================
        //  DỰNG GIAO DIỆN
        // =====================================================================
        private void BuildUi()
        {
            // ----- Bản đồ -----
            mapCanvas.Dock = DockStyle.Fill;
            mapCanvas.EmptyTileColor = MapCanvas.BackgroundColor;
            mapCanvas.ShowCenter = false;
            mapCanvas.DragButton = MouseButtons.Left;
            mapCanvas.MinZoom = 11;
            mapCanvas.MaxZoom = 19;
            mapCanvas.Zoom = 15;
            mapCanvas.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            mapCanvas.MouseClick += MapCanvas_MouseClick;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            mapCanvas.MapProvider = new BlankMapProvider(MapCanvas.BackgroundColor);

            // ----- Thanh trên -----
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Navy };
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                Padding = new Padding(12, 11, 12, 0),
                BackColor = Navy
            };
            pnlTop.Controls.Add(flow);

            cboArea.DropDownStyle = ComboBoxStyle.DropDownList;
            cboArea.Width = 175;
            cboArea.Margin = new Padding(0, 2, 8, 0);
            foreach (Area a in Areas) cboArea.Items.Add(a);
            cboArea.SelectedIndex = 0;
            cboArea.SelectionChangeCommitted += async (s, e) => await LoadArea(cboArea.SelectedIndex, false);

            StyleButton(btnReload, "Tải lại dữ liệu", 118, Color.FromArgb(51, 65, 85));
            btnReload.Click += async (s, e) => await LoadArea(cboArea.SelectedIndex, true);

            cboAlgo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAlgo.Width = 100;
            cboAlgo.Margin = new Padding(20, 2, 8, 0);
            cboAlgo.Items.Add("Dijkstra");
            cboAlgo.Items.Add("A*");
            cboAlgo.SelectedIndex = 1;

            StyleButton(btnRun, "Tìm đường", 100, Accent);
            btnRun.Click += btnRun_Click;
            StyleButton(btnCompare, "So sánh", 90, Color.FromArgb(51, 65, 85));
            btnCompare.Click += btnCompare_Click;
            StyleButton(btnSwap, "⇄ Đổi chiều", 100, Color.FromArgb(51, 65, 85));
            btnSwap.Click += btnSwap_Click;
            StyleButton(btnReset, "Xóa", 64, Color.FromArgb(51, 65, 85));
            btnReset.Click += btnReset_Click;

            chkStops.Text = "Hiện trạm";
            chkStops.Checked = true;
            chkStops.AutoSize = true;
            chkStops.ForeColor = Color.Gainsboro;
            chkStops.Margin = new Padding(10, 6, 0, 0);
            chkStops.CheckedChanged += (s, e) => { mapCanvas.ShowStops = chkStops.Checked; mapCanvas.Invalidate(); };

            flow.Controls.Add(cboArea);
            flow.Controls.Add(btnReload);
            flow.Controls.Add(cboAlgo);
            flow.Controls.Add(btnRun);
            flow.Controls.Add(btnCompare);
            flow.Controls.Add(btnSwap);
            flow.Controls.Add(btnReset);
            flow.Controls.Add(chkStops);

            // ----- Thanh trạng thái dưới -----
            lblInfo.Dock = DockStyle.Bottom;
            lblInfo.Height = 30;
            lblInfo.BackColor = Navy;
            lblInfo.ForeColor = Color.FromArgb(203, 213, 225);
            lblInfo.Padding = new Padding(12, 0, 0, 0);
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;
            lblInfo.Text = "Đang khởi động...";

            // ----- Bảng bên phải -----
            var pnlSide = new Panel { Dock = DockStyle.Right, Width = 310, BackColor = Color.FromArgb(248, 250, 252) };

            lstStops.Dock = DockStyle.Fill;
            lstStops.BorderStyle = BorderStyle.None;
            lstStops.IntegralHeight = false;
            lstStops.ItemHeight = 22;
            lstStops.Font = new Font("Segoe UI", 9.5f);
            lstStops.SelectedIndexChanged += lstStops_SelectedIndexChanged;

            var lblListTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "  CÁC TRẠM ĐI QUA",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlStats = new Panel { Dock = DockStyle.Top, Height = 205, BackColor = Color.FromArgb(248, 250, 252) };
            pnlStats.Controls.Add(MakeLabel("LỘ TRÌNH", 16, 14, 270, 20, new Font("Segoe UI", 8.5f, FontStyle.Bold), Color.FromArgb(100, 116, 139)));

            lblFrom.SetBounds(16, 40, 280, 24);
            lblFrom.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblFrom.ForeColor = Color.FromArgb(21, 128, 61);
            lblFrom.AutoEllipsis = true;
            pnlStats.Controls.Add(lblFrom);

            lblTo.SetBounds(16, 66, 280, 24);
            lblTo.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblTo.ForeColor = Color.FromArgb(185, 28, 28);
            lblTo.AutoEllipsis = true;
            pnlStats.Controls.Add(lblTo);

            lblDist = StatBox(pnlStats, 16, "Quãng đường");
            lblTime = StatBox(pnlStats, 112, "Thời gian");
            lblStopCount = StatBox(pnlStats, 208, "Số trạm");

            lblVisited = MakeLabel("Giao lộ đã xét: —", 16, 168, 280, 22, new Font("Segoe UI", 9f), Color.FromArgb(100, 116, 139));
            pnlStats.Controls.Add(lblVisited);

            pnlSide.Controls.Add(lstStops);      // Fill thêm trước, Top thêm sau
            pnlSide.Controls.Add(lblListTitle);
            pnlSide.Controls.Add(pnlStats);

            // Thứ tự thêm quyết định thứ tự dock: cái thêm sau được dock trước
            Controls.Add(mapCanvas);
            Controls.Add(pnlSide);
            Controls.Add(lblInfo);
            Controls.Add(pnlTop);
        }

        private static void StyleButton(Button b, string text, int width, Color back)
        {
            b.Text = text;
            b.Width = width;
            b.Height = 32;
            b.Margin = new Padding(0, 0, 8, 0);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = back;
            b.ForeColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
        }

        private static Label MakeLabel(string text, int x, int y, int w, int h, Font font, Color color)
        {
            var l = new Label { Text = text, Font = font, ForeColor = color, AutoSize = false };
            l.SetBounds(x, y, w, h);
            return l;
        }

        // Ô thống kê: số to ở trên, chú thích nhỏ ở dưới. Trả về nhãn số.
        private static Label StatBox(Panel parent, int x, string caption)
        {
            var value = MakeLabel("—", x, 104, 94, 32, new Font("Segoe UI", 15f, FontStyle.Bold), Color.FromArgb(15, 23, 42));
            var cap = MakeLabel(caption, x, 136, 94, 20, new Font("Segoe UI", 8.5f), Color.FromArgb(100, 116, 139));
            parent.Controls.Add(value);
            parent.Controls.Add(cap);
            return value;
        }

        // =====================================================================
        //  TẢI DỮ LIỆU KHU VỰC
        // =====================================================================
        private async void Form1_Shown(object sender, EventArgs e)
        {
            await LoadArea(cboArea.SelectedIndex, false);
        }

        private string DataRoot
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"); }
        }

        private void Report(string msg)
        {
            if (IsHandleCreated) BeginInvoke(new Action(() => lblInfo.Text = msg));
        }

        private void SetBusy(bool value)
        {
            busy = value;
            cboArea.Enabled = !value;
            btnReload.Enabled = !value;
            btnRun.Enabled = !value;
            btnCompare.Enabled = !value;
            btnSwap.Enabled = !value;
            btnReset.Enabled = !value;
            UseWaitCursor = value;
        }

        // force = true: tải lại từ OpenStreetMap. false: dùng dữ liệu đã lưu, chưa có thì mới tải.
        private async Task LoadArea(int index, bool force)
        {
            if (busy) return;
            Area a = Areas[index];
            string dir = Path.Combine(DataRoot, a.Key);
            string nodesPath = Path.Combine(dir, "nodes.csv");
            string edgesPath = Path.Combine(dir, "edges.csv");
            string stopsPath = Path.Combine(dir, "stops.csv");
            bool cached = File.Exists(nodesPath) && File.Exists(edgesPath) && File.Exists(stopsPath);

            timer.Stop();
            SetBusy(true);
            try
            {
                if (force || !cached)
                {
                    try
                    {
                        lblInfo.Text = "Đang tải dữ liệu " + a.Name + "...";
                        await Task.Run(() => OsmImporter.ImportArea(a.South, a.West, a.North, a.East, dir, Report));
                    }
                    catch (Exception ex)
                    {
                        if (!cached)
                        {
                            lblInfo.Text = "Không tải được dữ liệu " + a.Name + ": " + ex.Message +
                                           ". Lần đầu cần có Internet; bấm \"Tải lại dữ liệu\" để thử lại.";
                            return;
                        }
                        MessageBox.Show("Không tải mới được (" + ex.Message + "). Dùng dữ liệu đã lưu trước đó.",
                                        Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                RoadGraph g;
                try
                {
                    g = RoadGraph.Load(nodesPath, edgesPath);
                    g.LoadStops(stopsPath);
                }
                catch (Exception ex)
                {
                    lblInfo.Text = "Không đọc được dữ liệu: " + ex.Message;
                    return;
                }
                ApplyGraph(g, a);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ApplyGraph(RoadGraph g, Area a)
        {
            graph = g;
            mapCanvas.RoadNet = g;
            ResetAnimation();

            if (g.Nodes.Count == 0)
            {
                lblInfo.Text = "Khu vực này không có dữ liệu đường.";
                return;
            }

            double minLat, minLng, maxLat, maxLng;
            g.GetBounds(out minLat, out minLng, out maxLat, out maxLng);

            // Điểm đi / đến mặc định: hai trạm ở hai phía của khu vực
            double lat1 = minLat + (maxLat - minLat) * 0.15, lng1 = minLng + (maxLng - minLng) * 0.15;
            double lat2 = minLat + (maxLat - minLat) * 0.85, lng2 = minLng + (maxLng - minLng) * 0.85;
            if (g.Stops.Count >= 2)
            {
                int s1 = g.NearestStop(lat1, lng1), s2 = g.NearestStop(lat2, lng2);
                SetPoint(true, s1, g.Stops[s1].Node);
                SetPoint(false, s2, g.Stops[s2].Node);
            }
            else
            {
                SetPoint(true, -1, g.Nearest(lat1, lng1));
                SetPoint(false, -1, g.Nearest(lat2, lng2));
            }
            pickingStart = true;

            mapCanvas.SetZoomToFitRect(new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat));
            mapCanvas.Invalidate();

            lblInfo.Text = a.Name + ": " + g.Nodes.Count + " giao lộ, " + g.Stops.Count + " trạm xe buýt. " +
                           "Chuột phải lên bản đồ để chọn điểm đi (A), rồi điểm đến (B); bấm Tìm đường." +
                           (g.Stops.Count == 0 ? " (Khu vực này chưa có trạm trong OpenStreetMap, chọn theo giao lộ.)" : "");
        }

        // =====================================================================
        //  CHỌN ĐIỂM ĐI / ĐẾN
        // =====================================================================
        private void SetPoint(bool isStart, int stop, int node)
        {
            if (isStart) { mapCanvas.StartStop = stop; mapCanvas.StartNode = node; }
            else { mapCanvas.EndStop = stop; mapCanvas.EndNode = node; }
            UpdateEndpointLabels();
        }

        private string PointName(int stop, int node)
        {
            if (graph != null && stop >= 0 && stop < graph.Stops.Count) return graph.Stops[stop].Name;
            return node >= 0 ? "Giao lộ #" + node : "—";
        }

        private void UpdateEndpointLabels()
        {
            lblFrom.Text = "A   " + PointName(mapCanvas.StartStop, mapCanvas.StartNode);
            lblTo.Text = "B   " + PointName(mapCanvas.EndStop, mapCanvas.EndNode);
        }

        // Chuột phải: chọn điểm đi, rồi điểm đến (lần lượt). Ưu tiên bắt vào trạm xe buýt gần nhất.
        private void MapCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || graph == null || busy) return;

            PointLatLng p = mapCanvas.FromLocalToLatLng(e.X, e.Y);
            int stop = graph.Stops.Count > 0 ? graph.NearestStop(p.Lat, p.Lng) : -1;
            int node = stop >= 0 ? graph.Stops[stop].Node : graph.Nearest(p.Lat, p.Lng);

            ResetAnimation();
            SetPoint(pickingStart, stop, node);
            lblInfo.Text = pickingStart
                ? "Đã chọn điểm đi (A). Chuột phải để chọn điểm đến (B)."
                : "Đã chọn điểm đến (B). Bấm Tìm đường.";
            pickingStart = !pickingStart;
            mapCanvas.Invalidate();
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            if (graph == null) return;
            int s1 = mapCanvas.StartStop, n1 = mapCanvas.StartNode;
            ResetAnimation();
            SetPoint(true, mapCanvas.EndStop, mapCanvas.EndNode);
            SetPoint(false, s1, n1);
            mapCanvas.Invalidate();
        }

        // =====================================================================
        //  TÌM ĐƯỜNG
        // =====================================================================
        private bool HasValidEndpoints()
        {
            if (graph == null) return false;
            if (mapCanvas.StartNode < 0 || mapCanvas.EndNode < 0 || mapCanvas.StartNode == mapCanvas.EndNode)
            {
                lblInfo.Text = "Hãy chọn điểm đi và điểm đến khác nhau (chuột phải lên bản đồ).";
                return false;
            }
            return true;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!HasValidEndpoints()) return;

            ResetAnimation();
            FitToEndpoints();

            bool useAStar = cboAlgo.SelectedIndex == 1;
            algoName = useAStar ? "A*" : "Dijkstra";

            // Thuật toán chạy xong ngay; hoạt ảnh chỉ phát lại thứ tự các giao lộ đã xét
            result = PathFinder.Find(graph, mapCanvas.StartNode, mapCanvas.EndNode, useAStar);
            mapCanvas.LastResult = result;
            mapCanvas.VisitedShown = 0;
            mapCanvas.ShowPath = false;

            double ticks = AnimationSeconds * 1000.0 / timer.Interval;
            stepsPerTick = Math.Max(1, (int)Math.Ceiling(result.Visited.Count / ticks));
            step = 0;

            lblInfo.Text = "Đang chạy " + algoName + "...";
            timer.Start();
        }

        // So sánh Dijkstra và A* trên cùng một cặp điểm
        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (!HasValidEndpoints()) return;
            SearchResult d = PathFinder.Find(graph, mapCanvas.StartNode, mapCanvas.EndNode, false);
            SearchResult a = PathFinder.Find(graph, mapCanvas.StartNode, mapCanvas.EndNode, true);
            if (!d.Found)
            {
                lblInfo.Text = "Không có đường đi giữa hai điểm này.";
                return;
            }
            lblInfo.Text = "So sánh: Dijkstra xét " + d.Visited.Count + " giao lộ (" + d.ElapsedMs.ToString("F1") + " ms)   |   " +
                           "A* xét " + a.Visited.Count + " giao lộ (" + a.ElapsedMs.ToString("F1") + " ms)   |   " +
                           "cùng quãng đường " + FormatDistance(d.Distance) + " = " + FormatDistance(a.Distance);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetAnimation();
            lblInfo.Text = "Chuột phải lên bản đồ để chọn điểm đi (A), rồi điểm đến (B); bấm Tìm đường.";
        }

        private void ResetAnimation()
        {
            timer.Stop();
            result = null;
            step = 0;
            mapCanvas.LastResult = null;
            mapCanvas.VisitedShown = 0;
            mapCanvas.ShowPath = false;
            mapCanvas.RouteStops.Clear();
            lblDist.Text = lblTime.Text = lblStopCount.Text = "—";
            lblVisited.Text = "Giao lộ đã xét: —";
            lstStops.Items.Clear();
            listStopIndex.Clear();
            mapCanvas.Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            step = Math.Min(step + stepsPerTick, result.Visited.Count);
            mapCanvas.VisitedShown = step;
            lblVisited.Text = "Giao lộ đã xét: " + step;

            if (step >= result.Visited.Count)
            {
                timer.Stop();
                FinishSearch();
            }
            mapCanvas.Invalidate();
        }

        private void FinishSearch()
        {
            if (!result.Found)
            {
                lblInfo.Text = algoName + ": không có đường đi (hai điểm thuộc phần đường tách rời, hoặc bị đường một chiều chặn).";
                return;
            }

            // Các trạm nằm trên đường đi, theo thứ tự
            var route = new List<int>();
            if (graph.Stops.Count > 0)
            {
                int last = result.PathNodes.Count - 1;
                for (int i = 0; i < result.PathNodes.Count; i++)
                {
                    int node = result.PathNodes[i];
                    int pick = -1;
                    if (i == 0 && mapCanvas.StartStop >= 0) pick = mapCanvas.StartStop;
                    else if (i == last && mapCanvas.EndStop >= 0) pick = mapCanvas.EndStop;
                    else if (graph.StopsAtNode[node] != null && graph.StopsAtNode[node].Count > 0) pick = graph.StopsAtNode[node][0];
                    if (pick >= 0) route.Add(pick);
                }
            }

            mapCanvas.RouteStops.Clear();
            mapCanvas.RouteStops.AddRange(route);
            mapCanvas.ShowPath = true;

            double seconds = result.Distance / BusSpeedMps + route.Count * SecondsPerStop;
            lblDist.Text = FormatDistance(result.Distance);
            lblTime.Text = Math.Max(1, (int)Math.Round(seconds / 60.0)) + " phút";
            lblStopCount.Text = graph.Stops.Count > 0 ? route.Count.ToString() : "—";

            lstStops.Items.Clear();
            listStopIndex.Clear();
            for (int i = 0; i < route.Count; i++)
            {
                string mark = i == 0 ? "A  " : (i == route.Count - 1 ? "B  " : (i + 1) + ".  ");
                lstStops.Items.Add("  " + mark + graph.Stops[route[i]].Name);
                listStopIndex.Add(route[i]);
            }

            lblInfo.Text = algoName + ": đã xét " + result.Visited.Count + " giao lộ, đường đi dài " + FormatDistance(result.Distance) +
                           ", thuật toán chạy " + result.ElapsedMs.ToString("F1") + " ms. Thời gian là ước tính (xe 18 km/h, dừng 20 giây mỗi trạm).";
        }

        private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lstStops.SelectedIndex;
            if (i < 0 || i >= listStopIndex.Count || graph == null) return;
            BusStop s = graph.Stops[listStopIndex[i]];
            mapCanvas.Position = new PointLatLng(s.Lat, s.Lng);
            if (mapCanvas.Zoom < 17) mapCanvas.Zoom = 17;
        }

        // Phóng bản đồ để thấy cả điểm đi và điểm đến
        private void FitToEndpoints()
        {
            LatLng a = graph.Nodes[mapCanvas.StartNode], b = graph.Nodes[mapCanvas.EndNode];
            double minLat = Math.Min(a.Lat, b.Lat), maxLat = Math.Max(a.Lat, b.Lat);
            double minLng = Math.Min(a.Lng, b.Lng), maxLng = Math.Max(a.Lng, b.Lng);
            double padLat = Math.Max(0.0015, (maxLat - minLat) * 0.35);
            double padLng = Math.Max(0.0015, (maxLng - minLng) * 0.35);
            minLat -= padLat; maxLat += padLat; minLng -= padLng; maxLng += padLng;
            mapCanvas.SetZoomToFitRect(new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat));
        }

        private static string FormatDistance(double meters)
        {
            return meters >= 1000 ? (meters / 1000).ToString("F2") + " km" : meters.ToString("F0") + " m";
        }
    }
}
