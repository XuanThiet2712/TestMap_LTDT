using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TestMap.Forms;
using TestMap.Routing;
using TestMap.Services;

namespace TestMap
{
    // Phần Form1 lo: (1) Bảng Dijkstra từng bước, (2) Chế độ demo tinh giản.
    public partial class Form1
    {
        private TableForm tableForm;
        private List<GraphModel> tableModels = new List<GraphModel>();
        private bool demoMode = true;
        private const int TableLimit = 32;              // bảng chỉ hợp với đồ thị nhỏ

        // =====================================================================
        //  CHẾ ĐỘ DEMO (TINH GIẢN): chỉ giữ những gì cần cho buổi trình bày
        // =====================================================================
        private void chkDemo_CheckedChanged(object sender, EventArgs e)
        {
            demoMode = chkDemo.Checked;
            RefreshChrome();
            lblInfo.Text = demoMode
                ? "Chế độ demo: đã ẩn đăng nhập, quản trị, tải dữ liệu, lọc địa điểm... Bỏ chọn \"Chế độ demo\" để dùng đầy đủ."
                : "Chế độ đầy đủ.";
        }

        // Đặt lại việc hiện / ẩn các nút theo chế độ demo và theo tài khoản đang đăng nhập
        private void RefreshChrome()
        {
            bool full = !demoMode;
            bool driver = account != null && account.Role == UserRole.Driver;
            bool passenger = account != null && account.Role == UserRole.Passenger;
            bool admin = account != null && account.Role == UserRole.Admin;

            // nhóm tài khoản
            btnAccount.Visible = lblAccount.Visible = full;
            btnAdmin.Visible = full && admin;
            btnTabDriver.Visible = full && driver;
            cboFav.Visible = btnFavAdd.Visible = full && passenger;
            // nhóm dữ liệu / địa điểm / định vị
            btnLoadLines.Visible = btnLoadPoi.Visible = btnReload.Visible = full;
            btnPoiFilter.Visible = chkPoi.Visible = chkBuses.Visible = full;
            btnLocate.Visible = full;

            if (!(full && driver) && mode == Mode.Driver) SetMode(Mode.Bus);
        }

        // =====================================================================
        //  BẢNG DIJKSTRA TỪNG BƯỚC
        // =====================================================================
        private void btnTable_Click(object sender, EventArgs e) { OpenTable(); }

        private void OpenTable()
        {
            if (graph == null || busy) return;
            tableModels = new List<GraphModel> { BuildRoadModel(), BuildBusModel() };

            // đang ở tab xe buýt thì mặc định lập bảng cho đồ thị xe buýt, ngược lại cho đồ thị đường
            int kind = (mode == Mode.Bus && tableModels[1].Error == null) ? 1 : 0;
            if (tableModels[kind].Error != null && tableModels[1 - kind].Error == null) kind = 1 - kind;

            if (tableForm != null && !tableForm.IsDisposed) tableForm.Close();
            tableForm = new TableForm(tableModels, kind, OnTableStep);
            tableForm.Show(this);
        }

        // Đồ thị ĐƯỜNG: đỉnh = giao lộ, trọng số = chiều dài đoạn đường (đơn vị 100 m)
        private GraphModel BuildRoadModel()
        {
            var m = new GraphModel { Name = "Đồ thị đường (đỉnh = giao lộ)", Unit = 100, UnitName = "100 m" };
            int n = graph.Nodes.Count;
            if (n > TableLimit)
            {
                m.Error = "Khu vực này có " + n + " giao lộ, quá nhiều để lập bảng (tối đa " + TableLimit + " đỉnh). " +
                          "Hãy chọn khu vực \"★ Demo 9 đỉnh\" hoặc \"★ Demo đơn giản\" ở ô trên cùng.";
                return m;
            }
            var nameAt = new Dictionary<int, string>();
            foreach (BusStop s in graph.Stops) if (!nameAt.ContainsKey(s.Node)) nameAt[s.Node] = s.ToString();

            m.MapIndex = new int[n];
            for (int i = 0; i < n; i++)
            {
                string nm;
                m.Labels.Add(i + (nameAt.TryGetValue(i, out nm) ? "   " + nm : ""));
                m.MapIndex[i] = i;
                var adj = new List<Link>();
                foreach (int ei in graph.Adj[i])
                {
                    if (graph.IsBlocked(ei)) continue;                    // đường bị chặn thì không có cạnh
                    adj.Add(new Link { To = graph.OtherEnd(ei, i), W = graph.Edges[ei].Length });
                }
                m.Adj.Add(adj);
            }
            m.DefaultSrc = mapCanvas.StartNode >= 0 && mapCanvas.StartNode < n ? mapCanvas.StartNode : 0;
            m.DefaultDst = mapCanvas.EndNode >= 0 && mapCanvas.EndNode < n ? mapCanvas.EndNode : n - 1;
            return m;
        }

        // Đồ thị XE BUÝT: đỉnh = trạm, cạnh = hai trạm liên tiếp trên một tuyến, trọng số = thời gian chạy (phút)
        private GraphModel BuildBusModel()
        {
            var m = new GraphModel { Name = "Đồ thị xe buýt (đỉnh = trạm)", Unit = 60, UnitName = "phút" };
            if (lines.Count == 0) { m.Error = "Khu vực này chưa có tuyến xe buýt để lập bảng."; return m; }

            var set = new SortedDictionary<int, int>();                  // chỉ số trạm -> chỉ số trong bảng
            foreach (BusLine l in lines) foreach (int s in l.Stops) set[s] = 0;
            if (set.Count > TableLimit)
            {
                m.Error = "Mạng xe buýt có " + set.Count + " trạm, quá nhiều để lập bảng (tối đa " + TableLimit + " đỉnh). " +
                          "Hãy chọn khu vực \"★ Demo 9 đỉnh\" hoặc \"★ Demo đơn giản\".";
                return m;
            }
            var stopOf = new List<int>();
            foreach (int s in new List<int>(set.Keys)) { set[s] = stopOf.Count; stopOf.Add(s); }

            for (int i = 0; i < stopOf.Count; i++)
            {
                m.Labels.Add(i + "   " + StopName(stopOf[i]));
                m.Adj.Add(new List<Link>());
            }
            m.MapIndex = stopOf.ToArray();

            // trọng số: quãng đường / tốc độ xe + thời gian dừng (giữ giá trị nhỏ nhất nếu nhiều tuyến cùng chạy giữa hai trạm)
            var best = new Dictionary<long, double>();
            foreach (BusLine l in lines)
                for (int i = 0; i + 1 < l.Stops.Count; i++)
                {
                    int a = set[l.Stops[i]], b = set[l.Stops[i + 1]];
                    double w = (l.StopDist[i + 1] - l.StopDist[i]) / TransitNet.BusSpeed + TransitNet.DwellSeconds;
                    long key = a * 1000L + b;
                    double old;
                    if (!best.TryGetValue(key, out old) || w < old) best[key] = w;
                }
            foreach (KeyValuePair<long, double> kv in best)
                m.Adj[(int)(kv.Key / 1000)].Add(new Link { To = (int)(kv.Key % 1000), W = kv.Value });

            int ss, ee;
            m.DefaultSrc = mapCanvas.StartStop >= 0 && set.TryGetValue(mapCanvas.StartStop, out ss) ? ss : 0;
            m.DefaultDst = mapCanvas.EndStop >= 0 && set.TryGetValue(mapCanvas.EndStop, out ee) ? ee : stopOf.Count - 1;
            return m;
        }

        // Bảng chạy tới bước nào thì bản đồ sáng các đỉnh đã chốt tới đó
        private void OnTableStep(int kind, HashSet<int> finalized, int current)
        {
            if (finalized == null)               // đóng bảng: xóa lớp vẽ
            {
                ClearSimOverlay();
                ApplyView();
                mapCanvas.Invalidate();
                return;
            }
            GraphModel m = tableModels[kind];
            var set = new HashSet<int>();
            foreach (int i in finalized) if (i < m.MapIndex.Length) set.Add(m.MapIndex[i]);
            int cur = current >= 0 && current < m.MapIndex.Length ? m.MapIndex[current] : -1;

            ClearSimOverlay();
            if (kind == 0) { mapCanvas.SimOpenNodes = set; mapCanvas.SimCurrentNode = cur; }
            else { mapCanvas.SimStops = set; mapCanvas.SimCurrentStop = cur; }
            ApplyView();
            mapCanvas.Invalidate();
        }
    }
}
