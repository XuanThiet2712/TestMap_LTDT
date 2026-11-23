using System;
using System.Collections.Generic;

namespace TestMap.Routing
{
    // Đường đi tìm được, ghép lại thành MỘT dãy điểm liên tục (đã tính cả các điểm uốn của từng đoạn đường).
    // Dùng để: vẽ đường đi, vẽ dần khi hiện đường, chạy xe mô phỏng, và biết mỗi trạm cách điểm xuất phát bao xa.
    public class RoutePath
    {
        public readonly List<LatLng> Points = new List<LatLng>();
        public readonly List<double> Cum = new List<double>();       // Cum[i] = quãng đường (m) từ điểm đầu tới Points[i]
        public readonly List<double> NodeDist = new List<double>();  // NodeDist[i] = quãng đường (m) tới giao lộ thứ i của PathNodes
        public double Total;

        public static RoutePath Build(RoadGraph g, SearchResult r)
        {
            var rp = new RoutePath();
            if (r == null || !r.Found || r.PathNodes.Count == 0) return rp;

            rp.Add(g.Nodes[r.PathNodes[0]]);
            rp.NodeDist.Add(0);
            rp.AppendEdges(g, r, true);
            return rp;
        }

        // Nối thêm một điểm vào cuối
        public void AppendPoint(LatLng p) { Add(p); }

        // Nối một kết quả tìm đường vào cuối. Điểm đầu của kết quả phải trùng điểm cuối hiện tại.
        public void AppendResult(RoadGraph g, SearchResult r)
        {
            if (r == null || !r.Found) return;
            AppendEdges(g, r, false);
        }

        private void AppendEdges(RoadGraph g, SearchResult r, bool recordNodes)
        {
            for (int i = 0; i < r.PathEdges.Count; i++)
            {
                int u = r.PathNodes[i];
                RoadEdge e = g.Edges[r.PathEdges[i]];
                bool forward = e.From == u;          // đi xuôi hay ngược chiều lưu trữ của đoạn đường
                if (e.Geometry != null)
                {
                    if (forward)
                        for (int k = 0; k < e.Geometry.Length; k++) Add(e.Geometry[k]);
                    else
                        for (int k = e.Geometry.Length - 1; k >= 0; k--) Add(e.Geometry[k]);
                }
                Add(g.Nodes[r.PathNodes[i + 1]]);
                if (recordNodes) NodeDist.Add(Total);
            }
        }

        private void Add(LatLng p)
        {
            if (Points.Count > 0) Total += RoadGraph.Distance(Points[Points.Count - 1], p);
            Points.Add(p);
            Cum.Add(Total);
        }

        // Vị trí của điểm cách điểm đầu `meters` mét dọc theo đường đi
        public LatLng PointAt(double meters)
        {
            if (Points.Count == 0) return new LatLng();
            if (meters <= 0) return Points[0];
            if (meters >= Total) return Points[Points.Count - 1];

            int lo = 0, hi = Cum.Count - 1;
            while (hi - lo > 1)
            {
                int mid = (lo + hi) / 2;
                if (Cum[mid] <= meters) lo = mid; else hi = mid;
            }
            double seg = Cum[hi] - Cum[lo];
            double t = seg > 0 ? (meters - Cum[lo]) / seg : 0;
            return new LatLng(Points[lo].Lat + (Points[hi].Lat - Points[lo].Lat) * t,
                              Points[lo].Lng + (Points[hi].Lng - Points[lo].Lng) * t);
        }

        // Đoạn đường nằm giữa hai vị trí (tính bằng mét từ điểm đầu)
        public List<LatLng> Slice(double d0, double d1)
        {
            var res = new List<LatLng>();
            if (Points.Count == 0) return res;
            if (d1 < d0) { double t = d0; d0 = d1; d1 = t; }
            d0 = Math.Max(0, d0);
            d1 = Math.Min(Total, d1);
            res.Add(PointAt(d0));
            for (int i = 0; i < Points.Count; i++)
                if (Cum[i] > d0 && Cum[i] < d1) res.Add(Points[i]);
            res.Add(PointAt(d1));
            return res;
        }

        public void GetBounds(out double minLat, out double minLng, out double maxLat, out double maxLng)
        {
            minLat = minLng = double.MaxValue;
            maxLat = maxLng = double.MinValue;
            foreach (LatLng p in Points)
            {
                if (p.Lat < minLat) minLat = p.Lat;
                if (p.Lat > maxLat) maxLat = p.Lat;
                if (p.Lng < minLng) minLng = p.Lng;
                if (p.Lng > maxLng) maxLng = p.Lng;
            }
        }
    }
}
