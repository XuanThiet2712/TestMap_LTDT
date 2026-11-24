using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace TestMap.Routing
{
    // Tải đường và trạm xe buýt của MỘT KHU VỰC NHỎ từ OpenStreetMap (qua Overpass API),
    // đổi thành 3 file trong thư mục xuất:
    //   nodes.csv  lat,lng                                   (giao lộ)
    //   edges.csv  from,to,length,oneway,geometry,class      (đoạn đường, geometry = điểm uốn thật của đường)
    //   stops.csv  lat,lng,node,name                         (trạm xe buýt, node = giao lộ gần trạm nhất)
    // Chỉ cần Internet ở lần tải đầu tiên; lần sau đọc thẳng từ 3 file này.
    public static class OsmImporter
    {
        internal static readonly string[] Servers =
{
    "https://overpass-api.de/api/interpreter",
    "https://overpass.private.coffee/api/interpreter",
    "https://overpass.kumi.systems/api/interpreter",
    "https://overpass.openstreetmap.fr/api/interpreter"
};

        // Các loại đường ô tô/xe buýt đi được -> cấp đường dùng để vẽ (0 nhỏ ... 3 lớn)
        private static readonly Dictionary<string, int> RoadClass = new Dictionary<string, int>
        {
            { "motorway", 3 }, { "motorway_link", 3 }, { "trunk", 3 }, { "trunk_link", 3 },
            { "primary", 3 }, { "primary_link", 3 },
            { "secondary", 2 }, { "secondary_link", 2 },
            { "tertiary", 1 }, { "tertiary_link", 1 },
            { "unclassified", 0 }, { "residential", 0 }, { "living_street", 0 }
        };

        private class WayTmp { public List<long> Refs; public bool OneWay; public int Cls; }
        private class StopTmp { public LatLng Pos; public string Name; public long Vertex; }
        private class SplitTmp { public int Way, Seg; public double T; public long Id; }
        private class EdgeTmp { public int A, B; public double Length; public bool OneWay; public List<LatLng> Geo; public int Cls; }

        // Gọi trong luồng nền (Task.Run). progress nhận các dòng trạng thái để hiển thị.
        public static void ImportArea(double south, double west, double north, double east,
                                      string outDir, Action<string> progress)
        {
            var inv = CultureInfo.InvariantCulture;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string box = south.ToString("F6", inv) + "," + west.ToString("F6", inv) + "," +
                         north.ToString("F6", inv) + "," + east.ToString("F6", inv);
            string query =
                "[out:xml][timeout:120];(" +
                "way[\"highway\"~\"^(motorway|motorway_link|trunk|trunk_link|primary|primary_link|secondary|secondary_link|tertiary|tertiary_link|unclassified|residential|living_street)$\"](" + box + ");" +
                ">;" +
                "node[\"highway\"=\"bus_stop\"](" + box + ");" +
                ");out body;";

            // ---------- 1. Tải dữ liệu ----------
            byte[] data = null;
            Exception lastError = null;
            for (int attempt = 0; attempt < 2 && data == null; attempt++)
            {
                foreach (string url in Servers)
                {
                    try
                    {
                        progress("Đang tải đường và trạm xe buýt từ " + new Uri(url).Host + " (có thể mất 10-60 giây)...");
                        var req = (HttpWebRequest)WebRequest.Create(url);
                        req.Method = "POST";
                        req.UserAgent = "TestMapDemo/1.0 (hoc tap)";
                        req.ContentType = "application/x-www-form-urlencoded";
                        req.Timeout = 180000;
                        req.ReadWriteTimeout = 180000;
                        req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        byte[] body = Encoding.UTF8.GetBytes("data=" + Uri.EscapeDataString(query));
                        req.ContentLength = body.Length;
                        using (Stream rs = req.GetRequestStream()) rs.Write(body, 0, body.Length);
                        using (var resp = (HttpWebResponse)req.GetResponse())
                        using (Stream s = resp.GetResponseStream())
                        data = ReadAll(s, progress);
                        break;
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                        data = null;
                    }
                }
                if (data == null)
                    throw new Exception("không kết nối được máy chủ OpenStreetMap (" + (lastError != null ? lastError.Message : "") + ")");
            }
            ConvertOsm(data, outDir, progress);
        }

        // Đọc hết luồng dữ liệu, cứ ~0,3 giây báo số MB đã nhận (để người dùng thấy đang tải tới đâu)
        private static byte[] ReadAll(Stream s, Action<string> progress)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            using (var ms = new MemoryStream())
            {
                var buf = new byte[32768];
                int n;
                long last = 0;
                while ((n = s.Read(buf, 0, buf.Length)) > 0)
                {
                    ms.Write(buf, 0, n);
                    if (progress != null && sw.ElapsedMilliseconds - last > 300)
                    {
                        last = sw.ElapsedMilliseconds;
                        progress("Đã nhận " + (ms.Length / 1048576.0).ToString("0.0") + " MB từ OpenStreetMap...");
                    }
                }
                return ms.ToArray();
            }
        }

        // Gửi một truy vấn Overpass, thử lần lượt các máy chủ. Dùng cho việc tải tuyến xe buýt.
        internal static byte[] Post(string query, int timeoutMs, Action<string> progress)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Exception last = null;
            foreach (string url in Servers)
            {
                try
                {
                    progress("Đang tải từ " + new Uri(url).Host + " (có thể mất 10-60 giây)...");
                    var req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "POST";
                    req.UserAgent = "TestMapDemo/1.0 (hoc tap)";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Timeout = timeoutMs;
                    req.ReadWriteTimeout = timeoutMs;
                    req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    byte[] body = Encoding.UTF8.GetBytes("data=" + Uri.EscapeDataString(query));
                    req.ContentLength = body.Length;
                    using (Stream rs = req.GetRequestStream()) rs.Write(body, 0, body.Length);
                    using (var resp = (HttpWebResponse)req.GetResponse())
                    using (Stream st = resp.GetResponseStream())
                        return ReadAll(st, progress);
                }
                catch (Exception ex) { last = ex; }
            }
            throw new Exception("không kết nối được máy chủ OpenStreetMap (" + (last != null ? last.Message : "") + ")");
        }

        // Đổi dữ liệu OSM (XML) thành 3 file csv. Tách riêng khỏi phần tải để kiểm thử được mà không cần Internet.
        public static void ConvertOsm(byte[] data, string outDir, Action<string> progress)
        {
            var inv = CultureInfo.InvariantCulture;

            // ---------- 2. Đọc XML ----------
            progress("Đang xử lý dữ liệu...");
            XDocument doc;
            using (var ms = new MemoryStream(data)) doc = XDocument.Load(ms);
            if (doc.Root == null || doc.Root.Name.LocalName != "osm")
                throw new Exception("máy chủ trả về dữ liệu không hợp lệ (có thể đang quá tải, thử lại sau)");

            var coords = new Dictionary<long, LatLng>();
            var stops = new List<StopTmp>();
            var ways = new List<WayTmp>();

            foreach (XElement el in doc.Root.Elements())
            {
                string kind = el.Name.LocalName;
                if (kind == "node")
                {
                    long id = long.Parse((string)el.Attribute("id"), inv);
                    var pos = new LatLng(double.Parse((string)el.Attribute("lat"), inv),
                                         double.Parse((string)el.Attribute("lon"), inv));
                    coords[id] = pos;

                    bool isStop = false;
                    string name = "";
                    foreach (XElement t in el.Elements("tag"))
                    {
                        string k = (string)t.Attribute("k");
                        string v = (string)t.Attribute("v");
                        if (k == "highway" && v == "bus_stop") isStop = true;
                        else if (k == "name") name = v ?? "";
                    }
                    if (isStop) stops.Add(new StopTmp { Pos = pos, Name = name });
                }
                else if (kind == "way")
                {
                    var tags = new Dictionary<string, string>();
                    foreach (XElement t in el.Elements("tag")) tags[(string)t.Attribute("k")] = (string)t.Attribute("v");

                    string hw;
                    if (!tags.TryGetValue("highway", out hw) || !RoadClass.ContainsKey(hw)) continue;
                    string acc;
                    if (tags.TryGetValue("access", out acc) && (acc == "no" || acc == "private")) continue;
                    if (tags.TryGetValue("motor_vehicle", out acc) && (acc == "no" || acc == "private")) continue;

                    var refs = new List<long>();
                    foreach (XElement nd in el.Elements("nd")) refs.Add(long.Parse((string)nd.Attribute("ref"), inv));
                    if (refs.Count < 2) continue;

                    string ow;
                    tags.TryGetValue("oneway", out ow);
                    bool oneway = false;
                    if (ow == "yes" || ow == "true" || ow == "1") oneway = true;
                    else if (ow == "-1") { oneway = true; refs.Reverse(); }
                    else if (ow == null)
                    {
                        string jn;
                        tags.TryGetValue("junction", out jn);
                        if (jn == "roundabout" || jn == "circular" || hw == "motorway" || hw == "motorway_link") oneway = true;
                    }

                    ways.Add(new WayTmp { Refs = refs, OneWay = oneway, Cls = RoadClass[hw] });
                }
            }
            doc = null;
            data = null;

            if (ways.Count == 0) throw new Exception("không tìm thấy con đường nào trong khu vực này");

            // ---------- 3. Xác định giao lộ ----------
            var useCount = new Dictionary<long, int>();
            foreach (WayTmp w in ways)
            {
                w.Refs.RemoveAll(r => !coords.ContainsKey(r));
                foreach (long r in w.Refs)
                {
                    int c;
                    useCount.TryGetValue(r, out c);
                    useCount[r] = c + 1;
                }
            }

            var junction = new HashSet<long>();
            foreach (WayTmp w in ways)
            {
                if (w.Refs.Count < 2) continue;
                junction.Add(w.Refs[0]);
                junction.Add(w.Refs[w.Refs.Count - 1]);
            }
            foreach (KeyValuePair<long, int> kv in useCount)
                if (kv.Value >= 2) junction.Add(kv.Key);

            // Gắn mỗi trạm vào ĐIỂM GẦN NHẤT TRÊN ĐƯỜNG (chiếu vuông góc lên đoạn đường, trong 50 m) rồi biến điểm đó
            // thành giao lộ. Nhờ vậy trạm nằm đúng trên đường và đường đi đi qua đúng trạm.
            // (Bản cũ chỉ chọn điểm nút gần nhất nên trạm có thể lệch khỏi đường tới ~60 m.)
            var keptStops = new List<StopTmp>();
            var inserts = new List<SplitTmp>();
            long fakeId = 0;                       // điểm mới tạo dùng id âm để không trùng id thật của OSM
            foreach (StopTmp st in stops)
            {
                double mx = 111320.0 * Math.Cos(st.Pos.Lat * Math.PI / 180), my = 111320.0;
                double best = double.MaxValue, bestDist = 0, bestT = 0, bestLen = 0;
                int bestWay = -1, bestSeg = -1;
                for (int wi = 0; wi < ways.Count; wi++)
                {
                    WayTmp w = ways[wi];
                    if (w.Refs.Count < 2) continue;
                    double penalty = w.Cls == 0 ? 8.0 : 0.0;      // ở ngã tư, ưu tiên đường lớn hơn hẻm
                    LatLng pa = coords[w.Refs[0]];
                    for (int i = 0; i + 1 < w.Refs.Count; i++)
                    {
                        LatLng pb = coords[w.Refs[i + 1]];
                        double ax = (pa.Lng - st.Pos.Lng) * mx, ay = (pa.Lat - st.Pos.Lat) * my;
                        double bx = (pb.Lng - st.Pos.Lng) * mx, by = (pb.Lat - st.Pos.Lat) * my;
                        double dx = bx - ax, dy = by - ay, len2 = dx * dx + dy * dy;
                        double t = len2 > 0 ? -(ax * dx + ay * dy) / len2 : 0;
                        if (t < 0) t = 0; else if (t > 1) t = 1;
                        double qx = ax + t * dx, qy = ay + t * dy;
                        double d = Math.Sqrt(qx * qx + qy * qy);
                        if (d + penalty < best)
                        {
                            best = d + penalty; bestDist = d; bestT = t; bestLen = Math.Sqrt(len2);
                            bestWay = wi; bestSeg = i;
                        }
                        pa = pb;
                    }
                }
                if (bestWay < 0 || bestDist > 50.0) continue;

                WayTmp bw = ways[bestWay];
                if (bestT * bestLen < 3.0) st.Vertex = bw.Refs[bestSeg];                  // sát điểm đầu đoạn: dùng luôn điểm đó
                else if ((1 - bestT) * bestLen < 3.0) st.Vertex = bw.Refs[bestSeg + 1];   // sát điểm cuối đoạn
                else
                {
                    long id = 0;
                    foreach (SplitTmp sp in inserts)      // hai trạm đối diện nhau thì dùng chung một điểm
                        if (sp.Way == bestWay && sp.Seg == bestSeg && Math.Abs(sp.T - bestT) * bestLen < 1.0) { id = sp.Id; break; }
                    if (id == 0)
                    {
                        LatLng a = coords[bw.Refs[bestSeg]], b = coords[bw.Refs[bestSeg + 1]];
                        id = --fakeId;
                        coords[id] = new LatLng(a.Lat + (b.Lat - a.Lat) * bestT, a.Lng + (b.Lng - a.Lng) * bestT);
                        inserts.Add(new SplitTmp { Way = bestWay, Seg = bestSeg, T = bestT, Id = id });
                    }
                    st.Vertex = id;
                }
                junction.Add(st.Vertex);
                keptStops.Add(st);
            }

            // Chèn các điểm mới vào đường. Chèn từ cuối về đầu để chỉ số chưa chèn không bị xê dịch.
            inserts.Sort(delegate (SplitTmp x, SplitTmp y)
            {
                if (x.Way != y.Way) return x.Way.CompareTo(y.Way);
                if (x.Seg != y.Seg) return y.Seg.CompareTo(x.Seg);
                return y.T.CompareTo(x.T);
            });
            foreach (SplitTmp sp in inserts) ways[sp.Way].Refs.Insert(sp.Seg + 1, sp.Id);

            // ---------- 4. Cắt đường thành các đoạn giữa 2 giao lộ ----------
            var index = new Dictionary<long, int>();
            var nodes = new List<LatLng>();
            Func<long, int> jid = delegate (long r)
            {
                int i;
                if (!index.TryGetValue(r, out i))
                {
                    i = nodes.Count;
                    index[r] = i;
                    nodes.Add(coords[r]);
                }
                return i;
            };

            var edges = new List<EdgeTmp>();
            foreach (WayTmp w in ways)
            {
                if (w.Refs.Count < 2) continue;
                var seg = new List<long>();
                seg.Add(w.Refs[0]);
                for (int q = 1; q < w.Refs.Count; q++)
                {
                    long r = w.Refs[q];
                    seg.Add(r);
                    if (!junction.Contains(r)) continue;

                    long a = seg[0], b = seg[seg.Count - 1];
                    if (a != b)
                    {
                        double length = 0;
                        var geo = new List<LatLng>();
                        for (int i = 0; i < seg.Count - 1; i++)
                            length += RoadGraph.Distance(coords[seg[i]], coords[seg[i + 1]]);
                        for (int i = 1; i < seg.Count - 1; i++) geo.Add(coords[seg[i]]);
                        edges.Add(new EdgeTmp { A = jid(a), B = jid(b), Length = length, OneWay = w.OneWay, Geo = geo, Cls = w.Cls });
                    }
                    seg.Clear();
                    seg.Add(r);
                }
            }

            // ---------- 5. Chỉ giữ phần đường liên thông lớn nhất ----------
            int[] parent = new int[nodes.Count];
            for (int i = 0; i < parent.Length; i++) parent[i] = i;
            Func<int, int> find = null;
            find = delegate (int x)
            {
                while (parent[x] != x)
                {
                    parent[x] = parent[parent[x]];
                    x = parent[x];
                }
                return x;
            };
            foreach (EdgeTmp e in edges)
            {
                int ra = find(e.A), rb = find(e.B);
                if (ra != rb) parent[ra] = rb;
            }
            var size = new Dictionary<int, int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                int r = find(i);
                int c;
                size.TryGetValue(r, out c);
                size[r] = c + 1;
            }
            int biggest = -1, biggestSize = -1;
            foreach (KeyValuePair<int, int> kv in size)
                if (kv.Value > biggestSize) { biggestSize = kv.Value; biggest = kv.Key; }

            var remap = new Dictionary<int, int>();
            var keptNodes = new List<LatLng>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (find(i) == biggest)
                {
                    remap[i] = keptNodes.Count;
                    keptNodes.Add(nodes[i]);
                }
            }

            // ---------- 6. Ghi file ----------
            progress("Đang lưu dữ liệu...");
            var sbNodes = new StringBuilder("lat,lng\n");
            foreach (LatLng p in keptNodes)
                sbNodes.Append(p.Lat.ToString("F6", inv)).Append(',').Append(p.Lng.ToString("F6", inv)).Append('\n');

            var sbEdges = new StringBuilder("from,to,length,oneway,geometry,class\n");
            int edgeCount = 0;
            foreach (EdgeTmp e in edges)
            {
                if (!remap.ContainsKey(e.A) || !remap.ContainsKey(e.B)) continue;
                sbEdges.Append(remap[e.A]).Append(',').Append(remap[e.B]).Append(',')
                       .Append(e.Length.ToString("F1", inv)).Append(',').Append(e.OneWay ? 1 : 0).Append(',');
                for (int i = 0; i < e.Geo.Count; i++)
                {
                    if (i > 0) sbEdges.Append(';');
                    sbEdges.Append(e.Geo[i].Lat.ToString("F6", inv)).Append(' ').Append(e.Geo[i].Lng.ToString("F6", inv));
                }
                sbEdges.Append(',').Append(e.Cls).Append('\n');
                edgeCount++;
            }

            var sbStops = new StringBuilder("lat,lng,node,name\n");
            var seen = new HashSet<string>();
            int stopCount = 0;
            foreach (StopTmp st in keptStops)
            {
                int ni;
                int fi;
                if (!index.TryGetValue(st.Vertex, out ni) || !remap.TryGetValue(ni, out fi)) continue;
                string name = (st.Name ?? "").Replace(',', ' ').Replace('\r', ' ').Replace('\n', ' ').Trim();
                if (name.Length == 0) name = "Trạm xe buýt";
                if (!seen.Add(fi + "|" + name)) continue;   // bỏ trạm trùng tên cùng một chỗ
                sbStops.Append(st.Pos.Lat.ToString("F6", inv)).Append(',').Append(st.Pos.Lng.ToString("F6", inv))
                       .Append(',').Append(fi).Append(',').Append(name).Append('\n');
                stopCount++;
            }

            Directory.CreateDirectory(outDir);
            var utf8 = new UTF8Encoding(false);
            File.WriteAllText(Path.Combine(outDir, "nodes.csv"), sbNodes.ToString(), utf8);
            File.WriteAllText(Path.Combine(outDir, "edges.csv"), sbEdges.ToString(), utf8);
            File.WriteAllText(Path.Combine(outDir, "stops.csv"), sbStops.ToString(), utf8);

            progress("Đã lưu " + keptNodes.Count + " giao lộ, " + edgeCount + " đoạn đường, " + stopCount + " trạm xe buýt.");
        }
    }
}
