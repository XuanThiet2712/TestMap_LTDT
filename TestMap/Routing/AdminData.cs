using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace TestMap.Routing
{
    // Thao tác chỉnh sửa trạm / tuyến của admin, và lưu / nạp dữ liệu admin.
    // Admin làm việc trên BẢN SAO danh sách trạm và tuyến; chỉ khi bấm lưu mới ghi ra file.
    public static class AdminData
    {
        // Bản sao độc lập của danh sách trạm
        public static List<BusStop> CloneStops(List<BusStop> src)
        {
            var res = new List<BusStop>();
            foreach (BusStop s in src)
                res.Add(new BusStop { Lat = s.Lat, Lng = s.Lng, RawLat = s.RawLat, RawLng = s.RawLng, Node = s.Node, Name = s.Name });
            return res;
        }

        // Bản sao một tuyến (chỉ dữ liệu, không có đường chạy)
        public static BusLine CloneLine(BusLine l)
        {
            return new BusLine
            {
                Ref = l.Ref, Name = l.Name, Color = l.Color, CustomColor = l.CustomColor, Demo = false,
                Fare = l.Fare, HeadwayMin = l.HeadwayMin, Hours = l.Hours, Stops = new List<int>(l.Stops)
            };
        }

        // Tọa độ gốc của trạm dùng để nhận ra trạm khi nạp lại tuyến, nên không được trùng với trạm khác
        private static void MakeUnique(List<BusStop> stops, int skip, ref double lat, ref double lng)
        {
            for (int guard = 0; guard < 50; guard++)
            {
                bool clash = false;
                for (int i = 0; i < stops.Count; i++)
                    if (i != skip && Math.Abs(stops[i].RawLat - lat) < 2e-6 && Math.Abs(stops[i].RawLng - lng) < 2e-6) { clash = true; break; }
                if (!clash) return;
                lng += 5e-6;                // lệch khoảng nửa mét
            }
        }

        // Thêm trạm mới tại (lat, lng): gắn vào giao lộ gần nhất. Trả về chỉ số trạm và khoảng cách tới giao lộ đó.
        public static int AddStop(RoadGraph g, List<BusStop> stops, string name, double lat, double lng, out double snapMeters)
        {
            MakeUnique(stops, -1, ref lat, ref lng);
            int node = g.Nearest(lat, lng);
            snapMeters = RoadGraph.Distance(new LatLng(lat, lng), g.Nodes[node]);
            stops.Add(new BusStop
            {
                Name = name.Trim(), RawLat = lat, RawLng = lng, Node = node,
                Lat = g.Nodes[node].Lat, Lng = g.Nodes[node].Lng
            });
            return stops.Count - 1;
        }

        // Dời trạm sang vị trí mới (gắn vào giao lộ gần nhất)
        public static double MoveStop(RoadGraph g, List<BusStop> stops, int index, double lat, double lng)
        {
            MakeUnique(stops, index, ref lat, ref lng);
            int node = g.Nearest(lat, lng);
            BusStop s = stops[index];
            s.RawLat = lat; s.RawLng = lng; s.Node = node;
            s.Lat = g.Nodes[node].Lat; s.Lng = g.Nodes[node].Lng;
            return RoadGraph.Distance(new LatLng(lat, lng), g.Nodes[node]);
        }

        // Xóa trạm. Các tuyến đi qua trạm đó bị bỏ trạm này; tuyến còn dưới 2 trạm thì bị xóa luôn.
        // Trả về số tuyến bị xóa.
        public static int RemoveStop(List<BusStop> stops, List<BusLine> lines, int index)
        {
            stops.RemoveAt(index);
            int removed = 0;
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var ns = new List<int>();
                foreach (int s in lines[i].Stops)
                {
                    if (s == index) continue;
                    int t = s > index ? s - 1 : s;
                    if (ns.Count == 0 || ns[ns.Count - 1] != t) ns.Add(t);
                }
                lines[i].Stops = ns;
                if (ns.Count < 2) { lines.RemoveAt(i); removed++; }
            }
            return removed;
        }

        // Số tuyến đang đi qua trạm
        public static int LinesUsing(List<BusLine> lines, int stopIndex)
        {
            int n = 0;
            foreach (BusLine l in lines) if (l.Stops.Contains(stopIndex)) n++;
            return n;
        }

        // Tuyến chiều ngược lại: cùng số tuyến, danh sách trạm đảo ngược
        public static BusLine Reverse(BusLine l)
        {
            BusLine r = CloneLine(l);
            r.Stops.Reverse();
            return r;
        }

        // ----- Ghi / đọc file -----
        private static string Clean(string s)
        {
            return (s ?? "").Replace(',', ' ').Replace(';', ' ').Replace('\n', ' ').Replace('\r', ' ').Trim();
        }

        // stops.csv: lat,lng,node,name (cùng định dạng với bộ nhập dữ liệu OSM)
        public static void SaveStops(List<BusStop> stops, string path)
        {
            var inv = CultureInfo.InvariantCulture;
            var sb = new StringBuilder("lat,lng,node,name\n");
            foreach (BusStop s in stops)
                sb.Append(s.RawLat.ToString("F6", inv)).Append(',').Append(s.RawLng.ToString("F6", inv)).Append(',')
                  .Append(s.Node.ToString(inv)).Append(',').Append(Clean(s.Name)).Append('\n');
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        }

        // lines_admin.csv: ref,name,fare,headway,hours,colour,points   (points = "lat lng;lat lng" của các trạm theo thứ tự)
        public static void SaveLines(List<BusStop> stops, List<BusLine> lines, string path)
        {
            var inv = CultureInfo.InvariantCulture;
            var sb = new StringBuilder("ref,name,fare,headway,hours,colour,points\n");
            foreach (BusLine l in lines)
            {
                sb.Append(Clean(l.Ref)).Append(',').Append(Clean(l.Name)).Append(',').Append(l.Fare).Append(',').Append(l.HeadwayMin)
                  .Append(',').Append(Clean(l.Hours)).Append(',');
                if (l.CustomColor) sb.Append('#').Append(l.Color.R.ToString("X2")).Append(l.Color.G.ToString("X2")).Append(l.Color.B.ToString("X2"));
                sb.Append(',');
                for (int i = 0; i < l.Stops.Count; i++)
                {
                    BusStop s = stops[l.Stops[i]];
                    if (i > 0) sb.Append(';');
                    sb.Append(s.RawLat.ToString("F6", inv)).Append(' ').Append(s.RawLng.ToString("F6", inv));
                }
                sb.Append('\n');
            }
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        }

        // Nạp tuyến admin; trạm được nhận ra bằng đúng tọa độ gốc đã lưu
        public static List<BusLine> LoadLines(RoadGraph g, string path)
        {
            var inv = CultureInfo.InvariantCulture;
            var res = new List<BusLine>();
            string[] rows = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 1; i < rows.Length; i++)
            {
                string[] f = rows[i].Split(new[] { ',' }, 7);
                if (f.Length < 7) continue;
                var l = new BusLine { Ref = f[0], Name = f[1], Hours = f[4] };
                int fare, head;
                if (int.TryParse(f[2], NumberStyles.Integer, inv, out fare)) l.Fare = fare;
                if (int.TryParse(f[3], NumberStyles.Integer, inv, out head) && head > 0) l.HeadwayMin = head;
                if (f[5].Length == 7 && f[5][0] == '#')
                {
                    int rgb;
                    if (int.TryParse(f[5].Substring(1), NumberStyles.HexNumber, inv, out rgb))
                    {
                        l.Color = Color.FromArgb((rgb >> 16) & 255, (rgb >> 8) & 255, rgb & 255);
                        l.CustomColor = true;
                    }
                }
                foreach (string pt in f[6].Split(';'))
                {
                    string[] xy = pt.Split(' ');
                    if (xy.Length != 2) continue;
                    double lat = double.Parse(xy[0], inv), lng = double.Parse(xy[1], inv);
                    int s = FindStop(g, lat, lng);
                    if (s >= 0 && (l.Stops.Count == 0 || l.Stops[l.Stops.Count - 1] != s)) l.Stops.Add(s);
                }
                if (l.Stops.Count >= 2) res.Add(l);
            }
            BusLines.Prepare(g, res);
            return res;
        }

        private static int FindStop(RoadGraph g, double lat, double lng)
        {
            for (int i = 0; i < g.Stops.Count; i++)
                if (Math.Abs(g.Stops[i].RawLat - lat) < 2e-6 && Math.Abs(g.Stops[i].RawLng - lng) < 2e-6) return i;
            return BusLines.MatchStop(g, lat, lng);
        }
    }
}
