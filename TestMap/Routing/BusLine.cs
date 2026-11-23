using System.Collections.Generic;
using System.Drawing;

namespace TestMap.Routing
{
    // Một tuyến xe buýt theo MỘT chiều: danh sách trạm theo thứ tự + đường xe chạy qua các trạm đó.
    // (Tuyến 2 chiều là 2 BusLine cùng Ref, giống cách OpenStreetMap lưu.)
    public class BusLine
    {
        public string Ref = "";            // số tuyến, ví dụ "06"
        public string Name = "";
        public Color Color = Color.FromArgb(37, 99, 235);
        public bool CustomColor;           // true: màu do admin chọn, false: tự gán theo số tuyến
        public bool Demo;                  // tuyến tự tạo để demo, không phải tuyến thật
        public int Fare = 7000;            // giá vé một chuyến (đồng)
        public int HeadwayMin = 10;        // cứ bao nhiêu phút có một chuyến (để ước tính thời gian chờ)
        public string Hours = "";          // giờ hoạt động, ví dụ "05:00 - 21:00" (chỉ hiển thị)
        public double Phase;               // 0..1, lệch pha để các tuyến không có xe xuất phát cùng lúc

        public List<int> Stops = new List<int>();            // chỉ số trạm trong RoadGraph.Stops, theo thứ tự
        public RoutePath Path = new RoutePath();             // đường xe chạy (bám theo đường trên bản đồ)
        public List<double> StopDist = new List<double>();   // StopDist[i] = quãng đường (m) từ trạm đầu tới trạm thứ i

        public double MinLat, MinLng, MaxLat, MaxLng;        // khung bao, để bỏ qua tuyến nằm ngoài màn hình

        public double Length { get { return Path.Total; } }

        // Nối các trạm bằng đường ngắn nhất trên mạng đường. Đoạn nào không tìm được đường thì nối thẳng.
        public void BuildPath(RoadGraph g)
        {
            Path = new RoutePath();
            StopDist = new List<double>();
            if (Stops.Count < 2) return;

            Path.AppendPoint(g.Nodes[g.Stops[Stops[0]].Node]);
            StopDist.Add(0);
            for (int i = 1; i < Stops.Count; i++)
            {
                int a = g.Stops[Stops[i - 1]].Node, b = g.Stops[Stops[i]].Node;
                if (a != b)
                {
                    SearchResult r = PathFinder.Find(g, a, b, true);
                    if (r.Found) Path.AppendResult(g, r);
                    else Path.AppendPoint(g.Nodes[b]);
                }
                StopDist.Add(Path.Total);
            }
            Path.GetBounds(out MinLat, out MinLng, out MaxLat, out MaxLng);
        }

        public string Title(RoadGraph g)
        {
            if (Stops.Count == 0) return Name;
            return g.Stops[Stops[0]].Name + " → " + g.Stops[Stops[Stops.Count - 1]].Name;
        }
    }
}
