using System.Collections.Generic;
using System.Diagnostics;

namespace TestMap.Routing
{
    // Kết quả một lần tìm đường
    public class SearchResult
    {
        public bool Found;                                           // có tìm được đường không
        public double Distance;                                      // tổng chiều dài đường đi (mét)
        public double ElapsedMs;                                     // thời gian chạy thuật toán

        public List<int> Visited = new List<int>();                  // các giao lộ theo thứ tự được xét
        public List<int> VisitedViaEdge = new List<int>();           // đoạn đường dùng để tới giao lộ đó (-1 với điểm xuất phát)
        public List<double> VisitedCost = new List<double>();        // g: quãng đường ngắn nhất từ điểm đi tới giao lộ vừa chốt (m)
        public List<int> HeapAtPop = new List<int>();                // số phần tử còn trong hàng đợi ngay sau khi lấy giao lộ đó ra
        public bool AStar;                                           // true: A*, false: Dijkstra

        public List<int> PathNodes = new List<int>();                // các giao lộ trên đường đi, từ đầu tới đích
        public List<int> PathEdges = new List<int>();                // các đoạn đường trên đường đi, theo thứ tự
    }

    public static class PathFinder
    {
        // useAStar = false: Dijkstra. useAStar = true: A* (thêm ước lượng khoảng cách chim bay tới đích).
        public static SearchResult Find(RoadGraph g, int start, int goal, bool useAStar)
        {
            var sw = Stopwatch.StartNew();
            var result = new SearchResult { AStar = useAStar };

            int n = g.Nodes.Count;
            double[] cost = new double[n];      // quãng đường ngắn nhất đã biết từ start
            int[] prevNode = new int[n];        // giao lộ đứng trước
            int[] prevEdge = new int[n];        // đoạn đường đi tới giao lộ này
            bool[] closed = new bool[n];        // đã chốt xong chưa
            for (int i = 0; i < n; i++) { cost[i] = double.PositiveInfinity; prevNode[i] = -1; prevEdge[i] = -1; }

            var heap = new MinHeap();
            cost[start] = 0;
            heap.Push(useAStar ? H(g, start, goal) : 0, start);

            while (heap.Count > 0)
            {
                int u = heap.PopNode();
                if (closed[u]) continue;            // bản ghi cũ trong heap, bỏ qua
                closed[u] = true;

                result.Visited.Add(u);
                result.VisitedViaEdge.Add(prevEdge[u]);
                result.VisitedCost.Add(cost[u]);
                result.HeapAtPop.Add(heap.Count);
                if (u == goal) break;               // tới đích rồi

                foreach (int ei in g.Adj[u])
                {
                    if (g.IsBlocked(ei)) continue;      // đường đang bị chặn: coi như không tồn tại
                    int v = g.OtherEnd(ei, u);
                    if (closed[v]) continue;

                    double nc = cost[u] + g.Edges[ei].Length;
                    if (nc < cost[v])
                    {
                        cost[v] = nc;
                        prevNode[v] = u;
                        prevEdge[v] = ei;
                        heap.Push(nc + (useAStar ? H(g, v, goal) : 0), v);
                    }
                }
            }

            if (!double.IsPositiveInfinity(cost[goal]))
            {
                result.Found = true;
                result.Distance = cost[goal];
                for (int v = goal; v != -1; v = prevNode[v])
                {
                    result.PathNodes.Insert(0, v);
                    if (prevEdge[v] != -1) result.PathEdges.Insert(0, prevEdge[v]);
                }
            }

            sw.Stop();
            result.ElapsedMs = sw.Elapsed.TotalMilliseconds;
            return result;
        }

        // Ước lượng còn bao xa tới đích: khoảng cách chim bay (không bao giờ dài hơn đường thật)
        private static double H(RoadGraph g, int a, int b)
        {
            return RoadGraph.Distance(g.Nodes[a], g.Nodes[b]);
        }

        // Hàng đợi ưu tiên (min-heap): lấy ra phần tử có Key nhỏ nhất
        private class MinHeap
        {
            private readonly List<double> keys = new List<double>();
            private readonly List<int> vals = new List<int>();

            public int Count { get { return keys.Count; } }

            public void Push(double key, int node)
            {
                keys.Add(key);
                vals.Add(node);
                int i = keys.Count - 1;
                while (i > 0)
                {
                    int p = (i - 1) / 2;
                    if (keys[p] <= keys[i]) break;
                    Swap(i, p);
                    i = p;
                }
            }

            public int PopNode()
            {
                int top = vals[0];
                int last = keys.Count - 1;
                keys[0] = keys[last];
                vals[0] = vals[last];
                keys.RemoveAt(last);
                vals.RemoveAt(last);

                int i = 0;
                while (true)
                {
                    int l = 2 * i + 1, r = l + 1, m = i;
                    if (l < keys.Count && keys[l] < keys[m]) m = l;
                    if (r < keys.Count && keys[r] < keys[m]) m = r;
                    if (m == i) break;
                    Swap(i, m);
                    i = m;
                }
                return top;
            }

            private void Swap(int a, int b)
            {
                double tk = keys[a]; keys[a] = keys[b]; keys[b] = tk;
                int tv = vals[a]; vals[a] = vals[b]; vals[b] = tv;
            }
        }
    }
}
