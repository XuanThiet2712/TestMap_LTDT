using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TestMap.Forms
{
    public class Link { public int To; public double W; }

    // Một đồ thị nhỏ để lập bảng Dijkstra: đỉnh có nhãn, cạnh có trọng số.
    public class GraphModel
    {
        public string Name = "";
        public string Error;                          // khác null: không lập bảng được (ví dụ đồ thị quá lớn)
        public double Unit = 1;                       // chia trọng số cho số này khi hiển thị (100 m, 60 giây...)
        public string UnitName = "";
        public List<string> Labels = new List<string>();      // "0  Bến Xe"
        public List<List<Link>> Adj = new List<List<Link>>();
        public int[] MapIndex = new int[0];           // chỉ số đỉnh trong bảng -> chỉ số thật trên bản đồ (giao lộ hoặc trạm)
        public int DefaultSrc, DefaultDst;
        public double[] Lat = new double[0], Lng = new double[0];    // tọa độ từng đỉnh (để vẽ đường chim bay h trên bản đồ)
        public Func<int, int, double> H;                              // ước lượng chi phí còn lại từ đỉnh a tới đỉnh b (null: không có A*)

        public string Fmt(double v)
        {
            return double.IsInfinity(v) ? "∞" : (v / Unit).ToString("0.0", CultureInfo.InvariantCulture);
        }
    }

    // Một dòng của bảng (trạng thái sau mỗi lần "chốt" một đỉnh), giống bảng trong sách giáo khoa:
    //   ô = (khoảng cách ngắn nhất hiện biết, đỉnh đứng trước);  "–" = đỉnh đã chốt;  in đậm = đỉnh được chọn chốt tiếp theo
    public class TableRow
    {
        public double[] Dist;
        public int[] Prev;
        public bool[] Final;
        public bool[] Changed;      // ô vừa được cập nhật trong bước này
        public int Relaxed = -1;    // đỉnh vừa chốt ở bước này (-1: dòng khởi tạo)
        public int Chosen = -1;     // đỉnh nhỏ nhất chưa chốt: sẽ được chốt ở bước kế tiếp
        public double[] F;          // chỉ với A*: f = g + h của từng đỉnh (Dist là g)
        public string Explain = "";
    }

    public class TableResult
    {
        public List<TableRow> Rows = new List<TableRow>();
        public List<int> Path = new List<int>();
        public double Total = double.PositiveInfinity;
        public double[] H;          // chỉ với A*: h của từng đỉnh tới đích
        public bool AStar;
        public int Expanded { get { return Math.Max(0, Rows.Count - 1); } }   // số đỉnh đã chốt
    }

    public static class DijkstraTable
    {
        public static TableResult Run(GraphModel m, int src, int dst)
        {
            int n = m.Labels.Count;
            var dist = new double[n];
            var prev = new int[n];
            var fin = new bool[n];
            for (int i = 0; i < n; i++) { dist[i] = double.PositiveInfinity; prev[i] = -1; }
            dist[src] = 0;

            var res = new TableResult();
            Func<string> unitText = () => m.UnitName.Length > 0 ? " " + m.UnitName : "";

            // Dòng 0: khởi tạo
            var r0 = Snapshot(dist, prev, fin, new bool[n]);
            r0.Chosen = src;
            r0.Explain = "Khởi tạo: khoảng cách từ đỉnh " + src + " tới chính nó = 0; mọi đỉnh khác = ∞ (chưa biết đường đi), đỉnh đứng trước = –.\r\n" +
                         "Chọn đỉnh có khoảng cách nhỏ nhất trong các đỉnh chưa chốt: đỉnh " + src + " (đang in đậm).";
            res.Rows.Add(r0);

            int u = src;
            int guard = 0;
            while (u >= 0 && guard++ < n + 2)
            {
                fin[u] = true;
                var changed = new bool[n];
                var sb = new StringBuilder();
                sb.Append("Chốt đỉnh ").Append(u).Append(" (khoảng cách ").Append(m.Fmt(dist[u])).Append(unitText()).Append("). Xét các đỉnh kề chưa chốt:");
                int considered = 0;
                foreach (Link l in m.Adj[u])
                {
                    int v = l.To;
                    if (fin[v]) continue;
                    considered++;
                    double nd = dist[u] + l.W;
                    sb.Append("\r\n  • đỉnh ").Append(v).Append(":  ").Append(m.Fmt(dist[u])).Append(" + ").Append(m.Fmt(l.W)).Append(" = ").Append(m.Fmt(nd));
                    if (nd < dist[v])
                    {
                        sb.Append("  <  ").Append(m.Fmt(dist[v])).Append("  →  cập nhật thành (").Append(m.Fmt(nd)).Append(", ").Append(u).Append(")");
                        dist[v] = nd; prev[v] = u; changed[v] = true;
                    }
                    else sb.Append("  ≥  ").Append(m.Fmt(dist[v])).Append("  →  giữ nguyên");
                }
                if (considered == 0) sb.Append("\r\n  (không còn đỉnh kề nào chưa chốt)");

                int next = -1;
                for (int i = 0; i < n; i++)
                    if (!fin[i] && !double.IsInfinity(dist[i]) && (next < 0 || dist[i] < dist[next])) next = i;

                TableRow row = Snapshot(dist, prev, fin, changed);
                row.Relaxed = u;
                bool reached = u == dst;
                row.Chosen = reached ? -1 : next;
                if (reached)
                    sb.Append("\r\n\r\nĐỉnh ").Append(dst).Append(" là ĐÍCH và đã được chốt: khoảng cách ngắn nhất không thể nhỏ hơn nữa. Dừng.");
                else if (next >= 0)
                    sb.Append("\r\n\r\nĐỉnh nhỏ nhất chưa chốt tiếp theo: đỉnh ").Append(next).Append(" (").Append(m.Fmt(dist[next])).Append(") — đang in đậm.");
                else
                    sb.Append("\r\n\r\nKhông còn đỉnh nào tới được: không có đường tới đích.");
                row.Explain = sb.ToString();
                res.Rows.Add(row);

                if (reached) break;
                u = next;
            }

            // Truy ngược đường đi bằng cột "đỉnh đứng trước"
            if (fin[dst])
            {
                res.Total = dist[dst];
                for (int v = dst; v >= 0; v = prev[v]) res.Path.Insert(0, v);
                TableRow last = res.Rows[res.Rows.Count - 1];
                var pb = new StringBuilder();
                for (int i = 0; i < res.Path.Count; i++) { if (i > 0) pb.Append(" → "); pb.Append(res.Path[i]); }
                last.Explain += "\r\n\r\nTruy ngược theo cột \"đỉnh đứng trước\": " + pb + "\r\nTổng = " + m.Fmt(res.Total) + unitText() + ".";
            }
            return res;
        }

        private static TableRow Snapshot(double[] dist, int[] prev, bool[] fin, bool[] changed)
        {
            return new TableRow
            {
                Dist = (double[])dist.Clone(), Prev = (int[])prev.Clone(),
                Final = (bool[])fin.Clone(), Changed = (bool[])changed.Clone()
            };
        }
    }
    // Bảng A*: giống Dijkstra nhưng chọn đỉnh có f = g + h nhỏ nhất (h = ước lượng "chim bay" còn lại tới đích).
    public static class AStarTable
    {
        public static TableResult Run(GraphModel m, int src, int dst)
        {
            int n = m.Labels.Count;
            var g = new double[n];
            var h = new double[n];
            var prev = new int[n];
            var fin = new bool[n];
            for (int i = 0; i < n; i++) { g[i] = double.PositiveInfinity; prev[i] = -1; h[i] = m.H(i, dst); }
            g[src] = 0;

            var res = new TableResult { AStar = true, H = h };
            Func<string> unitText = () => m.UnitName.Length > 0 ? " " + m.UnitName : "";

            TableRow r0 = Snap(g, h, prev, fin, new bool[n]);
            r0.Chosen = src;
            r0.Explain = "Khởi tạo: g(" + src + ") = 0 (quãng đường đã đi); h(đỉnh) là ước lượng đường chim bay còn lại tới đích " + dst + " (dòng \"h\" trên cùng của bảng).\r\n" +
                         "Ô trong bảng ghi (f, đỉnh đứng trước) với f = g + h. Chọn đỉnh có f nhỏ nhất: đỉnh " + src + " (f = 0 + " + m.Fmt(h[src]) + " = " + m.Fmt(h[src]) + ").";
            res.Rows.Add(r0);

            int u = src, guard = 0;
            while (u >= 0 && guard++ < n + 2)
            {
                fin[u] = true;
                var changed = new bool[n];
                var sb = new StringBuilder();
                sb.Append("Chốt đỉnh ").Append(u).Append(":  g = ").Append(m.Fmt(g[u])).Append(",  h = ").Append(m.Fmt(h[u]))
                  .Append(",  f = g + h = ").Append(m.Fmt(g[u] + h[u])).Append(unitText()).Append(". Xét các đỉnh kề chưa chốt:");
                int considered = 0;
                foreach (Link l in m.Adj[u])
                {
                    int v = l.To;
                    if (fin[v]) continue;
                    considered++;
                    double ng = g[u] + l.W;
                    sb.Append("\r\n  • đỉnh ").Append(v).Append(":  g = ").Append(m.Fmt(g[u])).Append(" + ").Append(m.Fmt(l.W)).Append(" = ").Append(m.Fmt(ng));
                    if (ng < g[v])
                    {
                        sb.Append("  <  ").Append(m.Fmt(g[v])).Append("  →  cập nhật;  h = ").Append(m.Fmt(h[v]))
                          .Append(",  f = ").Append(m.Fmt(ng)).Append(" + ").Append(m.Fmt(h[v])).Append(" = ").Append(m.Fmt(ng + h[v]));
                        g[v] = ng; prev[v] = u; changed[v] = true;
                    }
                    else sb.Append("  ≥  ").Append(m.Fmt(g[v])).Append("  →  giữ nguyên");
                }
                if (considered == 0) sb.Append("\r\n  (không còn đỉnh kề nào chưa chốt)");

                int next = -1;
                for (int i = 0; i < n; i++)
                {
                    if (fin[i] || double.IsInfinity(g[i])) continue;
                    if (next < 0) { next = i; continue; }
                    double fi = g[i] + h[i], fn = g[next] + h[next];
                    if (fi < fn - 1e-9 || (Math.Abs(fi - fn) <= 1e-9 && h[i] < h[next])) next = i;
                }

                TableRow row = Snap(g, h, prev, fin, changed);
                row.Relaxed = u;
                bool reached = u == dst;
                row.Chosen = reached ? -1 : next;
                if (reached)
                    sb.Append("\r\n\r\nĐỉnh ").Append(dst).Append(" là ĐÍCH và đã được chốt. Dừng.");
                else if (next >= 0)
                    sb.Append("\r\n\r\nĐỉnh có f nhỏ nhất chưa chốt: đỉnh ").Append(next).Append(" (f = ").Append(m.Fmt(g[next])).Append(" + ")
                      .Append(m.Fmt(h[next])).Append(" = ").Append(m.Fmt(g[next] + h[next])).Append(") — đang in đậm.");
                else
                    sb.Append("\r\n\r\nKhông còn đỉnh nào tới được: không có đường tới đích.");
                row.Explain = sb.ToString();
                res.Rows.Add(row);

                if (reached) break;
                u = next;
            }

            if (fin[dst])
            {
                res.Total = g[dst];
                for (int v = dst; v >= 0; v = prev[v]) res.Path.Insert(0, v);
                TableRow last = res.Rows[res.Rows.Count - 1];
                var pb = new StringBuilder();
                for (int i = 0; i < res.Path.Count; i++) { if (i > 0) pb.Append(" → "); pb.Append(res.Path[i]); }
                last.Explain += "\r\n\r\nTruy ngược theo cột \"đỉnh đứng trước\": " + pb + "\r\nTổng = " + m.Fmt(res.Total) + unitText() + ".";
                int dj = DijkstraTable.Run(m, src, dst).Expanded;
                last.Explain += "\r\nSo sánh: A* chỉ chốt " + res.Expanded + " đỉnh, còn Dijkstra phải chốt " + dj + " đỉnh để ra cùng kết quả.";
            }
            return res;
        }

        private static TableRow Snap(double[] g, double[] h, int[] prev, bool[] fin, bool[] changed)
        {
            var f = new double[g.Length];
            for (int i = 0; i < f.Length; i++) f[i] = g[i] + h[i];
            return new TableRow
            {
                Dist = (double[])g.Clone(), F = f, Prev = (int[])prev.Clone(),
                Final = (bool[])fin.Clone(), Changed = (bool[])changed.Clone()
            };
        }
    }
}
