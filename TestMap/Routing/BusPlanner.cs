using System;
using System.Collections.Generic;
using System.Drawing;

namespace TestMap.Routing
{
    // Một đoạn của hành trình để vẽ lên bản đồ
    public class MapLeg
    {
        public bool Walk;
        public Color Color;
        public string Ref = "";
        public int FromStop, ToStop;
        public List<LatLng> Points = new List<LatLng>();
    }

    // Một đoạn của hành trình: đi bộ hoặc đi một tuyến xe
    public class TransitLeg
    {
        public bool Walk;
        public BusLine Line;
        public int LineIndex = -1;
        public int FromPos, ToPos;        // vị trí trạm lên / xuống trong Line.Stops
        public int FromStop, ToStop;      // chỉ số trạm trong RoadGraph.Stops
        public double Meters, Seconds;
        public int StopCount;             // số trạm đi qua (chỉ với đoạn đi xe)
    }

    // Một cách đi từ A tới B
    public class Itinerary
    {
        public List<TransitLeg> Legs = new List<TransitLeg>();
        public double Seconds;            // tổng thời gian ước tính, gồm cả chờ xe
        public int Rides;                 // số chuyến xe
        public double WalkMeters, RideMeters;
        public int RideStops;
        public int FareVnd;
        public string Signature = "";

        public int Transfers { get { return Math.Max(0, Rides - 1); } }

        // Trạm lên xe đầu tiên
        public int FirstBoardStop
        {
            get
            {
                foreach (TransitLeg l in Legs) if (!l.Walk) return l.FromStop;
                return -1;
            }
        }

        public List<MapLeg> ToMapLegs(RoadGraph g)
        {
            var res = new List<MapLeg>();
            foreach (TransitLeg l in Legs)
            {
                var m = new MapLeg { Walk = l.Walk, FromStop = l.FromStop, ToStop = l.ToStop };
                if (l.Walk)
                {
                    m.Color = Color.FromArgb(100, 116, 139);
                    m.Points.Add(new LatLng(g.Stops[l.FromStop].Lat, g.Stops[l.FromStop].Lng));
                    m.Points.Add(new LatLng(g.Stops[l.ToStop].Lat, g.Stops[l.ToStop].Lng));
                }
                else
                {
                    m.Color = l.Line.Color;
                    m.Ref = l.Line.Ref;
                    m.Points = l.Line.Path.Slice(l.Line.StopDist[l.FromPos], l.Line.StopDist[l.ToPos]);
                }
                res.Add(m);
            }
            return res;
        }
    }

    // Mạng giao thông công cộng: tuyến xe + các trạm đi bộ được sang nhau. Dựng một lần cho mỗi bộ tuyến.
    public class TransitNet
    {
        // Các giả định (dữ liệu OpenStreetMap không có lịch chạy hay giá vé)
        public const double HeadwaySeconds = 600;    // xe 10 phút một chuyến -> chờ trung bình 5 phút
        public const double BusSpeed = 5.0;          // m/s (18 km/h)
        public const double DwellSeconds = 20;       // dừng mỗi trạm
        public const double WalkSpeed = 1.2;         // m/s
        public const double WalkMax = 250;           // đi bộ sang trạm khác tối đa chừng này (m)
        public const double WalkDetour = 1.3;        // đường đi bộ dài hơn đường chim bay
        public const int FarePerRide = 7000;         // đồng / chuyến

        public RoadGraph G;
        public List<BusLine> Lines;
        public int S;                                // số trạm
        internal int Total;
        internal int[] Offset, LineOf, PosOf;
        internal List<int>[] OccState;               // trạm -> các trạng thái "ngồi trên xe" tương ứng
        internal List<int>[] WalkTo;
        internal List<double>[] WalkDist;

        public static TransitNet Build(RoadGraph g, List<BusLine> lines)
        {
            var n = new TransitNet { G = g, Lines = lines, S = g.Stops.Count };
            n.Offset = new int[lines.Count];
            int tot = 0;
            for (int l = 0; l < lines.Count; l++) { n.Offset[l] = tot; tot += lines[l].Stops.Count; }
            n.Total = tot;
            n.LineOf = new int[tot];
            n.PosOf = new int[tot];
            n.OccState = new List<int>[n.S];
            n.WalkTo = new List<int>[n.S];
            n.WalkDist = new List<double>[n.S];
            for (int s = 0; s < n.S; s++)
            {
                n.OccState[s] = new List<int>();
                n.WalkTo[s] = new List<int>();
                n.WalkDist[s] = new List<double>();
            }
            for (int l = 0; l < lines.Count; l++)
                for (int i = 0; i < lines[l].Stops.Count; i++)
                {
                    int idx = n.Offset[l] + i;
                    n.LineOf[idx] = l;
                    n.PosOf[idx] = i;
                    n.OccState[lines[l].Stops[i]].Add(n.S + idx);
                }
            for (int a = 0; a < n.S; a++)
                for (int b = a + 1; b < n.S; b++)
                {
                    double d = RoadGraph.Distance(new LatLng(g.Stops[a].Lat, g.Stops[a].Lng),
                                                  new LatLng(g.Stops[b].Lat, g.Stops[b].Lng)) * WalkDetour;
                    if (d > WalkMax) continue;
                    n.WalkTo[a].Add(b); n.WalkDist[a].Add(d);
                    n.WalkTo[b].Add(a); n.WalkDist[b].Add(d);
                }
            return n;
        }
    }

    public static class BusPlanner
    {
        // Gợi ý các cách đi từ trạm `src` tới trạm `to`, tối đa `maxRides` chuyến xe, sắp theo thời gian.
        public static List<Itinerary> Plan(TransitNet n, int src, int to, int maxRides)
        {
            var res = new List<Itinerary>();
            if (n == null || n.Lines.Count == 0 || src < 0 || to < 0 || src == to) return res;

            var seen = new HashSet<string>();
            // Hai kiểu ưu tiên: nhanh nhất (đổi tuyến thoải mái) và ít đổi tuyến nhất
            foreach (double penalty in new[] { 60.0, 900.0 })
            {
                var queue = new Queue<bool[]>();
                var bannedSeen = new HashSet<string>();
                queue.Enqueue(new bool[n.Lines.Count]);
                int budget = 10;
                while (queue.Count > 0 && budget-- > 0)
                {
                    bool[] banned = queue.Dequeue();
                    Itinerary it = Search(n, src, to, penalty, banned);
                    if (it == null || it.Rides > maxRides || it.WalkMeters > 1200) continue;
                    if (seen.Add(it.Signature)) res.Add(it);

                    // Tìm phương án khác: cấm lần lượt từng tuyến vừa dùng
                    foreach (TransitLeg leg in it.Legs)
                    {
                        if (leg.Walk) continue;
                        bool[] b2 = (bool[])banned.Clone();
                        b2[leg.LineIndex] = true;
                        if (bannedSeen.Add(string.Join(",", Array.ConvertAll(b2, x => x ? "1" : "0")))) queue.Enqueue(b2);
                    }
                }
            }
            res.Sort((x, y) => x.Seconds.CompareTo(y.Seconds));
            if (res.Count > 6) res.RemoveRange(6, res.Count - 6);
            return res;
        }

        // Các trạng thái của thuật toán (mỗi trạm có 2 loại + mỗi vị trí trên mỗi tuyến):
        //   0..S-1        "chờ xe": đứng ở trạm sau khi đi bộ tới, chỉ được lên xe (không đi bộ tiếp)
        //   S..2S-1       "vừa xuống xe": được lên xe khác, được đi bộ tối đa một đoạn, hoặc kết thúc
        //   2S..          "ngồi trên xe": vừa tới một vị trí của một tuyến bằng xe
        //   2S+Total      điểm bắt đầu (tại trạm đi), 2S+Total+1: đích
        // Ràng buộc như vậy để không có chuyện đi bộ nhiều đoạn liên tiếp hay hành trình không đi chuyến xe nào.
        private static Itinerary Search(TransitNet n, int src, int to, double boardPenalty, bool[] banned)
        {
            int S = n.S, rideBase = 2 * S, start = rideBase + n.Total, goal = start + 1, N = goal + 1;
            var dist = new double[N];
            var prev = new int[N];
            var done = new bool[N];
            for (int i = 0; i < N; i++) { dist[i] = double.PositiveInfinity; prev[i] = -1; }

            var heap = new Heap();
            dist[start] = 0;
            heap.Push(0, start);
            while (heap.Count > 0)
            {
                double key;
                int u = heap.Pop(out key);
                if (done[u]) continue;
                done[u] = true;
                if (u == goal) break;
                double du = dist[u];

                if (u >= rideBase && u < start)
                {
                    // đang ngồi trên xe: đi tiếp tới trạm sau, hoặc xuống ở trạm này
                    int idx = u - rideBase, l = n.LineOf[idx], i = n.PosOf[idx];
                    BusLine line = n.Lines[l];
                    if (i + 1 < line.Stops.Count)
                    {
                        double seg = (line.StopDist[i + 1] - line.StopDist[i]) / TransitNet.BusSpeed + TransitNet.DwellSeconds;
                        Relax(dist, prev, heap, u, u + 1, du + seg);
                    }
                    if (i > 0) Relax(dist, prev, heap, u, S + line.Stops[i], du);
                    continue;
                }

                // đang đứng ở một trạm (điểm bắt đầu, chờ xe, hoặc vừa xuống xe)
                int stop = u == start ? src : (u < S ? u : u - S);
                bool alighted = u >= S && u < rideBase;
                foreach (int rs in n.OccState[stop])
                {
                    int idx = rs - S, l = n.LineOf[idx], i = n.PosOf[idx];
                    if (banned[l] || i + 1 >= n.Lines[l].Stops.Count) continue;
                    // Lên xe và đi ngay tới trạm kế tiếp: mỗi lần lên xe luôn đi qua ít nhất một trạm
                    BusLine bl = n.Lines[l];
                    double first = (bl.StopDist[i + 1] - bl.StopDist[i]) / TransitNet.BusSpeed + TransitNet.DwellSeconds;
                    Relax(dist, prev, heap, u, rideBase + idx + 1, du + TransitNet.HeadwaySeconds / 2 + boardPenalty + first);
                }
                if (u < S) continue;                         // đang chờ xe: không đi bộ tiếp

                if (alighted && stop == to) Relax(dist, prev, heap, u, goal, du);
                for (int k = 0; k < n.WalkTo[stop].Count; k++)
                {
                    int w = n.WalkTo[stop][k];
                    double cost = du + n.WalkDist[stop][k] / TransitNet.WalkSpeed;
                    if (alighted && w == to) Relax(dist, prev, heap, u, goal, cost);   // đi bộ nốt tới đích
                    Relax(dist, prev, heap, u, w, cost);                              // hoặc đi bộ tới trạm khác để lên xe
                }
            }
            if (double.IsPositiveInfinity(dist[goal])) return null;

            // Lần ngược từ đích về điểm bắt đầu
            var states = new List<int>();
            for (int s = goal; s != -1; s = prev[s]) states.Add(s);
            states.Reverse();
            return Decode(n, states, src, to);
        }

        private static void Relax(double[] dist, int[] prev, Heap heap, int u, int v, double cost)
        {
            if (cost < dist[v]) { dist[v] = cost; prev[v] = u; heap.Push(cost, v); }
        }

        // Từ dãy trạng thái đã đi qua, ghép lại thành các đoạn đi bộ / đi xe
        private static Itinerary Decode(TransitNet n, List<int> states, int src, int to)
        {
            int S = n.S, rideBase = 2 * S, start = rideBase + n.Total, goal = start + 1;
            Func<int, int> stopOf = st => st == start ? src : (st == goal ? to : (st < S ? st : st - S));

            var it = new Itinerary();
            TransitLeg cur = null;
            for (int k = 1; k < states.Count; k++)
            {
                int pv = states[k - 1], st = states[k];
                bool pvRide = pv >= rideBase && pv < start, stRide = st >= rideBase && st < start;
                if (!pvRide && stRide)
                {
                    int idx = st - rideBase, l = n.LineOf[idx], i = n.PosOf[idx];
                    // trạng thái "ngồi trên xe" nghĩa là đã tới vị trí i bằng xe, nên trạm lên xe là vị trí i - 1
                    cur = new TransitLeg { Line = n.Lines[l], LineIndex = l, FromPos = i - 1, ToPos = i, FromStop = n.Lines[l].Stops[i - 1] };
                }
                else if (pvRide && stRide)
                {
                    cur.ToPos = n.PosOf[st - rideBase];
                }
                else if (pvRide)
                {
                    cur.ToStop = stopOf(st);
                    cur.Meters = cur.Line.StopDist[cur.ToPos] - cur.Line.StopDist[cur.FromPos];
                    cur.StopCount = cur.ToPos - cur.FromPos;
                    cur.Seconds = cur.Meters / TransitNet.BusSpeed + cur.StopCount * TransitNet.DwellSeconds;
                    it.Legs.Add(cur);
                    cur = null;
                }
                else
                {
                    int a = stopOf(pv), b = stopOf(st);
                    if (a == b) continue;                  // đi bộ 0 m (ví dụ xuống xe rồi tới đích): bỏ qua
                    double d = RoadGraph.Distance(new LatLng(n.G.Stops[a].Lat, n.G.Stops[a].Lng),
                                                  new LatLng(n.G.Stops[b].Lat, n.G.Stops[b].Lng)) * TransitNet.WalkDetour;
                    it.Legs.Add(new TransitLeg { Walk = true, FromStop = a, ToStop = b, Meters = d, Seconds = d / TransitNet.WalkSpeed });
                }
            }

            var sig = new System.Text.StringBuilder();
            foreach (TransitLeg l in it.Legs)
            {
                it.Seconds += l.Seconds;
                if (l.Walk) it.WalkMeters += l.Meters;
                else
                {
                    it.Rides++;
                    it.RideMeters += l.Meters;
                    it.RideStops += l.StopCount;
                    it.Seconds += TransitNet.HeadwaySeconds / 2;      // chờ xe
                    sig.Append("l" + l.LineIndex + ";");        // gộp các phương án cùng dãy tuyến, chỉ khác trạm đổi xe
                }
            }
            it.FareVnd = it.Rides * TransitNet.FarePerRide;
            it.Signature = sig.ToString();
            return it.Rides == 0 ? null : it;
        }

        // Hàng đợi ưu tiên nhỏ gọn (heap nhị phân)
        private class Heap
        {
            private readonly List<double> keys = new List<double>();
            private readonly List<int> vals = new List<int>();
            public int Count { get { return keys.Count; } }

            public void Push(double key, int val)
            {
                keys.Add(key); vals.Add(val);
                int i = keys.Count - 1;
                while (i > 0)
                {
                    int p = (i - 1) / 2;
                    if (keys[p] <= keys[i]) break;
                    Swap(i, p);
                    i = p;
                }
            }

            public int Pop(out double key)
            {
                key = keys[0];
                int val = vals[0];
                int last = keys.Count - 1;
                keys[0] = keys[last]; vals[0] = vals[last];
                keys.RemoveAt(last); vals.RemoveAt(last);
                int i = 0, n = keys.Count;
                while (true)
                {
                    int l = 2 * i + 1, r = l + 1, m = i;
                    if (l < n && keys[l] < keys[m]) m = l;
                    if (r < n && keys[r] < keys[m]) m = r;
                    if (m == i) break;
                    Swap(i, m);
                    i = m;
                }
                return val;
            }

            private void Swap(int a, int b)
            {
                double k = keys[a]; keys[a] = keys[b]; keys[b] = k;
                int v = vals[a]; vals[a] = vals[b]; vals[b] = v;
            }
        }
    }
}
