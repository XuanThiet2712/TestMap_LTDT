using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace TestMap.Routing
{
    public enum PoiKind { Hospital, School, University, Market, Park, Landmark }

    // Địa điểm đáng chú ý (bệnh viện, trường học...) để vẽ biểu tượng trên bản đồ
    public class Poi
    {
        public PoiKind Kind;
        public double Lat, Lng;
        public string Name = "";

        public static string KindName(PoiKind k)
        {
            switch (k)
            {
                case PoiKind.Hospital: return "Bệnh viện";
                case PoiKind.School: return "Trường học";
                case PoiKind.University: return "Đại học";
                case PoiKind.Market: return "Chợ / TTTM";
                case PoiKind.Park: return "Công viên";
                default: return "Danh lam";
            }
        }
    }

    // Đọc / tải địa điểm. File csv: loại,vĩ độ,kinh độ,tên
    //   Data/pois_seed.csv       : danh sách mẫu đi kèm chương trình (tọa độ gần đúng)
    //   Data/<khu vực>/pois.csv  : tải từ OpenStreetMap bằng nút "Tải địa điểm" (chính xác hơn, ưu tiên dùng)
    public static class PoiStore
    {
        public static List<Poi> LoadFor(string dataRoot, string areaDir, double s, double w, double n, double e)
        {
            string own = Path.Combine(areaDir, "pois.csv");
            List<Poi> all = Read(File.Exists(own) ? own : Path.Combine(dataRoot, "pois_seed.csv"));
            var res = new List<Poi>();
            foreach (Poi p in all)
                if (p.Lat >= s && p.Lat <= n && p.Lng >= w && p.Lng <= e) res.Add(p);
            return res;
        }

        public static List<Poi> Read(string path)
        {
            var res = new List<Poi>();
            if (!File.Exists(path)) return res;
            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                string[] p = line.Split(new[] { ',' }, 4);
                double la, ln; PoiKind k;
                if (p.Length < 4 || !Enum.TryParse(p[0], true, out k) ||
                    !double.TryParse(p[1], NumberStyles.Float, CultureInfo.InvariantCulture, out la) ||
                    !double.TryParse(p[2], NumberStyles.Float, CultureInfo.InvariantCulture, out ln)) continue;
                res.Add(new Poi { Kind = k, Lat = la, Lng = ln, Name = p[3].Trim() });
            }
            return res;
        }

        // Tải từ OpenStreetMap (Overpass). Gọi trong luồng nền. Trả về số địa điểm.
        public static int Download(double s, double w, double n, double e, string areaDir, Action<string> progress)
        {
            var inv = CultureInfo.InvariantCulture;
            string box = s.ToString("F6", inv) + "," + w.ToString("F6", inv) + "," + n.ToString("F6", inv) + "," + e.ToString("F6", inv);
            string[] filters = {
                "[\"amenity\"=\"hospital\"]", "[\"amenity\"~\"^(school|kindergarten)$\"]", "[\"amenity\"~\"^(university|college)$\"]",
                "[\"amenity\"=\"marketplace\"]", "[\"shop\"~\"^(mall|supermarket|department_store)$\"]",
                "[\"leisure\"=\"park\"]", "[\"tourism\"~\"^(attraction|museum|zoo)$\"]", "[\"historic\"~\"^(monument|memorial|castle)$\"]"
            };
            var q = new StringBuilder("[out:xml][timeout:90];(");
            foreach (string f in filters)
                q.Append("node").Append(f).Append("(").Append(box).Append(");way").Append(f).Append("(").Append(box).Append(");");
            q.Append(");out center tags;");

            byte[] data = OsmImporter.Post(q.ToString(), 120000, progress);
            var doc = new XmlDocument();
            doc.LoadXml(Encoding.UTF8.GetString(data));

            var res = new List<Poi>();
            var seen = new HashSet<string>();
            foreach (XmlNode el in doc.DocumentElement.ChildNodes)
            {
                if (el.NodeType != XmlNodeType.Element || (el.Name != "node" && el.Name != "way")) continue;
                string lat = null, lon = null;
                if (el.Name == "node") { lat = Attr(el, "lat"); lon = Attr(el, "lon"); }
                else { XmlNode c = el.SelectSingleNode("center"); if (c != null) { lat = Attr(c, "lat"); lon = Attr(c, "lon"); } }
                double la, ln;
                if (!double.TryParse(lat, NumberStyles.Float, inv, out la) || !double.TryParse(lon, NumberStyles.Float, inv, out ln)) continue;

                string name = null, amenity = null, shop = null, leisure = null, tourism = null, historic = null;
                foreach (XmlNode t in el.SelectNodes("tag"))
                {
                    string k = Attr(t, "k"), v = Attr(t, "v");
                    if (k == "name:vi" || (k == "name" && name == null)) name = v;
                    else if (k == "amenity") amenity = v; else if (k == "shop") shop = v;
                    else if (k == "leisure") leisure = v; else if (k == "tourism") tourism = v; else if (k == "historic") historic = v;
                }
                if (string.IsNullOrWhiteSpace(name)) continue;       // địa điểm không tên thì bỏ cho đỡ rối
                PoiKind kind;
                if (amenity == "hospital") kind = PoiKind.Hospital;
                else if (amenity == "school" || amenity == "kindergarten") kind = PoiKind.School;
                else if (amenity == "university" || amenity == "college") kind = PoiKind.University;
                else if (amenity == "marketplace" || shop != null) kind = PoiKind.Market;
                else if (leisure == "park") kind = PoiKind.Park;
                else if (tourism != null || historic != null) kind = PoiKind.Landmark;
                else continue;
                if (!seen.Add(kind + "|" + name.ToLowerInvariant())) continue;
                res.Add(new Poi { Kind = kind, Lat = la, Lng = ln, Name = name.Trim() });
            }

            var sb = new StringBuilder();
            foreach (Poi p in res)
                sb.Append(p.Kind).Append(',').Append(p.Lat.ToString("F6", inv)).Append(',').Append(p.Lng.ToString("F6", inv))
                  .Append(',').Append(p.Name.Replace(',', ' ').Replace('\n', ' ')).Append('\n');
            Directory.CreateDirectory(areaDir);
            File.WriteAllText(Path.Combine(areaDir, "pois.csv"), sb.ToString(), new UTF8Encoding(false));
            return res.Count;
        }

        private static string Attr(XmlNode n, string name)
        {
            XmlAttribute a = n.Attributes[name];
            return a == null ? null : a.Value;
        }
    }
}
