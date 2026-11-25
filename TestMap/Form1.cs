using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using TestMap.Forms;
using TestMap.Mapping;
using TestMap.Routing;
using TestMap.Services;

namespace TestMap
{
    // Giao diện nằm trong Form1.Designer.cs: mở Form1.cs ở chế độ [Design] để kéo thả / chỉnh bằng chuột.
    // File này chỉ chứa phần xử lý (logic).
    //
    // Cách dùng:
    //   - Chọn điểm đi (A) và điểm đến (B): bấm vào trạm trên bản đồ, hoặc gõ tên trạm vào ô A / B.
    //   - Tab "Gợi ý xe buýt": các cách đi bằng xe buýt (số tuyến, giá vé, thời gian, đi bộ bao xa), có đổi tuyến.
    //   - Tab "Ngắn nhất": đường ngắn nhất theo đường bộ (Dijkstra / A*), kèm xe chạy thử dọc tuyến.
    //   - Tab "Các tuyến": danh sách các tuyến xe buýt, chọn một tuyến để xem lộ trình và xe đang chạy.
    //   - Chuột phải lên bản đồ: chọn trạm gần nhất. Phím Esc: xóa.
    public partial class Form1 : Form, IAdminHost
    {
        // ----- Các khu vực có thể chọn (hộp tọa độ gần đúng: nam, tây, bắc, đông). Muốn thêm khu vực thì thêm 1 dòng. -----
        private class Area
        {
            public string Key, Name;
            public double South, West, North, East;
            public bool Offline;        // true: dữ liệu tự tạo có sẵn, không tải lại từ OpenStreetMap
            public Area(string key, string name, double s, double w, double n, double e)
            { Key = key; Name = name; South = s; West = w; North = n; East = e; }
            public override string ToString() { return Name; }
        }

        private static readonly Area[] Areas =
        {
            // Quận 1, 3, 5, 10 gộp thành MỘT bản đồ (hộp bao ngoài cùng của 4 quận).
            new Area("hcm_q1_3_5_10", "TP.HCM - Quận 1, 3, 5, 10 (gộp)", 10.7490, 106.6500, 10.7900, 106.7110),
            new Area("demo_nho", "★ Demo đơn giản (cho thầy)", 10.7690, 106.6890, 10.7790, 106.7020) { Offline = true },
            new Area("hcm_q1",  "TP.HCM - Quận 1",  10.7640, 106.6850, 10.7900, 106.7110),
            new Area("hcm_q3",  "TP.HCM - Quận 3",  10.7700, 106.6660, 10.7900, 106.6910),
            new Area("hcm_q5",  "TP.HCM - Quận 5",  10.7490, 106.6500, 10.7640, 106.6720),
            new Area("hcm_q10", "TP.HCM - Quận 10", 10.7620, 106.6560, 10.7800, 106.6790),
            new Area("hn_hk",   "Hà Nội - Hoàn Kiếm", 21.0170, 105.8380, 21.0380, 105.8600),
            new Area("hn_bd",   "Hà Nội - Ba Đình",   21.0280, 105.8090, 21.0480, 105.8400),
            new Area("dn_hc",   "Đà Nẵng - Hải Châu", 16.0400, 108.1900, 16.0800, 108.2300),
        };

        private const double ExploreSeconds = 2.5;     // hoạt ảnh thuật toán (nếu bật) kéo dài khoảng bao lâu
        private const double RevealSeconds = 0.9;      // tuyến "chạy ra" trong bao lâu
        private const double BusSpeedMps = 5.0;        // tốc độ xe buýt giả định để tính thời gian: 18 km/h
        private const double SecondsPerStop = 20;      // thời gian dừng mỗi trạm giả định

        private static readonly Color Navy = Color.FromArgb(15, 23, 42);
        private static readonly Color Slate = Color.FromArgb(51, 65, 85);
        private static readonly Color Muted = Color.FromArgb(100, 116, 139);
        private static readonly Color PanelBack = Color.FromArgb(248, 250, 252);
        private static readonly Color ListBack = Color.FromArgb(241, 245, 249);
        private static readonly Color Green = Color.FromArgb(22, 163, 74);

        private static readonly Font ListNameFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        private static readonly Font ListSubFont = new Font("Segoe UI", 8.5f);
        private static readonly Font ListMarkFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        private static readonly Font ChipFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font BigTimeFont = new Font("Segoe UI", 15f, FontStyle.Bold);
        private static readonly Font InfoFont = new Font("Segoe UI", 9.5f);
        private static readonly Font SmallBoldFont = new Font("Segoe UI", 8.75f, FontStyle.Bold);
        private static readonly Font SmallFont = new Font("Segoe UI", 8.5f);

        private enum Phase { Idle, Explore, Reveal }
        private enum Mode { Bus, Shortest, Lines, Driver }

        private class StepItem { public Color Color; public string Title, Sub; }

        // ----- Tài khoản, địa điểm, ca của tài xế -----
        private UserAccount account;                                          // null: khách (chưa đăng nhập)
        private List<FavPlace> favorites = new List<FavPlace>();
        private List<Poi> pois = new List<Poi>();
        private const double DriverSpeed = 9.0;                               // m/s (khoảng 32 km/h)
        private const double DriverDwellSeconds = 3.0;                        // dừng ở mỗi trạm
        private BusLine driverLine;
        private bool onShift;
        private double driverMeters, driverDwell;
        private int driverNext;                                               // chỉ số (trong driverLine.Stops) của trạm sắp tới
        private int driverListNext = -2;

        // ----- Trạng thái -----
        private readonly Stopwatch revealWatch = new Stopwatch();
        private readonly Stopwatch liveWatch = Stopwatch.StartNew();
        private RoadGraph graph;
        private SearchResult result;
        private RoutePath route;
        private Phase phase = Phase.Idle;
        private Mode mode = Mode.Bus;
        private bool pickingStart = true;
        private bool busy;
        private bool suppressCombo;
        private int step;
        private int stepsPerTick = 1;
        private string algoName = "";
        private double busMeters;
        private double busSpeed;
        private readonly List<int> listStopIndex = new List<int>();     // lstStops[i] ứng với trạm số mấy
        private readonly List<string> listSub = new List<string>();     // dòng chú thích nhỏ dưới tên trạm

        // Tuyến xe buýt và gợi ý
        private List<BusLine> lines = new List<BusLine>();
        private TransitNet net;
        private bool usingDemo;
        private BusLine selectedLine;
        private List<Itinerary> itins = new List<Itinerary>();
        private List<BusLine> itinLines = new List<BusLine>();          // các tuyến trong hành trình đang chọn
        private readonly List<StepItem> steps = new List<StepItem>();
        private readonly HashSet<string> routeTried = new HashSet<string>();   // khu vực đã thử tải tuyến thật trong lần chạy này

        // Quyền admin
        private bool adminLoggedIn;
        private AdminForm adminForm;
        private Action<double, double> pendingPick;      // đang chờ admin bấm một điểm trên bản đồ
        private Action pendingCancel;
        public event EventHandler DataReloaded;

        public Form1()
        {
            InitializeComponent();      // toàn bộ giao diện nằm trong Form1.Designer.cs

            InitMap();
            foreach (Area a in Areas) cboArea.Items.Add(a);
            cboArea.SelectedIndex = 0;

            SetMode(Mode.Bus);
        }

        // Cấu hình bản đồ (thư viện GMap không cấu hình được trong Designer nên để ở code)
        private void InitMap()
        {
            mapCanvas.EmptyTileColor = MapCanvas.BackgroundColor;
            mapCanvas.ShowCenter = false;
            mapCanvas.DragButton = MouseButtons.Left;
            mapCanvas.MinZoom = 11;
            mapCanvas.MaxZoom = 19;
            mapCanvas.Zoom = 15;
            mapCanvas.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            mapCanvas.StopClicked += MapCanvas_StopClicked;
            mapCanvas.MapClicked += MapCanvas_MapClicked;
            mapCanvas.PoiClicked += MapCanvas_PoiClicked;
            mapCanvas.EdgeClicked += MapCanvas_EdgeClicked;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            mapCanvas.MapProvider = new BlankMapProvider(MapCanvas.BackgroundColor);
        }

        // =====================================================================
        //  SỰ KIỆN CỦA CÁC ĐIỀU KHIỂN (được nối trong Form1.Designer.cs)
        // =====================================================================
        private void LiveTimer_Tick(object sender, EventArgs e)
        {
            mapCanvas.BusClock = liveWatch.Elapsed.TotalSeconds;
            mapCanvas.Invalidate();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mapCanvas.Zoom = Math.Min(mapCanvas.MaxZoom, (int)mapCanvas.Zoom + 1);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mapCanvas.Zoom = Math.Max(mapCanvas.MinZoom, (int)Math.Ceiling(mapCanvas.Zoom) - 1);
        }

        private async void cboArea_SelectionChangeCommitted(object sender, EventArgs e)
        {
            await LoadArea(cboArea.SelectedIndex, false);
        }

        private void chkStops_CheckedChanged(object sender, EventArgs e)
        {
            mapCanvas.ShowStops = chkStops.Checked;
            mapCanvas.Invalidate();
        }

        // "Hiện tuyến" và "Xe chạy"
        private void chkView_CheckedChanged(object sender, EventArgs e)
        {
            ApplyView();
        }

        private bool IsOfflineArea()
        {
            if (!Areas[cboArea.SelectedIndex].Offline) return false;
            lblInfo.Text = "Khu vực demo dùng dữ liệu tự tạo có sẵn, không tải từ OpenStreetMap.";
            return true;
        }

        private async void btnLoadLines_Click(object sender, EventArgs e)
        {
            if (IsOfflineArea()) return;
            if (graph != null && !busy) await DownloadLines(Areas[cboArea.SelectedIndex]);
        }

        private async void btnReload_Click(object sender, EventArgs e)
        {
            if (IsOfflineArea()) return;
            await LoadArea(cboArea.SelectedIndex, true);
        }

        private void cboFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopComboChanged(true);
        }

        private void cboTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopComboChanged(false);
        }

        private void cboRides_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (graph != null && !busy && mapCanvas.StartNode >= 0 && mapCanvas.EndNode >= 0) ComputeItineraries();
        }

        private void btnTabBus_Click(object sender, EventArgs e) { SetMode(Mode.Bus); }
        private void btnTabShortest_Click(object sender, EventArgs e) { SetMode(Mode.Shortest); }
        private void btnTabLines_Click(object sender, EventArgs e) { SetMode(Mode.Lines); }

        private void lnkCopyBus_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { CopyItinerary(); }
        private void lnkCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { CopyRoute(); }

        // Hộp tọa độ từng quận: dùng để ghi "(Q.5)" vào tên các trạm cùng tên ở bản đồ gộp
        private static readonly object[][] Districts =
        {
            new object[] { "Q.1",  10.7640, 106.6850, 10.7900, 106.7110 },
            new object[] { "Q.3",  10.7700, 106.6660, 10.7900, 106.6910 },
            new object[] { "Q.5",  10.7490, 106.6500, 10.7640, 106.6720 },
            new object[] { "Q.10", 10.7620, 106.6560, 10.7800, 106.6790 },
        };

        // Quận của một điểm (ước lượng theo hộp tọa độ; nếu nằm trong nhiều hộp thì lấy quận có tâm gần nhất)
        private static string ZoneOf(double lat, double lng)
        {
            string best = null;
            double bestD = double.MaxValue;
            foreach (object[] d in Districts)
            {
                double s0 = (double)d[1], w0 = (double)d[2], n0 = (double)d[3], e0 = (double)d[4];
                if (lat < s0 || lat > n0 || lng < w0 || lng > e0) continue;
                double dy = lat - (s0 + n0) / 2, dx = lng - (w0 + e0) / 2, dist = dx * dx + dy * dy;
                if (dist < bestD) { bestD = dist; best = (string)d[0]; }
            }
            return best;
        }

        // Khoảng cách xấp xỉ giữa hai điểm (mét)
        private static double Meters(double lat1, double lng1, double lat2, double lng2)
        {
            double dy = (lat2 - lat1) * 111320.0, dx = (lng2 - lng1) * 111320.0 * Math.Cos((lat1 + lat2) * Math.PI / 360.0);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // =====================================================================
        //  CHẾ ĐỘ HIỂN THỊ (4 tab)
        // =====================================================================
        private void SetMode(Mode m)
        {
            mode = m;
            pnlBus.Visible = m == Mode.Bus;
            pnlShortest.Visible = m == Mode.Shortest;
            pnlLines.Visible = m == Mode.Lines;
            pnlDriver.Visible = m == Mode.Driver;
            TabStyle(btnTabDriver, m == Mode.Driver);
            TabStyle(btnTabBus, m == Mode.Bus);
            TabStyle(btnTabShortest, m == Mode.Shortest);
            TabStyle(btnTabLines, m == Mode.Lines);
            ApplyView();
            if (m == Mode.Bus && lstItins.SelectedIndex >= 0) SelectItinerary(lstItins.SelectedIndex);
        }

        private static void TabStyle(Button b, bool active)
        {
            b.BackColor = active ? MapCanvas.RouteColor : Color.FromArgb(226, 232, 240);
            b.ForeColor = active ? Color.White : Slate;
        }

        // Quyết định bản đồ vẽ gì tùy theo tab đang mở
        private void ApplyView()
        {
            bool pathOk = result != null && result.Found && route != null;
            // Đang mô phỏng từng bước thì bản đồ theo bảng mô phỏng, không theo tab
            mapCanvas.ShowPath = simActive ? (pathOk && algoStep >= result.Visited.Count)
                                           : (mode == Mode.Shortest && pathOk && phase != Phase.Explore);
            mapCanvas.ShowSearch = simActive || (mode == Mode.Shortest && chkSearch.Checked);
            mapCanvas.ShowItinerary = simBusLegs || (!simActive && mode == Mode.Bus && mapCanvas.ItinLegs.Count > 0);

            // Tuyến vẽ nền: ở tab "Các tuyến" luôn vẽ hết; ở tab "Gợi ý" tùy ô "Hiện tuyến"; ở tab "Ngắn nhất" không vẽ
            if (mode == Mode.Driver)
            {
                // Tab tài xế: chỉ vẽ tuyến đang chạy ca (chưa chạy ca thì vẽ tuyến đang chọn trong ô)
                BusLine pick = driverLine;
                if (pick == null && cboDriverLine.SelectedIndex >= 0 && cboDriverLine.SelectedIndex < lines.Count) pick = lines[cboDriverLine.SelectedIndex];
                mapCanvas.NetLines = pick != null ? new List<BusLine> { pick } : new List<BusLine>();
                mapCanvas.SelectedLine = pick;
            }
            else
            {
                mapCanvas.NetLines = (mode == Mode.Lines || (mode == Mode.Bus && chkLines.Checked)) ? lines : new List<BusLine>();
                mapCanvas.SelectedLine = mode == Mode.Lines ? selectedLine : null;
            }

            // Tuyến có xe chạy: tuyến đang chọn, hoặc các tuyến của hành trình đang xem, hoặc tất cả
            var runners = new List<BusLine>();
            if (mode == Mode.Lines) { if (selectedLine != null) runners.Add(selectedLine); else runners.AddRange(lines); }
            else if (mode == Mode.Bus) runners.AddRange(itinLines);
            mapCanvas.BusLines = runners;
            mapCanvas.ShowLiveBuses = chkBuses.Checked && runners.Count > 0;

            if (mapCanvas.ShowLiveBuses) { if (!liveTimer.Enabled) liveTimer.Start(); }
            else liveTimer.Stop();
            mapCanvas.Invalidate();
        }

        // =====================================================================
        //  TẢI DỮ LIỆU KHU VỰC
        // =====================================================================
        private async void Form1_Shown(object sender, EventArgs e)
        {
            await LoadArea(cboArea.SelectedIndex, false);
        }

        public string DataRoot
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"); }
        }

        private void Report(string msg)
        {
            if (IsHandleCreated)
                BeginInvoke(new Action(() =>
                {
                    lblInfo.Text = msg;
                    if (pbLoad.Marquee) { lblProgress.Text = msg; }     // đang tải mạng: hiện "Đã nhận x MB"
                }));
        }

        // ----- Thanh tiến trình tải dữ liệu (luôn hiện ở dưới cùng) -----
        // percent < 0: không biết tổng (đang tải từ mạng) nên chạy kiểu "đang chờ"
        private void SetProgress(int percent, string text)
        {
            progressTimer.Stop();
            if (percent < 0)
            {
                pbLoad.Marquee = true;                 // chưa biết tổng: thanh chạy qua lại
                pbLoad.Caption = "Đang tải...";
                lblProgress.Text = text;
            }
            else
            {
                pbLoad.Marquee = false;
                pbLoad.Value = Math.Max(0, Math.Min(100, percent));     // phần trăm hiện ngay giữa thanh
                lblProgress.Text = text;
            }
            pnlProgress.Update();
        }

        // Từ luồng nền
        private void Prog(int percent, string text)
        {
            if (IsHandleCreated) BeginInvoke(new Action(() => SetProgress(percent, text)));
        }

        private void FinishProgress(string text)
        {
            SetProgress(100, text);
            progressTimer.Stop();
            progressTimer.Start();          // 1,5 giây sau tự về "Sẵn sàng"
        }

        private void ResetProgress()
        {
            progressTimer.Stop();
            pbLoad.Marquee = false;
            pbLoad.Value = 0;
            lblProgress.Text = "Sẵn sàng";
        }

        private void progressTimer_Tick(object sender, EventArgs e) { ResetProgress(); }

        private void SetBusy(bool value)
        {
            busy = value;
            cboArea.Enabled = !value;
            cboFrom.Enabled = !value;
            cboTo.Enabled = !value;
            cboRides.Enabled = !value;
            btnReload.Enabled = !value;
            btnLoadPoi.Enabled = !value;
            btnLocate.Enabled = !value;
            btnLoadLines.Enabled = !value;
            btnRun.Enabled = !value;
            btnCompare.Enabled = !value;
            btnSwap.Enabled = !value;
            btnReset.Enabled = !value;
            btnBus.Enabled = !value;
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
            string routesPath = Path.Combine(dir, "routes.csv");
            bool cached = File.Exists(nodesPath) && File.Exists(edgesPath) && File.Exists(stopsPath);
            bool needLines = false;
            if (a.Offline)
            {
                force = false;
                if (!cached) { lblInfo.Text = "Thiếu dữ liệu demo trong thư mục Data/" + a.Key + "."; return; }
            }

            timer.Stop();
            SetBusy(true);
            bool loaded = false;
            SetProgress(0, "Bắt đầu");
            try
            {
                if (force || !cached)
                {
                    try
                    {
                        lblInfo.Text = "Đang tải dữ liệu " + a.Name + "...";
                        SetProgress(-1, "Tải đường và trạm từ OpenStreetMap...");
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
                    SetProgress(0, "Đọc dữ liệu đường");
                    g = await Task.Run(() =>
                    {
                        RoadGraph gg = RoadGraph.Load(nodesPath, edgesPath, (pct, txt) => Prog(pct * 60 / 100, txt));
                        Prog(62, "Đọc trạm xe buýt");
                        gg.LoadStops(stopsPath);
                        return gg;
                    });
                }
                catch (Exception ex)
                {
                    lblInfo.Text = "Không đọc được dữ liệu: " + ex.Message;
                    return;
                }

                // Tuyến xe buýt: dùng tuyến thật đã lưu; chưa có thì tạo tuyến demo
                lblInfo.Text = "Đang dựng các tuyến xe buýt...";
                SetProgress(65, "Dựng các tuyến xe buýt");
                string adminPath = Path.Combine(dir, "lines_admin.csv");
                List<BusLine> ls = null;
                bool demo = false;
                try
                {
                    ls = await Task.Run(() =>
                    {
                        // ưu tiên tuyến do admin chỉnh sửa, rồi tới tuyến thật đã tải về
                        try { if (File.Exists(adminPath)) return AdminData.LoadLines(g, adminPath); } catch { }
                        if (!File.Exists(routesPath)) return null;
                        try { return BusLines.Load(g, routesPath, (pct, txt) => Prog(65 + pct * 25 / 100, txt)); } catch { return null; }
                    });
                }
                catch { ls = null; }
                bool adminFile = ls != null && File.Exists(adminPath);
                if (!adminFile && (ls == null || ls.Count == 0))
                {
                    ls = await Task.Run(() => BusLines.Demo(g));
                    demo = true;
                }

                SetProgress(92, "Dựng mạng gợi ý và giao diện");
                ApplyGraph(g, a, ls, demo);
                loaded = true;
                needLines = force || (demo && !routeTried.Contains(a.Key));
            }
            finally
            {
                SetBusy(false);
                if (loaded) FinishProgress("Hoàn tất");
                else ResetProgress();
            }

            // Thử tải tuyến thật ở nền (không chặn giao diện); lỗi thì vẫn dùng tuyến demo
            if (needLines) { var ignored = DownloadLines(a); }
        }

        private void ApplyGraph(RoadGraph g, Area a, List<BusLine> ls, bool demo)
        {
            graph = g;
            chkNodes.Checked = a.Offline;            // khu vực demo nhỏ: bật sẵn đỉnh và trọng số cho dễ xem
            chkWeights.Checked = a.Offline;
            g.AssignLabels(a.Key == "hcm_q1_3_5_10" ? (Func<double, double, string>)ZoneOf : null);
            LoadPois(a);
            mapCanvas.RoadNet = g;
            mapCanvas.StartNode = mapCanvas.EndNode = -1;
            mapCanvas.StartStop = mapCanvas.EndStop = -1;
            ResetAnimation();
            SetLines(ls, demo);
            FillStopCombos();
            if (lines.Count == 0 && mode != Mode.Shortest) SetMode(Mode.Shortest);

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

            lblInfo.Text = a.Name + ": " + g.Nodes.Count + " giao lộ, " + g.Stops.Count + " trạm, " + lines.Count + " tuyến" +
                           (demo ? " (demo)" : "") + ". Bấm vào trạm trên bản đồ để chọn điểm đi (A) rồi điểm đến (B)." +
                           (g.Stops.Count == 0 ? " (Khu vực này chưa có trạm trong OpenStreetMap.)" : "");

            // Cho xem ngay một hành trình mẫu giữa hai điểm mặc định
            if (g.Stops.Count >= 2) RunSearch();
        }

        // Đặt bộ tuyến mới: dựng mạng đổi tuyến, nạp danh sách
        private void SetLines(List<BusLine> ls, bool demo)
        {
            lines = ls ?? new List<BusLine>();
            usingDemo = demo;
            selectedLine = null;
            net = (graph != null && lines.Count > 0) ? TransitNet.Build(graph, lines) : null;
            lblDemoBus.Visible = lblDemoLines.Visible = demo && lines.Count > 0;

            lstLines.Items.Clear();
            lstLines.Items.Add("Tất cả các tuyến");
            foreach (BusLine l in lines) lstLines.Items.Add(l.Ref);
            lstLines.SelectedIndex = 0;
            FillDriverLines();
        }

        // Tải tuyến xe buýt thật của khu vực từ OpenStreetMap rồi nạp vào
        private async Task DownloadLines(Area a)
        {
            SetProgress(-1, "Tải tuyến xe buýt từ OpenStreetMap...");
            try { await DownloadLinesCore(a); }
            finally { FinishProgress("Xong"); }
        }

        private async Task DownloadLinesCore(Area a)
        {
            RoadGraph g0 = graph;
            if (g0 == null) return;
            string path = Path.Combine(DataRoot, a.Key, "routes.csv");
            routeTried.Add(a.Key);
            btnLoadLines.Enabled = false;
            lblInfo.Text = "Đang tải tuyến xe buýt " + a.Name + " từ OpenStreetMap...";

            int count = 0;
            string err = null;
            try { count = await Task.Run(() => BusLines.Download(a.South, a.West, a.North, a.East, path, Report)); }
            catch (Exception ex) { err = ex.Message; }
            btnLoadLines.Enabled = !busy;
            if (graph != g0) return;                       // trong lúc chờ người dùng đã đổi khu vực

            if (err != null)
            {
                lblInfo.Text = "Không tải được tuyến xe buýt: " + err + (usingDemo ? ". Đang dùng tuyến demo." : ".");
                return;
            }
            if (count == 0)
            {
                lblInfo.Text = "OpenStreetMap chưa có tuyến xe buýt nào ở " + a.Name + (usingDemo ? " - giữ tuyến demo." : ".");
                return;
            }

            string adminFilePath = Path.Combine(Path.GetDirectoryName(path), "lines_admin.csv");
            if (File.Exists(adminFilePath))
            {
                lblInfo.Text = "Đã tải " + count + " tuyến thật nhưng đang dùng tuyến do admin chỉnh sửa. " +
                               "Vào Quản trị > Cập nhật dữ liệu > \"Khôi phục tuyến gốc\" để dùng tuyến vừa tải.";
                return;
            }

            List<BusLine> ls;
            try { ls = await Task.Run(() => BusLines.Load(g0, path)); }
            catch (Exception ex) { lblInfo.Text = "Không đọc được tuyến xe buýt: " + ex.Message; return; }
            if (graph != g0) return;
            if (ls.Count == 0)
            {
                lblInfo.Text = "Đã tải " + count + " tuyến nhưng không khớp được với trạm nào trong khu vực này" + (usingDemo ? " - giữ tuyến demo." : ".");
                return;
            }

            SetLines(ls, false);
            var refs = new HashSet<string>();
            foreach (BusLine l in ls) refs.Add(l.Ref);
            lblInfo.Text = "Đã nạp " + refs.Count + " tuyến xe buýt thật (" + ls.Count + " lượt đi/về).";
            if (mapCanvas.StartNode >= 0 && mapCanvas.EndNode >= 0 && mapCanvas.StartNode != mapCanvas.EndNode) ComputeItineraries();
            else ApplyView();
        }

        // Nạp danh sách trạm (sắp theo tên) vào hai ô chọn A / B
        private void FillStopCombos()
        {
            suppressCombo = true;
            try
            {
                cboFrom.Items.Clear();
                cboTo.Items.Clear();
                cboFrom.Text = "";
                cboTo.Text = "";
                if (graph == null) return;

                var sorted = new List<BusStop>(graph.Stops);
                sorted.Sort((x, y) => string.Compare(x.ToString(), y.ToString(), StringComparison.CurrentCultureIgnoreCase));
                foreach (BusStop s in sorted)
                {
                    cboFrom.Items.Add(s);
                    cboTo.Items.Add(s);
                }
            }
            finally
            {
                suppressCombo = false;
            }
        }

        // =====================================================================
        //  CHỌN ĐIỂM ĐI / ĐẾN
        // =====================================================================
        // Đặt điểm đi / đến và cập nhật ô chọn tương ứng
        private void SetPoint(bool isStart, int stop, int node)
        {
            if (isStart) { mapCanvas.StartStop = stop; mapCanvas.StartNode = node; }
            else { mapCanvas.EndStop = stop; mapCanvas.EndNode = node; }

            ComboBox combo = isStart ? cboFrom : cboTo;
            suppressCombo = true;
            try
            {
                if (graph != null && stop >= 0 && stop < graph.Stops.Count) combo.SelectedItem = graph.Stops[stop];
                else
                {
                    combo.SelectedIndex = -1;
                    combo.Text = node >= 0 ? "Giao lộ #" + node : "";
                }
            }
            finally
            {
                suppressCombo = false;
            }
        }

        // Chọn trạm trong ô A / B
        private void StopComboChanged(bool isStart)
        {
            if (suppressCombo || graph == null || busy) return;
            BusStop s = (isStart ? cboFrom : cboTo).SelectedItem as BusStop;
            if (s == null) return;

            int idx = graph.Stops.IndexOf(s);
            ResetAnimation();
            SetPoint(isStart, idx, s.Node);
            pickingStart = !isStart;
            if (!MapCanvas.InView(s.Lat, s.Lng, mapCanvas.ViewArea)) mapCanvas.Position = new PointLatLng(s.Lat, s.Lng);
            mapCanvas.Invalidate();

            if (mapCanvas.StartNode >= 0 && mapCanvas.EndNode >= 0 && mapCanvas.StartNode != mapCanvas.EndNode) RunSearch();
        }

        // Bấm vào một trạm: lần lượt chọn điểm đi, điểm đến. Chọn xong điểm đến thì tự tìm đường.
        private void MapCanvas_StopClicked(int stop)
        {
            if (pendingPick != null)                        // admin đang chọn vị trí: lấy luôn vị trí trạm vừa bấm
            {
                CompletePick(graph.Stops[stop].RawLat, graph.Stops[stop].RawLng);
                return;
            }
            if (graph == null || busy) return;
            PickPoint(stop, graph.Stops[stop].Node);
        }

        // Chuột phải: chọn trạm gần nhất (hoặc giao lộ gần nhất nếu khu vực không có trạm)
        private void MapCanvas_MapClicked(double lat, double lng)
        {
            if (pendingPick != null) CompletePick(lat, lng);
        }

        private void CompletePick(double lat, double lng)
        {
            Action<double, double> cb = pendingPick;
            pendingPick = null;
            pendingCancel = null;
            lblInfo.Text = "Đã chọn vị trí.";
            if (cb != null) cb(lat, lng);
        }

        private void CancelPick()
        {
            Action cancel = pendingCancel;
            pendingPick = null;
            pendingCancel = null;
            lblInfo.Text = "Đã hủy chọn vị trí.";
            if (cancel != null) cancel();
        }

        private void MapCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || graph == null || busy || pendingPick != null) return;

            PointLatLng p = mapCanvas.FromLocalToLatLng(e.X, e.Y);
            int stop = graph.Stops.Count > 0 ? graph.NearestStop(p.Lat, p.Lng) : -1;
            int node = stop >= 0 ? graph.Stops[stop].Node : graph.Nearest(p.Lat, p.Lng);
            PickPoint(stop, node);
        }

        private void PickPoint(int stop, int node)
        {
            ResetAnimation();
            bool wasStart = pickingStart;
            SetPoint(wasStart, stop, node);
            pickingStart = !wasStart;
            mapCanvas.Invalidate();

            if (wasStart)
                lblInfo.Text = "Đã chọn điểm đi (A). Bấm vào trạm khác để chọn điểm đến (B).";
            else
                RunSearch();
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            if (graph == null) return;
            bool hadRoute = route != null || itins.Count > 0;
            int s1 = mapCanvas.StartStop, n1 = mapCanvas.StartNode;
            ResetAnimation();
            SetPoint(true, mapCanvas.EndStop, mapCanvas.EndNode);
            SetPoint(false, s1, n1);
            mapCanvas.Invalidate();
            if (hadRoute) RunSearch();
        }

        // =====================================================================
        //  TÌM ĐƯỜNG (đường ngắn nhất + gợi ý xe buýt)
        // =====================================================================
        private bool HasValidEndpoints()
        {
            if (graph == null) return false;
            if (mapCanvas.StartNode < 0 || mapCanvas.EndNode < 0 || mapCanvas.StartNode == mapCanvas.EndNode)
            {
                lblInfo.Text = "Hãy chọn điểm đi và điểm đến khác nhau (bấm vào hai trạm trên bản đồ).";
                return false;
            }
            return true;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            RunSearch();
        }

        private void RunSearch()
        {
            if (!HasValidEndpoints()) return;

            ResetAnimation();

            bool useAStar = cboAlgo.SelectedIndex == 1;
            algoName = useAStar ? "A*" : "Dijkstra";

            // Thuật toán chạy xong ngay; hoạt ảnh chỉ phát lại thứ tự các giao lộ đã xét
            result = PathFinder.Find(graph, mapCanvas.StartNode, mapCanvas.EndNode, useAStar);
            mapCanvas.LastResult = result;
            mapCanvas.VisitedShown = 0;

            if (result.Found)
            {
                route = RoutePath.Build(graph, result);
                mapCanvas.Route = route;
                FitToRoute();
            }
            else
            {
                FitToEndpoints();
            }

            if (chkSearch.Checked && mode == Mode.Shortest)
            {
                double ticks = ExploreSeconds * 1000.0 / timer.Interval;
                stepsPerTick = Math.Max(1, (int)Math.Ceiling(result.Visited.Count / ticks));
                step = 0;
                phase = Phase.Explore;
                lblInfo.Text = "Đang chạy " + algoName + "...";
                ApplyView();
                timer.Start();
            }
            else if (result.Found)
            {
                StartReveal();
            }
            else
            {
                FinishSearch();
            }

            // Gợi ý các cách đi bằng xe buýt giữa hai điểm
            ComputeItineraries();
        }

        // Hiện tuyến: điền thông tin vào bảng bên phải rồi cho tuyến "chạy ra" từ A tới B
        private void StartReveal()
        {
            FillRouteInfo();
            phase = Phase.Reveal;
            mapCanvas.RouteProgress = 0;
            revealWatch.Reset();
            revealWatch.Start();
            ApplyView();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (result == null) { timer.Stop(); return; }

            if (phase == Phase.Explore)
            {
                step = Math.Min(step + stepsPerTick, result.Visited.Count);
                mapCanvas.VisitedShown = step;
                lblVisited.Text = algoName + ": đang xét giao lộ " + step + "/" + result.Visited.Count;
                if (step >= result.Visited.Count)
                {
                    timer.Stop();
                    if (result.Found) StartReveal(); else FinishSearch();
                }
            }
            else if (phase == Phase.Reveal)
            {
                double t = Math.Min(1.0, revealWatch.Elapsed.TotalSeconds / RevealSeconds);
                mapCanvas.RouteProgress = 1 - Math.Pow(1 - t, 3);      // chạy nhanh rồi chậm dần
                if (t >= 1.0)
                {
                    timer.Stop();
                    FinishSearch();
                }
            }
            mapCanvas.Invalidate();
        }

        // Tính các trạm nằm trên tuyến, quãng đường tới từng trạm, rồi điền thống kê và danh sách
        private void FillRouteInfo()
        {
            var stopsOnRoute = new List<int>();
            var dists = new List<double>();
            if (graph.Stops.Count > 0)
            {
                int last = result.PathNodes.Count - 1;
                for (int i = 0; i <= last; i++)
                {
                    int node = result.PathNodes[i];
                    int pick = -1;
                    if (i == 0 && mapCanvas.StartStop >= 0) pick = mapCanvas.StartStop;
                    else if (i == last && mapCanvas.EndStop >= 0) pick = mapCanvas.EndStop;
                    else if (graph.StopsAtNode[node] != null && graph.StopsAtNode[node].Count > 0) pick = graph.StopsAtNode[node][0];
                    if (pick >= 0)
                    {
                        stopsOnRoute.Add(pick);
                        dists.Add(route.NodeDist[i]);
                    }
                }
            }

            mapCanvas.RouteStops.Clear();
            mapCanvas.RouteStops.AddRange(stopsOnRoute);
            mapCanvas.RouteStopDist.Clear();
            mapCanvas.RouteStopDist.AddRange(dists);

            double seconds = result.Distance / BusSpeedMps + stopsOnRoute.Count * SecondsPerStop;
            lblDist.Text = FormatDistance(result.Distance);
            lblTime.Text = Math.Max(1, (int)Math.Round(seconds / 60.0)) + " phút";
            lblStopCount.Text = graph.Stops.Count > 0 ? stopsOnRoute.Count.ToString() : "—";

            lstStops.Items.Clear();
            listStopIndex.Clear();
            listSub.Clear();
            for (int i = 0; i < stopsOnRoute.Count; i++)
            {
                lstStops.Items.Add(graph.Stops[stopsOnRoute[i]].ToString());
                listStopIndex.Add(stopsOnRoute[i]);
                if (i == 0) listSub.Add("Điểm xuất phát");
                else
                {
                    double eta = dists[i] / BusSpeedMps + i * SecondsPerStop;
                    listSub.Add(FormatDistance(dists[i]) + "  ·  khoảng " + Math.Max(1, (int)Math.Round(eta / 60.0)) + " phút");
                }
            }
        }

        private void FinishSearch()
        {
            phase = Phase.Idle;
            if (!result.Found)
            {
                lblVisited.Text = "";
                if (mode == Mode.Shortest)
                    lblInfo.Text = algoName + ": không có đường đi (hai điểm thuộc phần đường tách rời, hoặc bị đường một chiều chặn).";
                ApplyView();
                return;
            }

            mapCanvas.RouteProgress = 1;
            lblVisited.Text = algoName + " xét " + result.Visited.Count + " giao lộ  ·  " + result.ElapsedMs.ToString("F1") + " ms";
            if (mode == Mode.Shortest)
                lblInfo.Text = "Đường ngắn nhất dài " + FormatDistance(result.Distance) + ", đi qua " + mapCanvas.RouteStops.Count +
                               " trạm. Thời gian là ước tính (xe 18 km/h, dừng 20 giây mỗi trạm). Bấm \"Chạy thử\" để xem xe chạy.";
            ApplyView();
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
            ClearAll();
        }

        private void ClearAll()
        {
            ResetAnimation();
            mapCanvas.HighlightStop = -1;
            lblInfo.Text = "Bấm vào trạm trên bản đồ để chọn điểm đi (A), rồi điểm đến (B).";
        }

        private void ResetAnimation()
        {
            ClearSimOverlay();
            RaiseSimInvalidated();
            timer.Stop();
            StopBus();
            phase = Phase.Idle;
            result = null;
            route = null;
            step = 0;
            mapCanvas.LastResult = null;
            mapCanvas.Route = null;
            mapCanvas.VisitedShown = 0;
            mapCanvas.RouteProgress = 1;
            mapCanvas.ShowPath = false;
            mapCanvas.HighlightStop = -1;
            mapCanvas.RouteStops.Clear();
            mapCanvas.RouteStopDist.Clear();
            lblDist.Text = lblTime.Text = lblStopCount.Text = "—";
            lblVisited.Text = "";
            lstStops.Items.Clear();
            listStopIndex.Clear();
            listSub.Clear();

            // gợi ý xe buýt
            allItins = new List<Itinerary>();
            itinWalkFallback = false;
            lblItinCount.Text = "";
            itins = new List<Itinerary>();
            itinLines = new List<BusLine>();
            steps.Clear();
            lstSteps.Items.Clear();
            lstItins.Items.Clear();
            mapCanvas.ItinLegs = new List<MapLeg>();
            mapCanvas.ShowItinerary = false;
            lblItinMsg.Visible = true;
            lblItinMsg.Text = "Chọn điểm đi và điểm đến (bấm vào trạm trên bản đồ) để xem các cách đi bằng xe buýt.";
            ApplyView();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && pendingPick != null)
            {
                CancelPick();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Escape && !cboFrom.DroppedDown && !cboTo.DroppedDown && !busy)
            {
                ClearAll();
                e.Handled = true;
            }
        }

        // =====================================================================
        //  GỢI Ý XE BUÝT
        // =====================================================================
        private List<Itinerary> allItins = new List<Itinerary>();   // mọi cách đi tìm được (trước khi lọc)
        private string itinMsgText = "";
        private bool itinWalkFallback;                                 // true: không có xe, đang gợi ý đi bộ

        private void ComputeItineraries()
        {
            allItins = new List<Itinerary>();
            itinWalkFallback = false;
            itins = new List<Itinerary>();
            itinLines = new List<BusLine>();
            steps.Clear();
            lstSteps.Items.Clear();
            lstItins.Items.Clear();
            mapCanvas.ItinLegs = new List<MapLeg>();

            int maxRides = cboRides.SelectedIndex + 1;
            bool ready = mapCanvas.StartStop >= 0 && mapCanvas.EndStop >= 0 && mapCanvas.StartStop != mapCanvas.EndStop;
            string msg = "";
            if (net == null)
                msg = "Khu vực này chưa có tuyến xe buýt nên không gợi ý được xe. Thử bấm \"Tải tuyến thật\".";
            else if (mapCanvas.StartStop < 0 || mapCanvas.EndStop < 0)
                msg = "Hãy chọn điểm đi và điểm đến là các trạm xe buýt (bấm vào trạm trên bản đồ).";
            else if (mapCanvas.StartStop == mapCanvas.EndStop)
                msg = "Điểm đi và điểm đến là cùng một trạm.";
            else
            {
                allItins = BusPlanner.Plan(net, mapCanvas.StartStop, mapCanvas.EndStop, maxRides);
                if (allItins.Count == 0)
                    msg = "Không có cách đi bằng xe buýt (tối đa " + maxRides + " chuyến) giữa hai trạm này." +
                          (maxRides < 3 ? " Thử tăng số chuyến tối đa." : "");
            }

            // Không có xe thì gợi ý đi bộ; có xe mà hai trạm gần nhau (<= 1 km) thì vẫn kèm phương án đi bộ để so sánh
            if (ready && graph != null)
            {
                Itinerary walk = BusPlanner.WalkOnly(graph, mapCanvas.StartStop, mapCanvas.EndStop);
                if (walk != null && (allItins.Count == 0 || walk.WalkMeters <= 1000))
                {
                    if (allItins.Count == 0)
                    {
                        itinWalkFallback = true;
                        msg += " Gợi ý đi bộ: " + FormatDistance(walk.WalkMeters) + ", khoảng " + Minutes(walk.Seconds) + " phút.";
                    }
                    allItins.Add(walk);
                }
                else if (walk == null && allItins.Count == 0)
                    msg += " Cũng không có đường đi bộ nối hai điểm (có thể đường bị chặn).";
            }

            itinMsgText = msg;
            ApplyItinFilter();
        }

        // Áp bộ lọc / sắp xếp (bảng "Lọc gợi ý") lên mọi cách đi, rồi điền danh sách
        private void ApplyItinFilter()
        {
            double[] walkLimits = { double.MaxValue, 300, 500, 800, 1200 };
            double walkLimit = walkLimits[Math.Max(0, Math.Min(walkLimits.Length - 1, cboWalkLimit.SelectedIndex))];

            var list = new List<Itinerary>();
            foreach (Itinerary it in allItins)
            {
                if (it.WalkOnly && !chkWalkSuggest.Checked) continue;
                if (it.WalkMeters > walkLimit) continue;
                list.Add(it);
            }

            int sort = cboItinSort.SelectedIndex;
            list.Sort((x, y) =>
            {
                int c = 0;
                if (sort == 1) c = x.Rides.CompareTo(y.Rides);
                else if (sort == 2) c = x.FareVnd.CompareTo(y.FareVnd);
                else if (sort == 3) c = x.WalkMeters.CompareTo(y.WalkMeters);
                return c != 0 ? c : x.Seconds.CompareTo(y.Seconds);
            });

            itins = list;
            itinLines = new List<BusLine>();
            steps.Clear();
            lstSteps.Items.Clear();
            lstItins.Items.Clear();
            mapCanvas.ItinLegs = new List<MapLeg>();
            foreach (Itinerary it in itins) lstItins.Items.Add(it.Signature);

            lblItinCount.Text = allItins.Count == 0 ? "" : itins.Count + "/" + allItins.Count + " cách đi";
            if (itins.Count == 0)
            {
                lblItinMsg.Text = allItins.Count > 0
                    ? "Không có cách đi nào khớp bộ lọc. Hãy nới giới hạn đi bộ hoặc bật \"Gợi ý đi bộ\"."
                    : itinMsgText;
                lblItinMsg.Visible = true;
            }
            else
            {
                lblItinMsg.Text = itinMsgText;
                lblItinMsg.Visible = itinWalkFallback;
            }

            if (itins.Count > 0) lstItins.SelectedIndex = 0;          // kích hoạt lstItins_SelectedIndexChanged
            else ApplyView();
        }

        private void cboItinSort_SelectedIndexChanged(object sender, EventArgs e) { ApplyItinFilter(); }
        private void cboWalkLimit_SelectedIndexChanged(object sender, EventArgs e) { ApplyItinFilter(); }
        private void chkWalkSuggest_CheckedChanged(object sender, EventArgs e) { ApplyItinFilter(); }

        private void lstItins_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lstItins.SelectedIndex;
            if (i >= 0 && i < itins.Count) SelectItinerary(i);
        }

        // Vẽ cách đi đã chọn lên bản đồ và liệt kê từng bước
        private void SelectItinerary(int idx)
        {
            if (idx < 0 || idx >= itins.Count || graph == null) return;
            Itinerary it = itins[idx];
            mapCanvas.ItinLegs = it.ToMapLegs(graph);

            itinLines = new List<BusLine>();
            foreach (TransitLeg leg in it.Legs)
                if (!leg.Walk && !itinLines.Contains(leg.Line)) itinLines.Add(leg.Line);

            steps.Clear();
            steps.Add(new StepItem { Color = MapCanvas.StartColor, Title = "Xuất phát: " + StopName(mapCanvas.StartStop), Sub = "" });
            foreach (TransitLeg leg in it.Legs)
            {
                if (leg.Walk)
                    steps.Add(new StepItem
                    {
                        Color = Muted,
                        Title = "Đi bộ " + Math.Round(leg.Meters) + " m",
                        Sub = "tới trạm " + StopName(leg.ToStop)
                    });
                else
                    steps.Add(new StepItem
                    {
                        Color = leg.Line.Color,
                        Title = "Tuyến " + leg.Line.Ref + "  ·  lên tại " + StopName(leg.FromStop),
                        Sub = "xuống tại " + StopName(leg.ToStop) + "  ·  " + leg.StopCount + " trạm  ·  " + FormatDistance(leg.Meters)
                    });
            }
            steps.Add(new StepItem { Color = MapCanvas.EndColor, Title = "Đến: " + StopName(mapCanvas.EndStop), Sub = "" });
            lstSteps.Items.Clear();
            foreach (StepItem s in steps) lstSteps.Items.Add(s.Title);

            ApplyView();
            if (mode == Mode.Bus && it.WalkOnly)
            {
                FitToLegs(mapCanvas.ItinLegs);
                lblInfo.Text = "Gợi ý đi bộ: " + FormatDistance(it.WalkMeters) + ", khoảng " + Minutes(it.Seconds) + " phút theo đường thật (tốc độ đi bộ ~4,3 km/h).";
            }
            else if (mode == Mode.Bus)
            {
                FitToLegs(mapCanvas.ItinLegs);
                lblInfo.Text = "Gợi ý: khoảng " + Minutes(it.Seconds) + " phút, " + it.Rides + " chuyến" +
                               (it.Transfers > 0 ? " (đổi tuyến " + it.Transfers + " lần)" : "") +
                               ". Giờ chờ xe tính theo tần suất và giá vé của từng tuyến (admin chỉnh được); thời gian là ước tính." +
                               (usingDemo ? " Đang dùng tuyến demo." : "");
            }
        }

        private string StopName(int stop)
        {
            return graph != null && stop >= 0 && stop < graph.Stops.Count ? graph.Stops[stop].ToString() : "điểm đã chọn";
        }

        private static int Minutes(double seconds)
        {
            return Math.Max(1, (int)Math.Round(seconds / 60.0));
        }

        // Thẻ gợi ý: số tuyến, giá vé, thời gian. Nếu phải đi nhiều chuyến thì các chuyến kế tiếp hiện CẠNH NHAU ngay trên thẻ
        // (mỗi chuyến một ô: tuyến nào, lên ở đâu, xuống ở đâu, đi mấy trạm, chờ bao lâu).
        private void lstItins_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= itins.Count) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Itinerary it = itins[e.Index];
            bool selected = (e.State & DrawItemState.Selected) != 0;

            using (var bg = new SolidBrush(ListBack)) g.FillRectangle(bg, e.Bounds);
            var card = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 6, e.Bounds.Width - 20, e.Bounds.Height - 12);
            using (GraphicsPath path = RoundPath(card, 10))
            {
                g.FillPath(Brushes.White, path);
                using (var border = new Pen(selected ? MapCanvas.RouteColor : Color.FromArgb(226, 232, 240), selected ? 2.2f : 1f))
                    g.DrawPath(border, path);
            }

            // Thời gian bên phải: số to màu xanh + chữ "phút"
            int timeX = card.Right - 62;
            TextRenderer.DrawText(g, Minutes(it.Seconds).ToString(), BigTimeFont, new Rectangle(timeX, card.Y + 6, 52, 26), Green,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, "phút", ListSubFont, new Rectangle(timeX, card.Y + 30, 52, 16), Green,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);

            // Giá vé: viên thuốc có viền
            string price = it.WalkOnly ? "Miễn phí" : (it.FareVnd / 1000.0).ToString("0.#") + "k VNĐ";
            int pw = TextRenderer.MeasureText(price, InfoFont, new Size(1000, 100), TextFormatFlags.NoPadding).Width + 22;
            var pill = new Rectangle(timeX - 10 - pw, card.Y + 10, pw, 28);
            using (GraphicsPath pp = RoundPath(pill, 14))
            using (var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f))
                g.DrawPath(pen, pp);
            TextRenderer.DrawText(g, price, InfoFont, pill, MapCanvas.RouteColor,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            // Các tuyến cần đi: ô màu có số tuyến, cách nhau bằng chấm tròn
            var rides = new List<TransitLeg>();
            foreach (TransitLeg leg in it.Legs) if (!leg.Walk) rides.Add(leg);

            int x = card.X + 12, y = card.Y + 10, limit = pill.X - 10;
            if (rides.Count == 0) DrawChip(g, new Rectangle(x, y, Math.Max(70, ChipWidth("Đi bộ")), 26), Muted, "Đi bộ");
            for (int i = 0; i < rides.Count; i++)
            {
                int cw = ChipWidth(rides[i].Line.Ref);
                if (i > 0)
                {
                    using (var dot = new SolidBrush(Color.FromArgb(148, 163, 184))) g.FillEllipse(dot, x + 3, y + 10, 7, 7);
                    x += 16;
                }
                if (x + cw > limit)
                {
                    TextRenderer.DrawText(g, "…", ChipFont, new Rectangle(x, y, 20, 26), Muted, TextFormatFlags.NoPadding);
                    break;
                }
                DrawChip(g, new Rectangle(x, y, cw, 26), rides[i].Line.Color, rides[i].Line.Ref);
                x += cw + 6;
            }

            // Mỗi chuyến một ô, xếp cạnh nhau
            int by = card.Y + 46, bh = 72, gap = 16, inner = card.Width - 24;
            int n = Math.Max(1, rides.Count);
            int bw = (inner - gap * (n - 1)) / n;
            if (rides.Count == 0)
            {
                var wb = new Rectangle(card.X + 12, by, inner, bh);
                using (GraphicsPath bp = RoundPath(wb, 8))
                using (var bb = new SolidBrush(Color.FromArgb(241, 245, 249)))
                    g.FillPath(bb, bp);
                using (var bar = new SolidBrush(Muted)) g.FillRectangle(bar, wb.X, wb.Y + 8, 4, wb.Height - 16);
                var wr = new Rectangle(wb.X + 12, wb.Y + 4, wb.Width - 16, 17);
                const TextFormatFlags wf = TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;
                TextRenderer.DrawText(g, "Đi bộ thẳng tới điểm đến, không cần xe buýt", SmallBoldFont, wr, Navy, wf);
                wr.Y += 17;
                TextRenderer.DrawText(g, "Từ: " + StopName(it.Legs[0].FromStop), SmallFont, wr, Slate, wf);
                wr.Y += 16;
                TextRenderer.DrawText(g, "Tới: " + StopName(it.Legs[0].ToStop), SmallFont, wr, Slate, wf);
                wr.Y += 16;
                TextRenderer.DrawText(g, "Đi theo đường thật, tránh các đoạn đường bị chặn", SmallFont, wr, Muted, wf);
            }
            for (int i = 0; i < rides.Count; i++)
            {
                TransitLeg r = rides[i];
                var blk = new Rectangle(card.X + 12 + i * (bw + gap), by, bw, bh);
                using (GraphicsPath bp = RoundPath(blk, 8))
                using (var bb = new SolidBrush(Color.FromArgb(241, 245, 249)))
                    g.FillPath(bb, bp);
                using (var bar = new SolidBrush(r.Line.Color))
                    g.FillRectangle(bar, blk.X, blk.Y + 8, 4, blk.Height - 16);
                if (i > 0)
                    TextRenderer.DrawText(g, "›", BigTimeFont, new Rectangle(blk.X - gap, by, gap, bh), Muted,
                                          TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                var tr = new Rectangle(blk.X + 12, blk.Y + 4, blk.Width - 16, 17);
                const TextFormatFlags fl = TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;
                TextRenderer.DrawText(g, "Chuyến " + (i + 1) + "  ·  Tuyến " + r.Line.Ref, SmallBoldFont, tr, Navy, fl);
                tr.Y += 17;
                TextRenderer.DrawText(g, "Lên: " + StopName(r.FromStop), SmallFont, tr, Slate, fl);
                tr.Y += 16;
                TextRenderer.DrawText(g, "Xuống: " + StopName(r.ToStop), SmallFont, tr, Slate, fl);
                tr.Y += 16;
                TextRenderer.DrawText(g, r.StopCount + " trạm · " + FormatDistance(r.Meters) + " · chờ ~" + Math.Max(1, r.Line.HeadwayMin / 2) + "p", SmallFont, tr, Muted, fl);
            }

            // Tổng đi bộ | tổng đi xe
            int ry = by + bh + 10;
            DrawWalkIcon(g, new Rectangle(card.X + 12, ry, 16, 18), Muted);
            string walk = Math.Round(it.WalkMeters) + " m";
            TextRenderer.DrawText(g, walk, InfoFont, new Point(card.X + 32, ry), Muted, TextFormatFlags.NoPadding);
            if (it.Rides == 0) return;
            int sepX = card.X + 32 + TextRenderer.MeasureText(walk, InfoFont, new Size(1000, 100), TextFormatFlags.NoPadding).Width + 14;
            using (var sep = new Pen(Color.FromArgb(226, 232, 240), 1.5f)) g.DrawLine(sep, sepX, ry + 1, sepX, ry + 17);
            DrawBusIcon(g, new Rectangle(sepX + 14, ry + 1, 16, 16), Muted, Color.White);
            TextRenderer.DrawText(g, FormatDistance(it.RideMeters), InfoFont, new Point(sepX + 35, ry), Muted, TextFormatFlags.NoPadding);
            if (it.Rides > 1)
                TextRenderer.DrawText(g, "đổi tuyến " + it.Transfers + " lần", InfoFont, new Point(card.Right - 130, ry), Muted, TextFormatFlags.NoPadding);
        }

        private void lstSteps_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= steps.Count) return;
            Graphics g = e.Graphics;
            StepItem s = steps[e.Index];
            using (var bg = new SolidBrush(PanelBack)) g.FillRectangle(bg, e.Bounds);
            using (var bar = new SolidBrush(s.Color)) g.FillRectangle(bar, e.Bounds.X + 14, e.Bounds.Y + 6, 5, e.Bounds.Height - 12);
            TextRenderer.DrawText(g, s.Title, ListNameFont, new Rectangle(e.Bounds.X + 30, e.Bounds.Y + 5, e.Bounds.Width - 40, 20), Navy,
                                  TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            if (s.Sub.Length > 0)
                TextRenderer.DrawText(g, s.Sub, ListSubFont, new Rectangle(e.Bounds.X + 30, e.Bounds.Y + 25, e.Bounds.Width - 40, 18), Muted,
                                      TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void CopyItinerary()
        {
            int i = lstItins.SelectedIndex;
            if (i < 0 || i >= itins.Count)
            {
                lblInfo.Text = "Chưa có cách đi để sao chép.";
                return;
            }
            Itinerary it = itins[i];
            var sb = new StringBuilder();
            sb.AppendLine("Đi xe buýt: " + StopName(mapCanvas.StartStop) + " → " + StopName(mapCanvas.EndStop));
            sb.AppendLine("Khoảng " + Minutes(it.Seconds) + " phút, " + it.Rides + " chuyến, đi bộ " + Math.Round(it.WalkMeters) + " m (ước tính).");
            foreach (StepItem s in steps) sb.AppendLine("- " + s.Title + (s.Sub.Length > 0 ? " (" + s.Sub + ")" : ""));
            try
            {
                Clipboard.SetText(sb.ToString());
                lblInfo.Text = "Đã sao chép cách đi vào clipboard.";
            }
            catch (ExternalException)
            {
                lblInfo.Text = "Không sao chép được (clipboard đang bị chương trình khác giữ). Thử lại sau.";
            }
        }

        // =====================================================================
        //  QUYỀN ADMIN
        // =====================================================================
        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (graph == null || busy) return;
            if (adminForm != null && !adminForm.IsDisposed)
            {
                adminForm.Show();
                adminForm.Activate();
                return;
            }
            if (!adminLoggedIn) return;      // chỉ tài khoản quản trị mới thấy nút này
            adminForm = new AdminForm(this);
            adminForm.Show(this);
        }

        public RoadGraph Graph { get { return graph; } }
        public List<BusLine> Lines { get { return lines; } }
        public string AreaName { get { return Areas[cboArea.SelectedIndex].Name; } }
        public string DataDir { get { return Path.Combine(DataRoot, Areas[cboArea.SelectedIndex].Key); } }
        public bool HasAdminLines { get { return File.Exists(Path.Combine(DataDir, "lines_admin.csv")); } }

        public void RequestMapPick(string prompt, Action<double, double> onPicked, Action onCancel)
        {
            pendingPick = onPicked;
            pendingCancel = onCancel;
            lblInfo.Text = prompt + "  (Esc để hủy)";
            Activate();
        }

        public void PreviewLine(BusLine line)
        {
            int i = lines.IndexOf(line);
            if (i < 0) return;
            SetMode(Mode.Lines);
            lstLines.SelectedIndex = i + 1;
        }

        public void ApplyChanges() { var ignored = ReloadCurrent(); }
        public void ReloadFromDisk() { var ignored = ReloadCurrent(); }

        public void RestoreOriginalLines()
        {
            try { File.Delete(Path.Combine(DataDir, "lines_admin.csv")); }
            catch (Exception ex) { lblInfo.Text = "Không xóa được file chỉnh sửa: " + ex.Message; return; }
            var ignored = ReloadCurrent();
        }

        public void DownloadRealLines()
        {
            if (!busy) { var ignored = DownloadLines(Areas[cboArea.SelectedIndex]); }
        }

        public void Logout()
        {
            account = null;
            ApplyAccount();
            lblInfo.Text = "Đã đăng xuất quyền quản trị.";
        }

        // Nạp lại khu vực đang mở từ file rồi báo cho màn hình quản trị
        private async Task ReloadCurrent()
        {
            await LoadArea(cboArea.SelectedIndex, false);
            EventHandler h = DataReloaded;
            if (h != null) h(this, EventArgs.Empty);
        }

        // =====================================================================
        //  ĐỊA ĐIỂM (bệnh viện, trường học...)
        // =====================================================================
        private void LoadPois(Area a)
        {
            try
            {
                pois = PoiStore.LoadFor(DataRoot, Path.Combine(DataRoot, a.Key), a.South - 0.002, a.West - 0.002, a.North + 0.002, a.East + 0.002);
            }
            catch { pois = new List<Poi>(); }
            mapCanvas.Pois = pois;
            mapCanvas.ShowPois = chkPoi.Checked;
            if (poiForm != null && !poiForm.IsDisposed) poiForm.SetPois(pois);
        }

        private void chkPoi_CheckedChanged(object sender, EventArgs e)
        {
            mapCanvas.ShowPois = chkPoi.Checked;
            mapCanvas.Invalidate();
        }

        private async void btnLoadPoi_Click(object sender, EventArgs e)
        {
            if (graph == null || busy || IsOfflineArea()) return;
            Area a = Areas[cboArea.SelectedIndex];
            RoadGraph g0 = graph;
            btnLoadPoi.Enabled = false;
            lblInfo.Text = "Đang tải bệnh viện, trường học, chợ, công viên... của " + a.Name + " từ OpenStreetMap...";
            int count = 0;
            string err = null;
            SetProgress(-1, "Tải địa điểm từ OpenStreetMap...");
            try { count = await Task.Run(() => PoiStore.Download(a.South, a.West, a.North, a.East, Path.Combine(DataRoot, a.Key), Report)); }
            catch (Exception ex) { err = ex.Message; }
            FinishProgress(err == null ? "Xong" : "Lỗi tải");
            btnLoadPoi.Enabled = !busy;
            if (graph != g0) return;
            if (err != null) { lblInfo.Text = "Không tải được địa điểm: " + err; return; }
            LoadPois(a);
            chkPoi.Checked = true;
            lblInfo.Text = count == 0 ? "OpenStreetMap chưa có địa điểm nào có tên ở " + a.Name + "." : "Đã nạp " + count + " địa điểm. Bấm vào biểu tượng để chọn trạm gần đó.";
        }

        // Bấm vào biểu tượng địa điểm: chọn trạm gần nhất làm điểm đi / đến
        private void MapCanvas_PoiClicked(Poi p)
        {
            if (graph == null || busy) return;
            int st = graph.NearestStop(p.Lat, p.Lng);
            if (st < 0) { lblInfo.Text = p.Name + ": khu vực này chưa có trạm nào."; return; }
            double d = Meters(p.Lat, p.Lng, graph.Stops[st].Lat, graph.Stops[st].Lng);
            string near = "Gần " + Poi.KindName(p.Kind).ToLowerInvariant() + " " + p.Name + ": trạm " + graph.Stops[st] + " (cách " + FormatDistance(d) + ").";
            PickPoint(st, graph.Stops[st].Node);
            if (!pickingStart) lblInfo.Text = near + " Đã chọn làm điểm đi (A). Bấm điểm đến (B).";
            else lblInfo.Text = near;
        }

        // =====================================================================
        //  VỊ TRÍ CỦA TÔI
        // =====================================================================
        private async void btnLocate_Click(object sender, EventArgs e)
        {
            if (busy) return;
            btnLocate.Enabled = false;
            lblInfo.Text = "Đang xác định vị trí của bạn...";
            MyLocation loc = await LocationService.GetAsync();
            btnLocate.Enabled = !busy;

            if (loc == null)
            {
                lblInfo.Text = "Không xác định được vị trí. Hãy bật dịch vụ định vị của Windows (Settings > Privacy > Location) hoặc kiểm tra kết nối Internet.";
                return;
            }
            mapCanvas.HasMyLocation = true;
            mapCanvas.MyLat = loc.Lat;
            mapCanvas.MyLng = loc.Lng;
            mapCanvas.MyLocationApprox = loc.Approximate;
            string src = "theo " + loc.Source + (loc.Approximate ? ", chỉ gần đúng" : "");

            int st = graph != null ? graph.NearestStop(loc.Lat, loc.Lng) : -1;
            double d = st >= 0 ? Meters(loc.Lat, loc.Lng, graph.Stops[st].Lat, graph.Stops[st].Lng) : double.MaxValue;
            if (st < 0 || d > 3000)
            {
                mapCanvas.Invalidate();
                lblInfo.Text = "Vị trí của bạn (" + src + ") nằm ngoài khu vực bản đồ đang mở" +
                               (st >= 0 ? " (trạm gần nhất cách " + FormatDistance(d) + ")" : "") + ". Hãy chọn khu vực khác ở ô trên cùng.";
                return;
            }

            ResetAnimation();
            SetPoint(true, st, graph.Stops[st].Node);
            pickingStart = false;
            mapCanvas.Position = new PointLatLng(loc.Lat, loc.Lng);
            mapCanvas.Invalidate();
            lblInfo.Text = "Vị trí của bạn (" + src + "): trạm gần nhất là " + graph.Stops[st] + ", cách " + FormatDistance(d) + ". Đã đặt làm điểm đi (A).";
            if (HasValidEndpoints()) RunSearch();
        }

        // =====================================================================
        //  TÀI KHOẢN: HÀNH KHÁCH / TÀI XẾ
        // =====================================================================
        private void btnAccount_Click(object sender, EventArgs e)
        {
            if (account == null)
            {
                using (var dlg = new AuthForm(DataRoot))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK || dlg.Account == null) return;
                    account = dlg.Account;
                }
                ApplyAccount();
                lblInfo.Text = "Xin chào " + account.Name + " (" + account.RoleText + ")." +
                               (account.Role == UserRole.Admin ? " Nút \"Quản trị\" đã hiện ở thanh trên để sửa trạm và tuyến."
                                : account.Role == UserRole.Driver ? " Mở tab \"Tài xế\" để bắt đầu ca."
                                : " Bấm \"☆ Lưu\" để lưu điểm đến vào yêu thích.");
            }
            else
            {
                if (onShift && MessageBox.Show("Bạn đang chạy ca. Đăng xuất sẽ kết thúc ca. Tiếp tục?", "Đăng xuất",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                if (adminForm != null && !adminForm.IsDisposed) adminForm.Close();
                account = null;
                ApplyAccount();
                lblInfo.Text = "Đã đăng xuất.";
            }
        }

        // Hiện / ẩn các chức năng theo vai trò
        private void ApplyAccount()
        {
            bool driver = account != null && account.Role == UserRole.Driver;
            bool passenger = account != null && account.Role == UserRole.Passenger;
            bool admin = account != null && account.Role == UserRole.Admin;
            adminLoggedIn = admin;
            btnAdmin.Visible = admin;

            btnAccount.Text = account == null ? "Đăng nhập" : "Đăng xuất";
            btnAccount.BackColor = account == null ? Green : Slate;
            lblAccount.Text = account == null ? "Chưa đăng nhập (dùng với tư cách khách)"
                                              : "Xin chào, " + account.Name + "  ·  " + account.RoleText;

            btnTabDriver.Visible = driver;
            cboFav.Visible = btnFavAdd.Visible = passenger;
            if (!driver)
            {
                if (onShift) StopShift("Đã kết thúc ca.");
                if (mode == Mode.Driver) SetMode(Mode.Bus);
            }

            favorites = passenger ? UserStore.LoadFavorites(DataRoot, account.Name) : new List<FavPlace>();
            RefreshFavCombo();
        }

        // ----- Yêu thích (hành khách) -----
        private void RefreshFavCombo()
        {
            cboFav.Items.Clear();
            cboFav.Items.Add("★ Yêu thích...");
            foreach (FavPlace f in favorites) cboFav.Items.Add(f);
            cboFav.SelectedIndex = 0;
        }

        private void btnFavAdd_Click(object sender, EventArgs e)
        {
            if (account == null || graph == null) return;
            int st = mapCanvas.EndStop >= 0 ? mapCanvas.EndStop : mapCanvas.StartStop;
            if (st < 0 || st >= graph.Stops.Count) { lblInfo.Text = "Hãy chọn một trạm (điểm đến B) trước khi lưu."; return; }
            BusStop bs = graph.Stops[st];

            FavPlace existing = null;
            foreach (FavPlace f in favorites)
                if (Meters(f.Lat, f.Lng, bs.Lat, bs.Lng) < 30) { existing = f; break; }

            if (existing != null)
            {
                if (MessageBox.Show("Bỏ \"" + existing.Name + "\" khỏi danh sách yêu thích?", "Yêu thích",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                favorites.Remove(existing);
                lblInfo.Text = "Đã bỏ \"" + existing.Name + "\" khỏi yêu thích.";
            }
            else
            {
                favorites.Add(new FavPlace { Name = bs.ToString(), Lat = bs.Lat, Lng = bs.Lng });
                lblInfo.Text = "Đã lưu \"" + bs + "\" vào yêu thích.";
            }
            UserStore.SaveFavorites(DataRoot, account.Name, favorites);
            RefreshFavCombo();
        }

        private void cboFav_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int idx = cboFav.SelectedIndex;
            if (idx <= 0 || idx > favorites.Count || graph == null || busy) return;
            FavPlace f = favorites[idx - 1];
            int st = graph.NearestStop(f.Lat, f.Lng);
            if (st < 0 || Meters(f.Lat, f.Lng, graph.Stops[st].Lat, graph.Stops[st].Lng) > 80)
                lblInfo.Text = "\"" + f.Name + "\" nằm ngoài khu vực bản đồ đang mở.";
            else
                PickPoint(st, graph.Stops[st].Node);
            cboFav.SelectedIndex = 0;
        }

        // ----- Ca làm việc của tài xế -----
        private void btnTabDriver_Click(object sender, EventArgs e) { SetMode(Mode.Driver); }

        private void FillDriverLines()
        {
            if (onShift) StopShift("Dữ liệu tuyến đã đổi nên ca được kết thúc.");
            cboDriverLine.Items.Clear();
            foreach (BusLine l in lines)
                cboDriverLine.Items.Add("Tuyến " + l.Ref + "  ·  " + (graph != null ? l.Title(graph) : l.Name));
            if (cboDriverLine.Items.Count > 0) cboDriverLine.SelectedIndex = 0;
            lstDriverStops.Items.Clear();
        }

        private void btnShift_Click(object sender, EventArgs e)
        {
            if (onShift) { StopShift("Đã kết thúc ca."); return; }
            int idx = cboDriverLine.SelectedIndex;
            if (graph == null || idx < 0 || idx >= lines.Count) { lblDriverInfo.Text = "Chưa có tuyến nào để chạy."; return; }
            BusLine l = lines[idx];
            if (l.Stops.Count < 2 || l.Length < 50) { lblDriverInfo.Text = "Tuyến này chưa đủ dữ liệu để chạy."; return; }

            driverLine = l;
            driverMeters = 0;
            driverNext = 1;
            driverDwell = DriverDwellSeconds;
            onShift = true;
            driverListNext = -2;
            btnShift.Text = "■ Kết thúc ca";
            btnShift.BackColor = Color.FromArgb(220, 38, 38);
            cboDriverLine.Enabled = false;

            lstDriverStops.Items.Clear();
            for (int i = 0; i < l.Stops.Count; i++) lstDriverStops.Items.Add("");
            mapCanvas.HasDriver = true;
            mapCanvas.DriverText = "Tuyến " + l.Ref;
            LatLng p0 = l.Path.PointAt(0);
            mapCanvas.DriverLat = p0.Lat;
            mapCanvas.DriverLng = p0.Lng;
            FitToPoints(l.Path.Points);
            ApplyView();
            UpdateDriverUi();
            driverTimer.Start();
        }

        private void StopShift(string message)
        {
            onShift = false;
            driverTimer.Stop();
            driverLine = null;
            mapCanvas.HasDriver = false;
            btnShift.Text = "▶ Bắt đầu ca";
            btnShift.BackColor = Green;
            cboDriverLine.Enabled = true;
            lblDriverInfo.Text = message;
            lstDriverStops.Items.Clear();
            ApplyView();
        }

        private void DriverTimer_Tick(object sender, EventArgs e)
        {
            if (!onShift || driverLine == null) return;
            if (driverDwell > 0)
            {
                driverDwell -= driverTimer.Interval / 1000.0;
                if (driverDwell < 0) driverDwell = 0;
                UpdateDriverUi();
                return;
            }

            driverMeters += DriverSpeed * driverTimer.Interval / 1000.0;
            if (driverNext < driverLine.Stops.Count && driverMeters >= driverLine.StopDist[driverNext])
            {
                driverMeters = driverLine.StopDist[driverNext];
                driverNext++;
                if (driverNext >= driverLine.Stops.Count)
                {
                    string last = graph.Stops[driverLine.Stops[driverLine.Stops.Count - 1]].ToString();
                    StopShift("Đã đến bến cuối: " + last + ". Chuyến kết thúc.");
                    return;
                }
                driverDwell = DriverDwellSeconds;
            }

            LatLng p = driverLine.Path.PointAt(driverMeters);
            mapCanvas.DriverLat = p.Lat;
            mapCanvas.DriverLng = p.Lng;
            UpdateDriverUi();
            mapCanvas.Invalidate();
        }

        // Cập nhật dòng trạng thái và danh sách trạm của ca (✓ đã qua, ▶ trạm sắp tới)
        private void UpdateDriverUi()
        {
            if (driverLine == null || graph == null) return;
            int n = driverLine.Stops.Count;
            int next = Math.Min(driverNext, n - 1);
            string nextName = graph.Stops[driverLine.Stops[next]].ToString();
            double toNext = Math.Max(0, driverLine.StopDist[next] - driverMeters);

            string head = driverDwell > 0
                ? "Đang dừng tại: " + graph.Stops[driverLine.Stops[Math.Max(0, driverNext - 1)]] + "  (còn " + Math.Ceiling(driverDwell) + " giây)"
                : "Đang chạy tới: " + nextName + "  (còn " + FormatDistance(toNext) + ")";
            lblDriverInfo.Text = head + "\r\nĐã chạy " + FormatDistance(driverMeters) + " / " + FormatDistance(driverLine.Length) +
                                 "  ·  trạm " + Math.Min(driverNext, n) + "/" + n;

            if (driverListNext != driverNext)
            {
                driverListNext = driverNext;
                for (int i = 0; i < n && i < lstDriverStops.Items.Count; i++)
                {
                    string mark = i < driverNext ? "✓   " : (i == driverNext ? "▶   " : "      ");
                    lstDriverStops.Items[i] = mark + graph.Stops[driverLine.Stops[i]];
                }
                if (driverNext < lstDriverStops.Items.Count) lstDriverStops.TopIndex = Math.Max(0, driverNext - 3);
            }
        }

        // =====================================================================
        //  DANH SÁCH CÁC TUYẾN
        // =====================================================================
        private void lstLines_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstLines.Items.Count) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            bool selected = (e.State & DrawItemState.Selected) != 0;

            using (var bg = new SolidBrush(ListBack)) g.FillRectangle(bg, e.Bounds);
            var card = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 4, e.Bounds.Width - 20, e.Bounds.Height - 8);
            using (GraphicsPath path = RoundPath(card, 10))
            {
                g.FillPath(Brushes.White, path);
                using (var border = new Pen(selected ? MapCanvas.RouteColor : Color.FromArgb(226, 232, 240), selected ? 2.2f : 1f))
                    g.DrawPath(border, path);
            }

            var textRect = new Rectangle(card.X + 100, card.Y + 9, card.Width - 112, 20);
            var subRect = new Rectangle(card.X + 100, card.Y + 31, card.Width - 112, 18);
            if (e.Index == 0)
            {
                TextRenderer.DrawText(g, "Tất cả các tuyến", ListNameFont, textRect, Navy, TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                TextRenderer.DrawText(g, lines.Count + " lượt đi / về, hiện cùng lúc trên bản đồ", ListSubFont, subRect, Muted, TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                return;
            }

            BusLine l = lines[e.Index - 1];
            DrawChip(g, new Rectangle(card.X + 12, card.Y + 14, Math.Max(70, ChipWidth(l.Ref)), 26), l.Color, l.Ref);
            TextRenderer.DrawText(g, l.Title(graph), ListNameFont, textRect, Navy, TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            TextRenderer.DrawText(g, l.Stops.Count + " trạm  ·  " + FormatDistance(l.Length) + "  ·  " + (l.Fare / 1000.0).ToString("0.#") + "k  ·  " + l.HeadwayMin + " phút/chuyến" + (l.Demo ? "  ·  demo" : ""), ListSubFont, subRect, Muted,
                                  TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void lstLines_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lstLines.SelectedIndex;
            selectedLine = (i >= 1 && i - 1 < lines.Count) ? lines[i - 1] : null;
            ApplyView();
            if (selectedLine != null && mode == Mode.Lines)
            {
                FitToPoints(selectedLine.Path.Points);
                lblInfo.Text = "Tuyến " + selectedLine.Ref + ": " + selectedLine.Title(graph) + "  ·  " + selectedLine.Stops.Count + " trạm, " +
                               FormatDistance(selectedLine.Length) + ". Các xe trên bản đồ chạy mô phỏng, không phải vị trí xe thật.";
            }
        }

        // =====================================================================
        //  VẼ THẺ, Ô SỐ TUYẾN, ICON
        // =====================================================================
        private static GraphicsPath RoundPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static int ChipWidth(string text)
        {
            return TextRenderer.MeasureText(text, ChipFont, new Size(1000, 100), TextFormatFlags.NoPadding).Width + 40;
        }

        // Ô số tuyến: nền màu, icon xe buýt trắng và số tuyến
        private static void DrawChip(Graphics g, Rectangle r, Color color, string text)
        {
            using (GraphicsPath p = RoundPath(r, 7))
            using (var fill = new SolidBrush(color))
                g.FillPath(fill, p);
            DrawBusIcon(g, new Rectangle(r.X + 9, r.Y + 5, 16, 16), Color.White, color);
            TextRenderer.DrawText(g, text, ChipFont, new Rectangle(r.X + 29, r.Y, r.Width - 31, r.Height), Color.White,
                                  TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        // Icon xe buýt: thân xe, kính, hai bánh
        private static void DrawBusIcon(Graphics g, Rectangle r, Color fg, Color bg)
        {
            var body = new Rectangle(r.X + 1, r.Y, r.Width - 2, r.Height - 3);
            using (GraphicsPath p = RoundPath(body, 3))
            using (var b = new SolidBrush(fg))
                g.FillPath(b, p);
            using (var w = new SolidBrush(bg))
            {
                g.FillRectangle(w, r.X + 3, r.Y + 2, r.Width - 6, 5);
                g.FillEllipse(w, r.X + 3, r.Y + r.Height - 8, 3, 3);
                g.FillEllipse(w, r.Right - 6, r.Y + r.Height - 8, 3, 3);
            }
            using (var b = new SolidBrush(fg))
            {
                g.FillRectangle(b, r.X + 3, r.Bottom - 3, 3, 3);
                g.FillRectangle(b, r.Right - 6, r.Bottom - 3, 3, 3);
            }
        }

        // Icon người đi bộ
        private static void DrawWalkIcon(Graphics g, Rectangle r, Color c)
        {
            using (var pen = new Pen(c, 1.8f))
            using (var b = new SolidBrush(c))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                float cx = r.X + r.Width / 2f;
                g.FillEllipse(b, cx - 2.5f, r.Y, 5, 5);
                g.DrawLine(pen, cx, r.Y + 6, cx, r.Y + 11);                 // thân
                g.DrawLine(pen, cx, r.Y + 7, cx - 4, r.Y + 10);             // tay
                g.DrawLine(pen, cx, r.Y + 7, cx + 4, r.Y + 9);
                g.DrawLine(pen, cx, r.Y + 11, cx - 3, r.Bottom);            // chân
                g.DrawLine(pen, cx, r.Y + 11, cx + 3, r.Bottom);
            }
        }

        // =====================================================================
        //  XE BUÝT CHẠY THỬ DỌC ĐƯỜNG NGẮN NHẤT
        // =====================================================================
        private void btnBus_Click(object sender, EventArgs e)
        {
            if (busTimer.Enabled) { StopBus(); return; }
            if (route == null || route.Total <= 0 || phase != Phase.Idle)
            {
                lblInfo.Text = "Hãy tìm đường trước, rồi bấm \"Chạy thử\".";
                return;
            }

            // Chạy nhanh hơn thật nhiều lần để xem trong vài chục giây
            double seconds = Math.Max(6.0, Math.Min(25.0, route.Total / 100.0));
            busSpeed = route.Total / seconds;
            busMeters = 0;
            mapCanvas.BusMeters = 0;
            btnBus.Text = "■ Dừng";
            busTimer.Start();
        }

        private void StopBus()
        {
            busTimer.Stop();
            mapCanvas.BusMeters = -1;
            btnBus.Text = "► Chạy thử";
            mapCanvas.Invalidate();
        }

        private void BusTimer_Tick(object sender, EventArgs e)
        {
            if (route == null) { StopBus(); return; }
            busMeters += busSpeed * busTimer.Interval / 1000.0;
            if (busMeters >= route.Total)
            {
                StopBus();
                lblInfo.Text = "Xe đã tới điểm đến (B).";
                return;
            }
            mapCanvas.BusMeters = busMeters;

            // Trạm tiếp theo xe sẽ dừng
            int next = -1;
            for (int i = 0; i < mapCanvas.RouteStopDist.Count; i++)
                if (mapCanvas.RouteStopDist[i] > busMeters) { next = i; break; }
            if (next >= 0)
            {
                double left = mapCanvas.RouteStopDist[next] - busMeters;
                lblInfo.Text = "Xe đang chạy  ·  trạm tiếp theo: " + graph.Stops[mapCanvas.RouteStops[next]].ToString() +
                               " (còn " + FormatDistance(left) + ")";
            }

            // Xe ra khỏi màn hình thì dời bản đồ theo xe
            LatLng p = route.PointAt(busMeters);
            if (!MapCanvas.InView(p.Lat, p.Lng, mapCanvas.ViewArea)) mapCanvas.Position = new PointLatLng(p.Lat, p.Lng);
            mapCanvas.Invalidate();
        }

        // =====================================================================
        //  DANH SÁCH TRẠM CỦA ĐƯỜNG NGẮN NHẤT
        // =====================================================================
        private void lstStops_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= listStopIndex.Count || e.Index >= lstStops.Items.Count) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            bool selected = (e.State & DrawItemState.Selected) != 0;
            int n = listStopIndex.Count;
            bool isA = e.Index == 0, isB = e.Index == n - 1;

            using (var bg = new SolidBrush(selected ? Color.FromArgb(219, 234, 254) : PanelBack))
                g.FillRectangle(bg, e.Bounds);

            // Đường dọc nối các trạm (kiểu dòng thời gian) và chấm tròn đánh số
            int cx = e.Bounds.X + 30, cy = e.Bounds.Y + e.Bounds.Height / 2;
            using (var pen = new Pen(Color.FromArgb(191, 219, 254), 3f))
            {
                if (e.Index > 0) g.DrawLine(pen, cx, e.Bounds.Y, cx, cy);
                if (e.Index < n - 1) g.DrawLine(pen, cx, cy, cx, e.Bounds.Bottom);
            }
            Color c = isA ? MapCanvas.StartColor : (isB ? MapCanvas.EndColor : MapCanvas.RouteColor);
            using (var fill = new SolidBrush(c))
                g.FillEllipse(fill, cx - 10, cy - 10, 20, 20);
            string mark = isA ? "A" : (isB ? "B" : (e.Index + 1).ToString());
            TextRenderer.DrawText(g, mark, ListMarkFont,
                                  new Rectangle(cx - 10, cy - 10, 20, 20), Color.White,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            var nameRect = new Rectangle(e.Bounds.X + 52, e.Bounds.Y + 6, e.Bounds.Width - 60, 20);
            var subRect = new Rectangle(e.Bounds.X + 52, e.Bounds.Y + 25, e.Bounds.Width - 60, 16);
            TextRenderer.DrawText(g, (string)lstStops.Items[e.Index], ListNameFont, nameRect, Navy,
                                  TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            if (e.Index < listSub.Count)
                TextRenderer.DrawText(g, listSub[e.Index], ListSubFont, subRect, Muted,
                                      TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lstStops.SelectedIndex;
            if (i < 0 || i >= listStopIndex.Count || graph == null) return;
            BusStop s = graph.Stops[listStopIndex[i]];
            mapCanvas.HighlightStop = listStopIndex[i];
            mapCanvas.Position = new PointLatLng(s.Lat, s.Lng);
            if (mapCanvas.Zoom < 17) mapCanvas.Zoom = 17;
            mapCanvas.Invalidate();
        }

        // Chép lộ trình dạng chữ vào clipboard (để dán vào tin nhắn, ghi chú...)
        private void CopyRoute()
        {
            if (result == null || !result.Found || listStopIndex.Count == 0)
            {
                lblInfo.Text = "Chưa có lộ trình để sao chép.";
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine("Lộ trình: " + graph.Stops[listStopIndex[0]].ToString() + " → " + graph.Stops[listStopIndex[listStopIndex.Count - 1]].ToString());
            sb.AppendLine("Quãng đường " + lblDist.Text + ", khoảng " + lblTime.Text + ", đi qua " + listStopIndex.Count + " trạm.");
            for (int i = 0; i < listStopIndex.Count; i++)
                sb.AppendLine((i + 1) + ". " + graph.Stops[listStopIndex[i]].ToString() + " (" + listSub[i] + ")");
            try
            {
                Clipboard.SetText(sb.ToString());
                lblInfo.Text = "Đã sao chép lộ trình vào clipboard.";
            }
            catch (ExternalException)
            {
                lblInfo.Text = "Không sao chép được (clipboard đang bị chương trình khác giữ). Thử lại sau.";
            }
        }

        // =====================================================================
        //  PHÓNG BẢN ĐỒ
        // =====================================================================
        private void FitToRoute()
        {
            FitToPoints(route.Points);
        }

        private void FitToLegs(List<MapLeg> legs)
        {
            var all = new List<LatLng>();
            foreach (MapLeg l in legs) all.AddRange(l.Points);
            FitToPoints(all);
        }

        // Phóng bản đồ để thấy cả dãy điểm
        private void FitToPoints(List<LatLng> pts)
        {
            if (pts == null || pts.Count == 0) return;
            double minLat = double.MaxValue, minLng = double.MaxValue, maxLat = double.MinValue, maxLng = double.MinValue;
            foreach (LatLng p in pts)
            {
                if (p.Lat < minLat) minLat = p.Lat;
                if (p.Lat > maxLat) maxLat = p.Lat;
                if (p.Lng < minLng) minLng = p.Lng;
                if (p.Lng > maxLng) maxLng = p.Lng;
            }
            double padLat = Math.Max(0.0012, (maxLat - minLat) * 0.15);
            double padLng = Math.Max(0.0012, (maxLng - minLng) * 0.15);
            mapCanvas.SetZoomToFitRect(new RectLatLng(maxLat + padLat, minLng - padLng,
                                                      (maxLng - minLng) + 2 * padLng, (maxLat - minLat) + 2 * padLat));
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
            return meters >= 1000 ? (meters / 1000).ToString("F1") + " km" : meters.ToString("F0") + " m";
        }
    }
}
