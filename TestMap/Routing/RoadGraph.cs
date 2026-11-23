using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TestMap.Routing
{
    // Một tọa độ (không phụ thuộc thư viện bản đồ)
    public struct LatLng
    {
        public double Lat, Lng;
        public LatLng(double lat, double lng) { Lat = lat; Lng = lng; }
    }

    // Một đoạn đường nối 2 giao lộ
    public class RoadEdge
    {
        public int From, To;        // chỉ số giao lộ đầu, cuối
        public double Length;       // chiều dài (mét)
        public bool OneWay;         // true: chỉ đi được từ From sang To
        public LatLng[] Geometry;   // các điểm uốn nằm giữa From và To (có thể null = đường thẳng)
        public int Class;           // cấp đường: 0 hẻm/đường nhỏ, 1 đường vừa, 2 đường lớn, 3 trục chính
        public double MinLat, MaxLat, MinLng, MaxLng; // khung bao của đoạn đường (để bỏ qua đoạn ngoài màn hình)
    }

    // Một trạm xe buýt
    public class BusStop
    {
        public double Lat, Lng;         // vị trí dùng để vẽ: nằm ĐÚNG trên đường (trùng với giao lộ mà trạm gắn vào)
        public double RawLat, RawLng;   // vị trí gốc trong OpenStreetMap (thường nằm trên vỉa hè, lệch đường vài chục mét)
        public int Node;                // giao lộ trong mạng đường mà trạm được gắn vào
        public string Name;

        public override string ToString() { return Name; }
    }

    // Mạng đường: giao lộ (nodes) + đoạn đường (edges)
    public class RoadGraph
    {
        public readonly List<LatLng> Nodes = new List<LatLng>();
        public readonly List<RoadEdge> Edges = new List<RoadEdge>();

        // Adj[u] = danh sách CHỈ SỐ các đoạn đường có thể đi ra từ giao lộ u (đã tính đường một chiều)
        public List<int>[] Adj;

        public readonly List<BusStop> Stops = new List<BusStop>();
        public List<int>[] StopsAtNode;     // StopsAtNode[n] = các trạm gắn vào giao lộ n (null nếu không có)
        public List<int>[] EdgesByClass;    // chỉ số các đoạn đường theo cấp đường 0..3 (để vẽ đường lớn đè lên đường nhỏ)

        // ===== Đọc từ 2 file CSV =====
        // nodes.csv: dòng đầu là tiêu đề "lat,lng"; mỗi dòng sau là 1 giao lộ, chỉ số = thứ tự dòng (bắt đầu từ 0)
        // edges.csv: dòng đầu là tiêu đề "from,to,length,oneway,geometry"
        //            geometry = "lat lng;lat lng;..." (có thể để trống)
        public static RoadGraph Load(string nodesPath, string edgesPath)
        {
            var g = new RoadGraph();
            var inv = CultureInfo.InvariantCulture;

            bool first = true;
            foreach (string line in File.ReadLines(nodesPath))
            {
                if (first) { first = false; continue; }
                if (line.Length == 0) continue;
                string[] p = line.Split(',');
                g.Nodes.Add(new LatLng(double.Parse(p[0], inv), double.Parse(p[1], inv)));
            }

            first = true;
            foreach (string line in File.ReadLines(edgesPath))
            {
                if (first) { first = false; continue; }
                if (line.Length == 0) continue;
                string[] p = line.Split(',');

                var e = new RoadEdge();
                e.From = int.Parse(p[0], inv);
                e.To = int.Parse(p[1], inv);
                if (e.From == e.To) continue;                       // bỏ đoạn đường khép kín vào chính nó
                if (e.From < 0 || e.To < 0 || e.From >= g.Nodes.Count || e.To >= g.Nodes.Count) continue;

                e.Length = double.Parse(p[2], inv);
                e.OneWay = p[3].Trim() == "1";

                if (p.Length > 4 && p[4].Length > 0)
                {
                    string[] pts = p[4].Split(';');
                    var geo = new LatLng[pts.Length];
                    for (int i = 0; i < pts.Length; i++)
                    {
                        string[] ll = pts[i].Split(' ');
                        geo[i] = new LatLng(double.Parse(ll[0], inv), double.Parse(ll[1], inv));
                    }
                    e.Geometry = geo;
                }
                if (p.Length > 5)
                {
                    int cls;
                    if (int.TryParse(p[5], NumberStyles.Integer, inv, out cls)) e.Class = Math.Max(0, Math.Min(3, cls));
                }
                g.Edges.Add(e);
            }

            g.BuildAdjacency();
            return g;
        }

        // stops.csv: dòng đầu "lat,lng,node,name"
        public void LoadStops(string stopsPath)
        {
            Stops.Clear();
            var inv = CultureInfo.InvariantCulture;
            if (File.Exists(stopsPath))
            {
                bool first = true;
                foreach (string line in File.ReadLines(stopsPath))
                {
                    if (first) { first = false; continue; }
                    if (line.Length == 0) continue;
                    string[] p = line.Split(new[] { ',' }, 4);
                    if (p.Length < 4) continue;
                    int node = int.Parse(p[2], inv);
                    if (node < 0 || node >= Nodes.Count) continue;
                    // Vẽ trạm tại chính giao lộ nó gắn vào: đường đi đi qua giao lộ này nên trạm luôn nằm đúng trên tuyến
                    // (trước đây vẽ ở vị trí gốc trên vỉa hè nên bị lệch khỏi đường đi tới 50-60 m).
                    Stops.Add(new BusStop
                    {
                        RawLat = double.Parse(p[0], inv),
                        RawLng = double.Parse(p[1], inv),
                        Lat = Nodes[node].Lat,
                        Lng = Nodes[node].Lng,
                        Node = node,
                        Name = p[3]
                    });
                }
            }

            StopsAtNode = new List<int>[Nodes.Count];
            for (int i = 0; i < Stops.Count; i++)
            {
                int n = Stops[i].Node;
                if (StopsAtNode[n] == null) StopsAtNode[n] = new List<int>();
                StopsAtNode[n].Add(i);
            }
        }

        public void BuildAdjacency()
        {
            Adj = new List<int>[Nodes.Count];
            for (int i = 0; i < Adj.Length; i++) Adj[i] = new List<int>();
            StopsAtNode = new List<int>[Nodes.Count];

            EdgesByClass = new List<int>[4];
            for (int c = 0; c < 4; c++) EdgesByClass[c] = new List<int>();

            for (int i = 0; i < Edges.Count; i++)
            {
                RoadEdge e = Edges[i];
                Adj[e.From].Add(i);                 // luôn đi được từ From sang To
                if (!e.OneWay) Adj[e.To].Add(i);    // đường hai chiều thì đi ngược lại cũng được
                EdgesByClass[Math.Max(0, Math.Min(3, e.Class))].Add(i);

                // khung bao của đoạn đường
                LatLng a = Nodes[e.From], b = Nodes[e.To];
                e.MinLat = Math.Min(a.Lat, b.Lat); e.MaxLat = Math.Max(a.Lat, b.Lat);
                e.MinLng = Math.Min(a.Lng, b.Lng); e.MaxLng = Math.Max(a.Lng, b.Lng);
                if (e.Geometry != null)
                {
                    foreach (LatLng p in e.Geometry)
                    {
                        if (p.Lat < e.MinLat) e.MinLat = p.Lat;
                        if (p.Lat > e.MaxLat) e.MaxLat = p.Lat;
                        if (p.Lng < e.MinLng) e.MinLng = p.Lng;
                        if (p.Lng > e.MaxLng) e.MaxLng = p.Lng;
                    }
                }
            }
        }

        // Trạm xe buýt gần một tọa độ nhất (-1 nếu không có trạm nào)
        public int NearestStop(double lat, double lng)
        {
            double k = Math.Cos(lat * Math.PI / 180);
            int best = -1;
            double bestD = double.MaxValue;
            for (int i = 0; i < Stops.Count; i++)
            {
                double dy = Stops[i].Lat - lat;
                double dx = (Stops[i].Lng - lng) * k;
                double d = dx * dx + dy * dy;
                if (d < bestD) { bestD = d; best = i; }
            }
            return best;
        }

        // Giao lộ ở đầu kia của đoạn đường edgeIndex khi đứng ở giao lộ u
        public int OtherEnd(int edgeIndex, int u)
        {
            RoadEdge e = Edges[edgeIndex];
            return e.From == u ? e.To : e.From;
        }

        // Tìm giao lộ gần một tọa độ nhất (quét tuyến tính, đủ nhanh cho vài trăm nghìn điểm)
        public int Nearest(double lat, double lng)
        {
            double k = Math.Cos(lat * Math.PI / 180); // thu nhỏ trục kinh độ cho gần đúng khoảng cách thật
            int best = -1;
            double bestD = double.MaxValue;
            for (int i = 0; i < Nodes.Count; i++)
            {
                double dy = Nodes[i].Lat - lat;
                double dx = (Nodes[i].Lng - lng) * k;
                double d = dx * dx + dy * dy;
                if (d < bestD) { bestD = d; best = i; }
            }
            return best;
        }

        // Khung bao quanh toàn bộ mạng đường
        public void GetBounds(out double minLat, out double minLng, out double maxLat, out double maxLng)
        {
            minLat = minLng = double.MaxValue;
            maxLat = maxLng = double.MinValue;
            foreach (LatLng p in Nodes)
            {
                if (p.Lat < minLat) minLat = p.Lat;
                if (p.Lat > maxLat) maxLat = p.Lat;
                if (p.Lng < minLng) minLng = p.Lng;
                if (p.Lng > maxLng) maxLng = p.Lng;
            }
        }

        // Khoảng cách giữa 2 tọa độ theo mét (công thức Haversine)
        public static double Distance(LatLng a, LatLng b)
        {
            const double R = 6371000;
            double dLat = (b.Lat - a.Lat) * Math.PI / 180;
            double dLng = (b.Lng - a.Lng) * Math.PI / 180;
            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(a.Lat * Math.PI / 180) * Math.Cos(b.Lat * Math.PI / 180) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
        }
    }
}
