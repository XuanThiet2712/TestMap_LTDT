using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using TestMap.Routing;

namespace TestMap.Mapping
{
    // Bản đồ đơn giản: nền trơn + đường + trạm xe buýt, tự vẽ bằng GDI+ từ chính dữ liệu đường,
    // nên đường đi luôn bám đúng đường vẽ trên bản đồ.
    //   1. đường (đường lớn dày hơn, có viền)
    //   2. các đoạn đường thuật toán đã xét (xanh dương)
    //   3. đường đi tìm được (cam, có viền trắng)
    //   4. trạm xe buýt, tên trạm, điểm đi / điểm đến
    public class MapCanvas : GMapControl
    {
        // ----- Dữ liệu để vẽ (Form gán vào) -----
        public RoadGraph RoadNet;
        public SearchResult LastResult;
        public int VisitedShown;                 // phát lại tới bước thứ mấy (hoạt ảnh loang)
        public bool ShowPath;                    // true thì vẽ đường đi cuối cùng
        public bool ShowStops = true;            // bật/tắt trạm xe buýt
        public int StartNode = -1, EndNode = -1; // giao lộ điểm đi / đến
        public int StartStop = -1, EndStop = -1; // trạm điểm đi / đến (-1 nếu chọn theo giao lộ)
        public List<int> RouteStops = new List<int>(); // các trạm nằm trên đường đi, theo thứ tự
        public string Attribution = "Dữ liệu © OpenStreetMap contributors";

        // ----- Bảng màu -----
        public static readonly Color BackgroundColor = Color.FromArgb(238, 242, 246);
        private static readonly Color[] RoadFill =
            { Color.White, Color.White, Color.White, Color.FromArgb(255, 244, 214) };
        private static readonly Color[] RoadCasing =
            { Color.FromArgb(208, 215, 225), Color.FromArgb(196, 204, 216), Color.FromArgb(180, 190, 205), Color.FromArgb(240, 190, 100) };
        private static readonly float[] RoadWidth = { 2.2f, 3.2f, 4.4f, 6.0f };

        private static readonly Color VisitedColor = Color.FromArgb(210, 59, 130, 246);
        private static readonly Color PathColor = Color.FromArgb(234, 88, 12);
        private static readonly Color StopColor = Color.FromArgb(29, 78, 216);
        private static readonly Color StartColor = Color.FromArgb(22, 163, 74);
        private static readonly Color EndColor = Color.FromArgb(220, 38, 38);

        private static readonly Font StopFont = new Font("Segoe UI", 8.5f);
        private static readonly Font RouteFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font PinFont = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font SmallFont = new Font("Segoe UI", 8f);

        private int hoverStop = -1;

        private static Pen RoundPen(Color c, float w)
        {
            var p = new Pen(c, w);
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;
            p.LineJoin = LineJoin.Round;
            return p;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // GMap vẽ nền trơn trước

            if (RoadNet == null || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            Graphics g = e.Graphics;
            g.ResetTransform();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            RectLatLng view = ViewArea;

            float scale = (float)Math.Max(0.6, Math.Min(2.6, 1 + (Zoom - 15) * 0.35));
            int minClass = Zoom < 13 ? 2 : (Zoom < 14 ? 1 : 0);   // zoom xa thì bỏ hẻm và đường nhỏ cho nhẹ

            // 1. Đường: vẽ viền trước, rồi vẽ ruột, để các ngã tư nối liền đẹp
            for (int c = minClass; c < 4; c++)
                using (Pen pen = RoundPen(RoadCasing[c], RoadWidth[c] * scale + 2.2f))
                    DrawEdges(g, pen, RoadNet.EdgesByClass[c], view);
            for (int c = minClass; c < 4; c++)
                using (Pen pen = RoundPen(RoadFill[c], RoadWidth[c] * scale))
                    DrawEdges(g, pen, RoadNet.EdgesByClass[c], view);

            if (LastResult != null)
            {
                // 2. Các đoạn đường đã xét
                using (Pen pen = RoundPen(VisitedColor, 3.2f * scale + 1f))
                {
                    int n = Math.Min(VisitedShown, LastResult.VisitedViaEdge.Count);
                    for (int i = 0; i < n; i++)
                    {
                        int ei = LastResult.VisitedViaEdge[i];
                        if (ei >= 0) DrawEdge(g, pen, ei, view);
                    }
                }

                // 3. Đường đi cuối cùng
                if (ShowPath && LastResult.Found)
                {
                    using (Pen casing = RoundPen(Color.White, 8f * scale + 4f))
                        DrawEdges(g, casing, LastResult.PathEdges, view);
                    using (Pen pen = RoundPen(PathColor, 8f * scale))
                        DrawEdges(g, pen, LastResult.PathEdges, view);
                }
            }

            // 4. Trạm xe buýt
            var onRoute = new HashSet<int>();
            if (ShowPath && LastResult != null && LastResult.Found)
                foreach (int s in RouteStops) onRoute.Add(s);

            var used = new List<RectangleF>();       // các ô chữ đã chiếm chỗ, để nhãn không đè nhau
            var normalLabels = new List<KeyValuePair<string, PointF>>();
            var routeLabels = new List<KeyValuePair<string, PointF>>();

            if (ShowStops)
            {
                float r = Zoom >= 16 ? 5.5f : 4f;
                using (var fill = new SolidBrush(StopColor))
                using (var border = new Pen(Color.White, 1.6f))
                    for (int i = 0; i < RoadNet.Stops.Count; i++)
                    {
                        if (onRoute.Contains(i)) continue;
                        BusStop s = RoadNet.Stops[i];
                        if (!InView(s.Lat, s.Lng, view)) continue;
                        PointF p = ToScreen(s.Lat, s.Lng);
                        g.FillEllipse(fill, p.X - r, p.Y - r, 2 * r, 2 * r);
                        g.DrawEllipse(border, p.X - r, p.Y - r, 2 * r, 2 * r);
                        if (Zoom >= 16) normalLabels.Add(new KeyValuePair<string, PointF>(s.Name, p));
                    }
            }

            // Trạm nằm trên đường đi: to hơn, màu cam
            using (var fill = new SolidBrush(PathColor))
            using (var border = new Pen(Color.White, 2f))
                foreach (int si in onRoute)
                {
                    BusStop s = RoadNet.Stops[si];
                    PointF p = ToScreen(s.Lat, s.Lng);
                    g.FillEllipse(fill, p.X - 7, p.Y - 7, 14, 14);
                    g.DrawEllipse(border, p.X - 7, p.Y - 7, 14, 14);
                    routeLabels.Add(new KeyValuePair<string, PointF>(s.Name, p));
                }

            // Điểm đi, điểm đến
            PointF startP = new PointF(), endP = new PointF();
            bool hasStart = StartNode >= 0, hasEnd = EndNode >= 0;
            if (hasStart) startP = PointOf(StartStop, StartNode);
            if (hasEnd) endP = PointOf(EndStop, EndNode);
            if (hasStart) DrawPin(g, startP, "A", StartColor);
            if (hasEnd) DrawPin(g, endP, "B", EndColor);

            // Nhãn: điểm đi/đến trước, rồi trạm trên đường đi, cuối cùng các trạm còn lại
            if (hasStart) DrawCard(g, StopName(StartStop, "Điểm đi"), startP, used, StartColor);
            if (hasEnd) DrawCard(g, StopName(EndStop, "Điểm đến"), endP, used, EndColor);
            if (Zoom >= 14)
                foreach (var kv in routeLabels) DrawLabel(g, kv.Key, kv.Value, RouteFont, used);
            foreach (var kv in normalLabels) DrawLabel(g, kv.Key, kv.Value, StopFont, used);

            // Tên trạm khi rê chuột
            if (ShowStops && hoverStop >= 0 && hoverStop < RoadNet.Stops.Count)
            {
                BusStop s = RoadNet.Stops[hoverStop];
                PointF p = ToScreen(s.Lat, s.Lng);
                SizeF ts = g.MeasureString(s.Name, PinFont);
                var rc = new RectangleF(p.X - ts.Width / 2 - 8, p.Y - ts.Height - 18, ts.Width + 16, ts.Height + 6);
                using (var bg = new SolidBrush(Color.FromArgb(235, 15, 23, 42)))
                    g.FillRectangle(bg, rc);
                g.DrawString(s.Name, PinFont, Brushes.White, rc.X + 8, rc.Y + 3);
            }

            // Ghi nguồn dữ liệu (yêu cầu của giấy phép OpenStreetMap)
            if (!string.IsNullOrEmpty(Attribution))
            {
                SizeF s = g.MeasureString(Attribution, SmallFont);
                using (var bg = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                    g.FillRectangle(bg, Width - s.Width - 8, Height - s.Height - 6, s.Width + 6, s.Height + 2);
                g.DrawString(Attribution, SmallFont, Brushes.DimGray, Width - s.Width - 5, Height - s.Height - 5);
            }
        }

        // ----- Rê chuột lên trạm: hiện tên -----
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (RoadNet == null || !ShowStops || e.Button != MouseButtons.None) return;

            int best = -1;
            double bestD = 12 * 12;
            for (int i = 0; i < RoadNet.Stops.Count; i++)
            {
                BusStop s = RoadNet.Stops[i];
                GPoint p = FromLatLngToLocal(new PointLatLng(s.Lat, s.Lng));
                double dx = p.X - e.X, dy = p.Y - e.Y;
                double d = dx * dx + dy * dy;
                if (d < bestD) { bestD = d; best = i; }
            }
            if (best != hoverStop)
            {
                hoverStop = best;
                Invalidate();
            }
        }

        // ----- Hàm phụ -----
        private PointF ToScreen(double lat, double lng)
        {
            GPoint s = FromLatLngToLocal(new PointLatLng(lat, lng));
            return new PointF(s.X, s.Y);
        }

        private PointF PointOf(int stop, int node)
        {
            if (stop >= 0 && stop < RoadNet.Stops.Count) return ToScreen(RoadNet.Stops[stop].Lat, RoadNet.Stops[stop].Lng);
            return ToScreen(RoadNet.Nodes[node].Lat, RoadNet.Nodes[node].Lng);
        }

        private string StopName(int stop, string fallback)
        {
            return stop >= 0 && stop < RoadNet.Stops.Count ? RoadNet.Stops[stop].Name : fallback;
        }

        private static bool InView(double lat, double lng, RectLatLng v)
        {
            return lat <= v.Top && lat >= v.Bottom && lng >= v.Left && lng <= v.Right;
        }

        private void DrawEdges(Graphics g, Pen pen, List<int> list, RectLatLng view)
        {
            for (int i = 0; i < list.Count; i++) DrawEdge(g, pen, list[i], view);
        }

        // Vẽ một đoạn đường (kể cả điểm uốn). Đoạn nằm hoàn toàn ngoài màn hình thì bỏ qua.
        private void DrawEdge(Graphics g, Pen pen, int edgeIndex, RectLatLng view)
        {
            RoadEdge e = RoadNet.Edges[edgeIndex];
            if (e.MaxLat < view.Bottom || e.MinLat > view.Top || e.MaxLng < view.Left || e.MinLng > view.Right) return;

            LatLng a = RoadNet.Nodes[e.From];
            LatLng b = RoadNet.Nodes[e.To];
            if (e.Geometry == null || e.Geometry.Length == 0)
            {
                g.DrawLine(pen, ToScreen(a.Lat, a.Lng), ToScreen(b.Lat, b.Lng));
                return;
            }

            var pts = new List<PointF>(e.Geometry.Length + 2);
            pts.Add(ToScreen(a.Lat, a.Lng));
            foreach (LatLng p in e.Geometry) pts.Add(ToScreen(p.Lat, p.Lng));
            pts.Add(ToScreen(b.Lat, b.Lng));
            g.DrawLines(pen, pts.ToArray());
        }

        // Chấm tròn có chữ A / B
        private void DrawPin(Graphics g, PointF c, string letter, Color color)
        {
            using (var glow = new SolidBrush(Color.FromArgb(70, color)))
                g.FillEllipse(glow, c.X - 16, c.Y - 16, 32, 32);
            using (var fill = new SolidBrush(color))
                g.FillEllipse(fill, c.X - 10, c.Y - 10, 20, 20);
            using (var border = new Pen(Color.White, 2f))
                g.DrawEllipse(border, c.X - 10, c.Y - 10, 20, 20);
            SizeF ts = g.MeasureString(letter, PinFont);
            g.DrawString(letter, PinFont, Brushes.White, c.X - ts.Width / 2 + 0.5f, c.Y - ts.Height / 2 + 0.5f);
        }

        // Thẻ tên có nền màu, đặt dưới điểm đi/đến
        private void DrawCard(Graphics g, string text, PointF c, List<RectangleF> used, Color color)
        {
            SizeF ts = g.MeasureString(text, PinFont);
            var r = new RectangleF(c.X - ts.Width / 2 - 8, c.Y + 18, ts.Width + 16, ts.Height + 6);
            used.Add(r);
            using (var bg = new SolidBrush(color))
                g.FillRectangle(bg, r);
            g.DrawString(text, PinFont, Brushes.White, r.X + 8, r.Y + 3);
        }

        // Tên trạm bên cạnh chấm; bỏ qua nếu đè lên nhãn khác
        private void DrawLabel(Graphics g, string text, PointF c, Font font, List<RectangleF> used)
        {
            SizeF ts = g.MeasureString(text, font);
            var r = new RectangleF(c.X + 9, c.Y - ts.Height / 2, ts.Width, ts.Height);
            foreach (RectangleF u in used)
                if (u.IntersectsWith(r)) return;
            used.Add(r);

            // viền trắng quanh chữ cho dễ đọc trên nền đường
            for (int dx = -1; dx <= 1; dx += 2)
                for (int dy = -1; dy <= 1; dy += 2)
                    g.DrawString(text, font, Brushes.White, r.X + dx, r.Y + dy);
            g.DrawString(text, font, Brushes.Black, r.X, r.Y);
        }
    }
}
