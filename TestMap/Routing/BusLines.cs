using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace TestMap.Routing
{
    // Nạp / tạo / tải các tuyến xe buýt.
    //   - Tuyến THẬT: lấy từ các "relation route=bus" của OpenStreetMap, lưu vào routes.csv.
    //   - Tuyến DEMO: tự tạo từ mạng đường khi chưa có dữ liệu thật (để app luôn có gì đó để chạy thử).
    public static class BusLines
    {
        private static readonly Color[] Palette =
        {
            Color.FromArgb(220, 38, 38),  Color.FromArgb(37, 99, 235),  Color.FromArgb(22, 163, 74),
            Color.FromArgb(217, 119, 6),  Color.FromArgb(147, 51, 234), Color.FromArgb(8, 145, 178),
            Color.FromArgb(219, 39, 119), Color.FromArgb(101, 163, 13), Color.FromArgb(234, 88, 12),
            Color.FromArgb(79, 70, 229),  Color.FromArgb(13, 148, 136), Color.FromArgb(120, 53, 15)
        };

        private const double MatchMeters = 70.0;     // điểm dừng trong relation phải cách trạm đã có tối đa chừng này

        // ------------------------------------------------------------------
        //  TẢI TỪ OPENSTREETMAP
        // ------------------------------------------------------------------
        // Tải các tuyến xe buýt trong khung tọa độ, lưu vào path. Trả về số tuyến (một chiều) đã lưu.
        public static int Download(double south, double west, double north, double east, string path, Action<string> progress)
        {
            var inv = CultureInfo.InvariantCulture;
            string bbox = south.ToString(inv) + "," + west.ToString(inv) + "," + north.ToString(inv) + "," + east.ToString(inv);
            // relation tuyến xe buýt trong khung, cộng với các điểm (trạm, vị trí dừng) mà chúng tham chiếu tới
            string query = "[out:xml][timeout:90];relation[\"route\"=\"bus\"](" + bbox + ")->.r;(.r;node(r.r););out body;";
            byte[] data = OsmImporter.Post(query, 90000, progress);
            progress("Đang xử lý các tuyến xe buýt...");
            return ParseRoutes(data, path);
        }

        // Đọc XML Overpass, ghi routes.csv: ref,name,colour,"lat lng;lat lng;..."
        public static int ParseRoutes(byte[] data, string path)
        {
            var inv = CultureInfo.InvariantCulture;
            XDocument doc;
            using (var ms = new MemoryStream(data)) doc = XDocument.Load(ms);

            var pos = new Dictionary<long, LatLng>();
            foreach (XElement el in doc.Root.Elements("node"))
            {
                XAttribute id = el.Attribute("id"), la = el.Attribute("lat"), lo = el.Attribute("lon");
                if (id == null || la == null || lo == null) continue;
                pos[long.Parse(id.Value, inv)] = new LatLng(double.Parse(la.Value, inv), double.Parse(lo.Value, inv));
            }

            var sb = new StringBuilder("ref,name,colour,points\n");
            int count = 0;
            foreach (XElement rel in doc.Root.Elements("relation"))
            {
                string rf = Tag(rel, "ref"), name = Tag(rel, "name"), fromName = Tag(rel, "from"), toName = Tag(rel, "to");
                if (name.Length == 0 && fromName.Length > 0) name = fromName + " - " + toName;
                if (rf.Length == 0) rf = name.Length > 0 ? name : "?";

                List<LatLng> pts = Members(rel, pos, "stop");
                if (pts.Count < 2) pts = Members(rel, pos, "platform");
                if (pts.Count < 2) continue;

                var pp = new StringBuilder();
                for (int i = 0; i < pts.Count; i++)
                {
                    if (i > 0) pp.Append(';');
                    pp.Append(pts[i].Lat.ToString("F6", inv)).Append(' ').Append(pts[i].Lng.ToString("F6", inv));
                }
                sb.Append(Clean(rf)).Append(',').Append(Clean(name)).Append(',').Append(Clean(Tag(rel, "colour"))).Append(',').Append(pp).Append('\n');
                count++;
            }
            if (count > 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
            }
            return count;
        }

        private static string Tag(XElement el, string key)
        {
            foreach (XElement t in el.Elements("tag"))
            {
                XAttribute k = t.Attribute("k"), v = t.Attribute("v");
                if (k != null && v != null && k.Value == key) return v.Value;
            }
            return "";
        }

        private static string Clean(string s)
        {
            return s.Replace(',', ' ').Replace(';', ' ').Replace('\n', ' ').Replace('\r', ' ').Trim();
        }

        private static List<LatLng> Members(XElement rel, Dictionary<long, LatLng> pos, string rolePrefix)
        {
            var res = new List<LatLng>();
            foreach (XElement m in rel.Elements("member"))
            {
                XAttribute type = m.Attribute("type"), rf = m.Attribute("ref"), role = m.Attribute("role");
                if (type == null || rf == null || type.Value != "node") continue;
                if (role == null || !role.Value.StartsWith(rolePrefix, StringComparison.Ordinal)) continue;
                LatLng p;
                if (pos.TryGetValue(long.Parse(rf.Value, CultureInfo.InvariantCulture), out p)) res.Add(p);
            }
            return res;
        }

        // ------------------------------------------------------------------
        //  ĐỌC routes.csv
        // ------------------------------------------------------------------
        public static List<BusLine> Load(RoadGraph g, string path)
        {
            var inv = CultureInfo.InvariantCulture;
            var res = new List<BusLine>();
            string[] rows = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 1; i < rows.Length; i++)
            {
                string[] f = rows[i].Split(new[] { ',' }, 4);
                if (f.Length < 4) continue;

                var line = new BusLine { Ref = f[0], Name = f[1] };
                foreach (string pt in f[3].Split(';'))
                {
                    string[] xy = pt.Split(' ');
                    if (xy.Length != 2) continue;
                    int s = MatchStop(g, double.Parse(xy[0], inv), double.Parse(xy[1], inv));
                    if (s >= 0 && (line.Stops.Count == 0 || line.Stops[line.Stops.Count - 1] != s)) line.Stops.Add(s);
                }
                if (line.Stops.Count >= 2) res.Add(line);
            }
            Finish(g, res);
            return res;
        }

        // Trạm gần điểm (lat, lng) nhất, so với cả vị trí gốc lẫn vị trí đã gắn lên đường; -1 nếu quá xa
        private static int MatchStop(RoadGraph g, double lat, double lng)
        {
            double k = Math.Cos(lat * Math.PI / 180) * 111320.0, m = 111320.0;
            int best = -1;
            double bestD = MatchMeters * MatchMeters;
            for (int i = 0; i < g.Stops.Count; i++)
            {
                BusStop s = g.Stops[i];
                double d1 = Sq((s.RawLng - lng) * k) + Sq((s.RawLat - lat) * m);
                double d2 = Sq((s.Lng - lng) * k) + Sq((s.Lat - lat) * m);
                double d = Math.Min(d1, d2);
                if (d < bestD) { bestD = d; best = i; }
            }
            return best;
        }

        private static double Sq(double x) { return x * x; }

        // ------------------------------------------------------------------
        //  TUYẾN DEMO
        // ------------------------------------------------------------------
        // Tạo vài tuyến chạy ngang dọc khu vực (mỗi tuyến 2 chiều) để có chỗ thử tính năng gợi ý và đổi tuyến.
        public static List<BusLine> Demo(RoadGraph g)
        {
            var res = new List<BusLine>();
            if (g.Stops.Count < 6 || g.Nodes.Count == 0) return res;

            double minLat, minLng, maxLat, maxLng;
            g.GetBounds(out minLat, out minLng, out maxLat, out maxLng);
            // mỗi tuyến: (vĩ độ, kinh độ) điểm đầu và cuối, theo tỉ lệ trong khung khu vực
            double[][] pairs =
            {
                new[] { 0.10, 0.10, 0.90, 0.90 },
                new[] { 0.10, 0.90, 0.90, 0.10 },
                new[] { 0.50, 0.05, 0.50, 0.95 },
                new[] { 0.05, 0.50, 0.95, 0.50 },
                new[] { 0.20, 0.25, 0.80, 0.95 }
            };
            for (int k = 0; k < pairs.Length; k++)
            {
                int a = g.NearestStop(minLat + (maxLat - minLat) * pairs[k][0], minLng + (maxLng - minLng) * pairs[k][1]);
                int b = g.NearestStop(minLat + (maxLat - minLat) * pairs[k][2], minLng + (maxLng - minLng) * pairs[k][3]);
                if (a == b) continue;
                for (int dir = 0; dir < 2; dir++)
                {
                    int src = dir == 0 ? a : b, dst = dir == 0 ? b : a;
                    SearchResult r = PathFinder.Find(g, g.Stops[src].Node, g.Stops[dst].Node, true);
                    if (!r.Found) continue;
                    var line = new BusLine { Ref = "D" + (k + 1), Name = "Tuyến demo", Demo = true };
                    foreach (int node in r.PathNodes)
                    {
                        List<int> at = g.StopsAtNode[node];
                        if (at != null && at.Count > 0 && (line.Stops.Count == 0 || line.Stops[line.Stops.Count - 1] != at[0]))
                            line.Stops.Add(at[0]);
                    }
                    if (line.Stops.Count >= 3) res.Add(line);
                }
            }
            Finish(g, res);
            return res;
        }

        // Gán màu theo số tuyến, lệch pha, dựng đường chạy
        private static void Finish(RoadGraph g, List<BusLine> lines)
        {
            var colorOf = new Dictionary<string, Color>();
            for (int i = 0; i < lines.Count; i++)
            {
                BusLine l = lines[i];
                Color c;
                if (!colorOf.TryGetValue(l.Ref, out c))
                {
                    c = Palette[colorOf.Count % Palette.Length];
                    colorOf[l.Ref] = c;
                }
                l.Color = c;
                l.Phase = (i * 0.37) % 1.0;
                l.BuildPath(g);
            }
            lines.RemoveAll(l => l.Path.Points.Count < 2 || l.Length < 100);
        }
    }
}
