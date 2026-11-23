using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using TestMap.Routing;

namespace TestMap.Mapping
{
    // Bản đồ tối giản: nền trơn + đường nhạt + trạm xe buýt + tuyến đi, tự vẽ bằng GDI+ từ chính dữ liệu đường,
    // nên tuyến đi và trạm luôn khớp với đường vẽ trên bản đồ.
    //   1. đường (nhạt, đường lớn dày hơn)
    //   2. (tùy chọn) các đoạn đường thuật toán đã xét
    //   3. tuyến đi: nét xanh có viền trắng và mũi tên chỉ chiều
    //   4. trạm xe buýt, trạm trên tuyến (đánh số), điểm A / B, xe buýt chạy thử
    public class MapCanvas : GMapControl
    {
        // ----- Dữ liệu để vẽ (Form gán vào) -----
        public RoadGraph RoadNet;
        public SearchResult LastResult;
        public RoutePath Route;                        // tuyến đi dạng dãy điểm liên tục
        public double RouteProgress = 1.0;             // 0..1: vẽ tuyến tới đâu (để tuyến "chạy ra" khi vừa tìm xong)
        public int VisitedShown;                       // số giao lộ đã xét được vẽ (hoạt ảnh thuật toán)
        public bool ShowSearch;                        // vẽ các đoạn đường thuật toán đã xét
        public bool ShowPath;                          // true thì vẽ tuyến đi
        public bool ShowStops = true;                  // bật/tắt trạm xe buýt
        public int StartNode = -1, EndNode = -1;       // giao lộ điểm đi / đến
        public int StartStop = -1, EndStop = -1;       // trạm điểm đi / đến (-1 nếu chọn theo giao lộ)
        public List<int> RouteStops = new List<int>();         // các trạm nằm trên tuyến, theo thứ tự
        public List<double> RouteStopDist = new List<double>(); // quãng đường (m) từ điểm đi tới từng trạm trên tuyến
        public double BusMeters = -1;                  // vị trí xe buýt chạy thử dọc tuyến (m); < 0 là ẩn
        public int HighlightStop = -1;                 // trạm đang được chọn trong danh sách
        public string Attribution = "Dữ liệu © OpenStreetMap contributors";

        // ----- Tuyến xe buýt -----
        public List<BusLine> NetLines = new List<BusLine>();    // các tuyến vẽ nền (mảnh, đúng màu tuyến)
        public BusLine SelectedLine;                            // tuyến đang chọn: vẽ đậm, hiện các trạm của tuyến
        public List<MapLeg> ItinLegs = new List<MapLeg>();      // hành trình được gợi ý: các đoạn đi bộ / đi xe
        public bool ShowItinerary;
        public List<BusLine> BusLines = new List<BusLine>();    // các tuyến có xe chạy (mô phỏng)
        public bool ShowLiveBuses;
        public double BusClock;                                 // đồng hồ mô phỏng (giây)
        public const double SimSpeed = 14.0;                    // xe mô phỏng chạy m/s (nhanh hơn thật để dễ nhìn)

        // Bấm chuột trái (không kéo) lên một trạm
        public event Action<int> StopClicked;

        // ----- Bảng màu -----
        public static readonly Color BackgroundColor = Color.FromArgb(241, 244, 248);
        private static readonly Color RoadCasing = Color.FromArgb(224, 229, 236);
        private static readonly Color MainCasing = Color.FromArgb(208, 215, 225);
        private static readonly float[] RoadWidth = { 1.5f, 2.4f, 3.4f, 4.6f };

        public static readonly Color RouteColor = Color.FromArgb(37, 99, 235);
        public static readonly Color StopColor = Color.FromArgb(71, 85, 105);
        public static readonly Color StartColor = Color.FromArgb(22, 163, 74);
        public static readonly Color EndColor = Color.FromArgb(220, 38, 38);
        private static readonly Color InkColor = Color.FromArgb(15, 23, 42);
        private static readonly Color VisitedColor = Color.FromArgb(150, 147, 197, 253);

        private static readonly Font StopFont = new Font("Segoe UI", 8.25f);
        private static readonly Font RouteFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font PinFont = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font NumFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        private static readonly Font SmallFont = new Font("Segoe UI", 8f);
        private static readonly StringFormat Fmt = new StringFormat(StringFormat.GenericTypographic);
        private static readonly StringFormat CenterFmt = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        private int hoverStop = -1;
        private Point downPoint;

        private static Pen RoundPen(Color c, float w)
        {
            var p = new Pen(c, w);
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;
            p.LineJoin = LineJoin.Round;
            return p;
        }

        // =====================================================================
        //  VẼ
        // =====================================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // GMap vẽ nền trơn trước

            if (RoadNet == null || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            Graphics g = e.Graphics;
            g.ResetTransform();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            RectLatLng view = ViewArea;

            float scale = (float)Math.Max(0.6, Math.Min(2.4, 1 + (Zoom - 15) * 0.3));
            int minClass = Zoom < 13 ? 2 : (Zoom < 14 ? 1 : 0);   // zoom xa thì bỏ hẻm và đường nhỏ cho nhẹ

            // 1. Đường: viền trước, ruột sau, để các ngã tư nối liền
            for (int c = minClass; c < 4; c++)
                using (Pen pen = RoundPen(c >= 2 ? MainCasing : RoadCasing, RoadWidth[c] * scale + 1.8f))
                    DrawEdges(g, pen, RoadNet.EdgesByClass[c], view);
            for (int c = minClass; c < 4; c++)
                using (Pen pen = RoundPen(Color.White, RoadWidth[c] * scale))
                    DrawEdges(g, pen, RoadNet.EdgesByClass[c], view);

            // 2. Các đoạn đường thuật toán đã xét (chỉ khi người dùng bật)
            if (ShowSearch && LastResult != null)
            {
                using (Pen pen = RoundPen(VisitedColor, 2.4f * scale + 1f))
                {
                    int n = Math.Min(VisitedShown, LastResult.VisitedViaEdge.Count);
                    for (int i = 0; i < n; i++)
                    {
                        int ei = LastResult.VisitedViaEdge[i];
                        if (ei >= 0) DrawEdge(g, pen, ei, view);
                    }
                }
            }

            // 2b. Tuyến xe buýt và hành trình được gợi ý
            DrawTransit(g, scale, view);

            // 3. Tuyến đi
            double upto = RouteLength();
            DrawRoute(g, scale, upto);

            // 4. Trạm xe buýt
            var routeShown = new HashSet<int>();      // trạm trên tuyến đã được tuyến "chạy tới"
            var order = new Dictionary<int, int>();   // trạm -> số thứ tự trên tuyến
            if (ShowPath && Route != null)
                for (int i = 0; i < RouteStops.Count && i < RouteStopDist.Count; i++)
                    if (RouteStopDist[i] <= upto + 0.01 && routeShown.Add(RouteStops[i]))
                        order[RouteStops[i]] = i + 1;

            var used = new List<RectangleF>();        // các ô chữ đã chiếm chỗ, để nhãn không đè nhau
            var normalLabels = new List<KeyValuePair<string, PointF>>();
            var routeLabels = new List<KeyValuePair<string, PointF>>();

            float r = Zoom >= 17 ? 5f : (Zoom >= 15 ? 4f : 3f);
            if (ShowStops)
            {
                using (var fill = new SolidBrush(StopColor))
                using (var border = new Pen(Color.White, 1.6f))
                    for (int i = 0; i < RoadNet.Stops.Count; i++)
                    {
                        if (routeShown.Contains(i)) continue;
                        BusStop s = RoadNet.Stops[i];
                        if (!InView(s.Lat, s.Lng, view)) continue;
                        PointF p = ToScreen(s.Lat, s.Lng);
                        g.FillEllipse(fill, p.X - r, p.Y - r, 2 * r, 2 * r);
                        g.DrawEllipse(border, p.X - r, p.Y - r, 2 * r, 2 * r);
                        if (Zoom >= 17) normalLabels.Add(new KeyValuePair<string, PointF>(s.Name, p));
                    }
            }

            // Trạm nằm trên tuyến: chấm trắng viền xanh, có số thứ tự khi đủ lớn
            float rr = Zoom >= 16 ? 9f : 6f;
            using (var fill = new SolidBrush(Color.White))
            using (var border = new Pen(RouteColor, 2.6f))
            using (var txt = new SolidBrush(RouteColor))
                foreach (int si in routeShown)
                {
                    if (si == StartStop || si == EndStop) continue;     // điểm A / B có ghim riêng
                    BusStop s = RoadNet.Stops[si];
                    PointF p = ToScreen(s.Lat, s.Lng);
                    g.FillEllipse(fill, p.X - rr, p.Y - rr, 2 * rr, 2 * rr);
                    g.DrawEllipse(border, p.X - rr, p.Y - rr, 2 * rr, 2 * rr);
                    if (Zoom >= 16)
                        g.DrawString(order[si].ToString(), NumFont, txt, new RectangleF(p.X - rr, p.Y - rr, 2 * rr, 2 * rr), CenterFmt);
                    routeLabels.Add(new KeyValuePair<string, PointF>(s.Name, p));
                }

            // Trạm đang chọn trong danh sách: vòng sáng
            if (HighlightStop >= 0 && HighlightStop < RoadNet.Stops.Count)
            {
                PointF p = ToScreen(RoadNet.Stops[HighlightStop].Lat, RoadNet.Stops[HighlightStop].Lng);
                using (var ring = new Pen(Color.FromArgb(245, 158, 11), 3f))
                    g.DrawEllipse(ring, p.X - 14, p.Y - 14, 28, 28);
            }

            // Trạm lên / xuống xe của hành trình gợi ý và các trạm của tuyến đang chọn
            DrawTransitStops(g, view, routeLabels);

            // Điểm đi, điểm đến
            bool hasStart = StartNode >= 0, hasEnd = EndNode >= 0;
            PointF startP = hasStart ? PointOf(StartStop, StartNode) : new PointF();
            PointF endP = hasEnd ? PointOf(EndStop, EndNode) : new PointF();
            if (hasStart) DrawPin(g, startP, "A", StartColor);
            if (hasEnd) DrawPin(g, endP, "B", EndColor);

            // Nhãn: điểm đi/đến trước, rồi trạm trên tuyến, cuối cùng các trạm còn lại
            if (hasStart) DrawLabel(g, StopName(StartStop, "Điểm đi"), startP, PinFont, StartColor, 15f, used);
            if (hasEnd) DrawLabel(g, StopName(EndStop, "Điểm đến"), endP, PinFont, EndColor, 15f, used);
            if (Zoom >= 16)
                foreach (var kv in routeLabels) DrawLabel(g, kv.Key, kv.Value, RouteFont, InkColor, rr + 4f, used);
            foreach (var kv in normalLabels) DrawLabel(g, kv.Key, kv.Value, StopFont, StopColor, r + 4f, used);

            // 4b. Xe buýt đang chạy (mô phỏng)
            DrawLiveBuses(g, view);

            // 5. Xe buýt chạy thử
            if (BusMeters >= 0 && Route != null && Route.Points.Count > 1)
            {
                LatLng b = Route.PointAt(BusMeters);
                DrawBus(g, ToScreen(b.Lat, b.Lng));
            }

            // 6. Tên trạm khi rê chuột
            if (hoverStop >= 0 && hoverStop < RoadNet.Stops.Count)
            {
                BusStop s = RoadNet.Stops[hoverStop];
                PointF p = ToScreen(s.Lat, s.Lng);
                SizeF ts = g.MeasureString(s.Name, PinFont, 10000, Fmt);
                var rc = new RectangleF(p.X - ts.Width / 2 - 9, p.Y - ts.Height - 22, ts.Width + 18, ts.Height + 8);
                using (GraphicsPath path = RoundRect(rc, 5f))
                using (var bg = new SolidBrush(Color.FromArgb(235, 15, 23, 42)))
                    g.FillPath(bg, path);
                g.DrawString(s.Name, PinFont, Brushes.White, rc.X + 9, rc.Y + 4, Fmt);
            }

            DrawLegend(g);
            DrawAttribution(g);
        }

        // ----- Tuyến xe buýt -----
        private static bool LineInView(BusLine l, RectLatLng v)
        {
            return !(l.MaxLat < v.Bottom || l.MinLat > v.Top || l.MaxLng < v.Left || l.MinLng > v.Right);
        }

        private PointF[] ScreenPoints(List<LatLng> pts)
        {
            var res = new PointF[pts.Count];
            for (int i = 0; i < pts.Count; i++) res[i] = ToScreen(pts[i].Lat, pts[i].Lng);
            return res;
        }

        private void DrawTransit(Graphics g, float scale, RectLatLng view)
        {
            // Các tuyến nền: nét mảnh, mờ hơn nếu đang chọn một tuyến khác
            foreach (BusLine l in NetLines)
            {
                if (l == SelectedLine || !LineInView(l, view)) continue;
                PointF[] pts = ScreenPoints(l.Path.Points);
                if (pts.Length < 2) continue;
                using (Pen pen = RoundPen(Color.FromArgb(SelectedLine != null ? 70 : 150, l.Color), Math.Max(2.5f, 3f * scale)))
                    g.DrawLines(pen, pts);
            }

            // Tuyến đang chọn: nét đậm có viền trắng và mũi tên
            if (SelectedLine != null && SelectedLine.Path.Points.Count > 1)
            {
                PointF[] pts = ScreenPoints(SelectedLine.Path.Points);
                float w = Math.Max(4.5f, 5.5f * scale);
                using (Pen casing = RoundPen(Color.White, w + 4f)) g.DrawLines(casing, pts);
                using (Pen pen = RoundPen(SelectedLine.Color, w)) g.DrawLines(pen, pts);
                if (Zoom >= 15) DrawArrows(g, pts, w);
            }

            // Hành trình gợi ý: đoạn đi xe tô màu tuyến, đoạn đi bộ nét đứt xám
            if (!ShowItinerary) return;
            foreach (MapLeg leg in ItinLegs)
            {
                PointF[] pts = ScreenPoints(leg.Points);
                if (pts.Length < 2) continue;
                if (leg.Walk)
                {
                    using (var pen = new Pen(Color.FromArgb(100, 116, 139), 3.4f))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        g.DrawLines(pen, pts);
                    }
                }
                else
                {
                    float w = Math.Max(4.5f, 5.5f * scale);
                    using (Pen casing = RoundPen(Color.White, w + 4f)) g.DrawLines(casing, pts);
                    using (Pen pen = RoundPen(leg.Color, w)) g.DrawLines(pen, pts);
                    if (Zoom >= 15) DrawArrows(g, pts, w);
                }
            }
        }

        // Trạm lên / xuống xe của hành trình gợi ý, và trạm của tuyến đang chọn
        private void DrawTransitStops(Graphics g, RectLatLng view, List<KeyValuePair<string, PointF>> labels)
        {
            if (ShowItinerary)
                foreach (MapLeg leg in ItinLegs)
                {
                    if (leg.Walk) continue;
                    foreach (int si in new[] { leg.FromStop, leg.ToStop })
                    {
                        if (si < 0 || si >= RoadNet.Stops.Count || si == StartStop || si == EndStop) continue;
                        PointF p = ToScreen(RoadNet.Stops[si].Lat, RoadNet.Stops[si].Lng);
                        using (var fill = new SolidBrush(Color.White))
                        using (var border = new Pen(leg.Color, 3.2f))
                        {
                            g.FillEllipse(fill, p.X - 7.5f, p.Y - 7.5f, 15, 15);
                            g.DrawEllipse(border, p.X - 7.5f, p.Y - 7.5f, 15, 15);
                        }
                        labels.Add(new KeyValuePair<string, PointF>(RoadNet.Stops[si].Name, p));
                    }
                }

            if (SelectedLine != null)
                foreach (int si in SelectedLine.Stops)
                {
                    BusStop s = RoadNet.Stops[si];
                    if (!InView(s.Lat, s.Lng, view)) continue;
                    PointF p = ToScreen(s.Lat, s.Lng);
                    using (var fill = new SolidBrush(Color.White))
                    using (var border = new Pen(SelectedLine.Color, 2.8f))
                    {
                        g.FillEllipse(fill, p.X - 5.5f, p.Y - 5.5f, 11, 11);
                        g.DrawEllipse(border, p.X - 5.5f, p.Y - 5.5f, 11, 11);
                    }
                    if (Zoom >= 16) labels.Add(new KeyValuePair<string, PointF>(s.Name, p));
                }
        }

        // Xe buýt đang chạy: mỗi tuyến vài xe cách đều nhau, chạy dọc tuyến rồi quay lại đầu tuyến (mô phỏng)
        private void DrawLiveBuses(Graphics g, RectLatLng view)
        {
            if (!ShowLiveBuses) return;
            foreach (BusLine l in BusLines)
            {
                double len = l.Length;
                if (len < 50 || !LineInView(l, view)) continue;
                int count = Math.Max(1, (int)Math.Round(len / 1200.0));
                for (int k = 0; k < count; k++)
                {
                    double d = (BusClock * SimSpeed + (k + l.Phase) * len / count) % len;
                    LatLng p = l.Path.PointAt(d);
                    if (!InView(p.Lat, p.Lng, view)) continue;
                    DrawBusChip(g, ToScreen(p.Lat, p.Lng), l);
                }
            }
        }

        // Xe là một ô tròn góc màu tuyến, ghi số tuyến
        private void DrawBusChip(Graphics g, PointF c, BusLine l)
        {
            if (Zoom < 14)
            {
                using (var b = new SolidBrush(l.Color)) g.FillEllipse(b, c.X - 4, c.Y - 4, 8, 8);
                g.DrawEllipse(Pens.White, c.X - 4, c.Y - 4, 8, 8);
                return;
            }
            string text = l.Ref.Length > 4 ? l.Ref.Substring(0, 4) : l.Ref;
            SizeF ts = g.MeasureString(text, NumFont, 10000, Fmt);
            var rc = new RectangleF(c.X - (ts.Width + 12) / 2, c.Y - 9, ts.Width + 12, 18);
            using (var shadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (GraphicsPath sp = RoundRect(new RectangleF(rc.X, rc.Y + 2, rc.Width, rc.Height), 7f))
                g.FillPath(shadow, sp);
            using (GraphicsPath path = RoundRect(rc, 7f))
            {
                using (var fill = new SolidBrush(l.Color)) g.FillPath(fill, path);
                using (var border = new Pen(Color.White, 2f)) g.DrawPath(border, path);
            }
            g.DrawString(text, NumFont, Brushes.White, rc, CenterFmt);
        }

        // Chiều dài tuyến đã "chạy ra" (m)
        private double RouteLength()
        {
            if (!ShowPath || Route == null) return 0;
            return Route.Total * Math.Max(0.0, Math.Min(1.0, RouteProgress));
        }

        private void DrawRoute(Graphics g, float scale, double upto)
        {
            if (!ShowPath || Route == null || Route.Points.Count < 2) return;
            PointF[] pts = RouteScreenPoints(upto);
            if (pts.Length < 2) return;

            float w = Math.Max(4.5f, 5.5f * scale);
            using (Pen casing = RoundPen(Color.White, w + 4f)) g.DrawLines(casing, pts);
            using (Pen pen = RoundPen(RouteColor, w)) g.DrawLines(pen, pts);
            if (Zoom >= 15) DrawArrows(g, pts, w);
        }

        // Các điểm của tuyến (đã đổi ra tọa độ màn hình) tới quãng đường upto
        private PointF[] RouteScreenPoints(double upto)
        {
            var list = new List<PointF>();
            List<LatLng> pts = Route.Points;
            List<double> cum = Route.Cum;
            for (int i = 0; i < pts.Count; i++)
            {
                if (cum[i] <= upto)
                {
                    list.Add(ToScreen(pts[i].Lat, pts[i].Lng));
                    continue;
                }
                if (i > 0)
                {
                    double seg = cum[i] - cum[i - 1];
                    double t = seg > 0 ? (upto - cum[i - 1]) / seg : 0;
                    list.Add(ToScreen(pts[i - 1].Lat + (pts[i].Lat - pts[i - 1].Lat) * t,
                                      pts[i - 1].Lng + (pts[i].Lng - pts[i - 1].Lng) * t));
                }
                break;
            }
            return list.ToArray();
        }

        // Mũi tên trắng nhỏ trên tuyến để thấy chiều đi
        private void DrawArrows(Graphics g, PointF[] pts, float w)
        {
            const float spacing = 90f;
            float size = Math.Max(2.6f, w * 0.36f);
            float next = spacing * 0.5f;
            float walked = 0f;
            using (Pen pen = RoundPen(Color.White, Math.Max(1.8f, w * 0.32f)))
                for (int i = 1; i < pts.Length; i++)
                {
                    float dx = pts[i].X - pts[i - 1].X, dy = pts[i].Y - pts[i - 1].Y;
                    float len = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (len < 0.01f) continue;
                    float ux = dx / len, uy = dy / len;
                    while (next <= walked + len)
                    {
                        float t = next - walked;
                        float cx = pts[i - 1].X + ux * t, cy = pts[i - 1].Y + uy * t;
                        next += spacing;
                        if (cx < -20 || cy < -20 || cx > Width + 20 || cy > Height + 20) continue;
                        var chevron = new[]
                        {
                            new PointF(cx - ux * size - uy * size, cy - uy * size + ux * size),
                            new PointF(cx + ux * size, cy + uy * size),
                            new PointF(cx - ux * size + uy * size, cy - uy * size - ux * size)
                        };
                        g.DrawLines(pen, chevron);
                    }
                    walked += len;
                }
        }

        // Ghim tròn có chữ A / B
        private void DrawPin(Graphics g, PointF c, string letter, Color color)
        {
            using (var shadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                g.FillEllipse(shadow, c.X - 12, c.Y - 9, 24, 24);
            using (var fill = new SolidBrush(color))
                g.FillEllipse(fill, c.X - 11, c.Y - 11, 22, 22);
            using (var border = new Pen(Color.White, 2.4f))
                g.DrawEllipse(border, c.X - 11, c.Y - 11, 22, 22);
            g.DrawString(letter, PinFont, Brushes.White, new RectangleF(c.X - 11, c.Y - 11, 22, 22), CenterFmt);
        }

        // Xe buýt chạy thử
        private void DrawBus(Graphics g, PointF c)
        {
            using (var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                g.FillEllipse(shadow, c.X - 12, c.Y - 9, 24, 24);
            using (var fill = new SolidBrush(InkColor))
                g.FillEllipse(fill, c.X - 12, c.Y - 12, 24, 24);
            using (var border = new Pen(Color.White, 2.4f))
                g.DrawEllipse(border, c.X - 12, c.Y - 12, 24, 24);
            var body = new RectangleF(c.X - 6, c.Y - 5, 12, 10);
            using (GraphicsPath p = RoundRect(body, 2.5f))
                g.FillPath(Brushes.White, p);
            using (var win = new SolidBrush(InkColor))
            {
                g.FillRectangle(win, c.X - 4, c.Y - 3, 8, 3);
                g.FillEllipse(win, c.X - 4, c.Y + 2, 2.4f, 2.4f);
                g.FillEllipse(win, c.X + 1.6f, c.Y + 2, 2.4f, 2.4f);
            }
        }

        // Nhãn bên cạnh chấm (thử bên phải rồi bên trái); bỏ qua nếu đè lên nhãn khác
        private void DrawLabel(Graphics g, string text, PointF c, Font font, Color color, float gap, List<RectangleF> used)
        {
            SizeF ts = g.MeasureString(text, font, 10000, Fmt);
            var r = new RectangleF(c.X + gap, c.Y - ts.Height / 2, ts.Width, ts.Height);
            if (Collides(r, used))
            {
                r = new RectangleF(c.X - gap - ts.Width, c.Y - ts.Height / 2, ts.Width, ts.Height);
                if (Collides(r, used)) return;
            }
            used.Add(r);

            // viền trắng quanh chữ cho dễ đọc trên nền đường
            using (var halo = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0) g.DrawString(text, font, halo, r.X + dx, r.Y + dy, Fmt);
            using (var b = new SolidBrush(color))
                g.DrawString(text, font, b, r.X, r.Y, Fmt);
        }

        private static bool Collides(RectangleF r, List<RectangleF> used)
        {
            RectangleF padded = RectangleF.Inflate(r, 2, 1);
            foreach (RectangleF u in used)
                if (u.IntersectsWith(padded)) return true;
            return false;
        }

        // Chú giải nhỏ ở góc trên trái
        private void DrawLegend(Graphics g)
        {
            const string t1 = "Tuyến đi";
            const string t2 = "Trạm xe buýt";
            float w1 = g.MeasureString(t1, SmallFont, 10000, Fmt).Width;
            float w2 = g.MeasureString(t2, SmallFont, 10000, Fmt).Width;
            var rc = new RectangleF(12, 12, 46 + Math.Max(w1, w2) + 12, 52);
            using (GraphicsPath path = RoundRect(rc, 8f))
            {
                using (var bg = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
                    g.FillPath(bg, path);
                using (var edge = new Pen(Color.FromArgb(210, 216, 226), 1f))
                    g.DrawPath(edge, path);
            }
            using (Pen line = RoundPen(RouteColor, 4f))
                g.DrawLine(line, rc.X + 12, rc.Y + 16, rc.X + 34, rc.Y + 16);
            using (var dot = new SolidBrush(StopColor))
                g.FillEllipse(dot, rc.X + 17, rc.Y + 31, 10, 10);
            using (var ink = new SolidBrush(InkColor))
            {
                g.DrawString(t1, SmallFont, ink, rc.X + 42, rc.Y + 9, Fmt);
                g.DrawString(t2, SmallFont, ink, rc.X + 42, rc.Y + 29, Fmt);
            }
        }

        // Ghi nguồn dữ liệu (yêu cầu của giấy phép OpenStreetMap)
        private void DrawAttribution(Graphics g)
        {
            if (string.IsNullOrEmpty(Attribution)) return;
            SizeF s = g.MeasureString(Attribution, SmallFont, 10000, Fmt);
            using (var bg = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                g.FillRectangle(bg, 12, Height - s.Height - 12, s.Width + 10, s.Height + 6);
            g.DrawString(Attribution, SmallFont, Brushes.DimGray, 17, Height - s.Height - 9, Fmt);
        }

        // =====================================================================
        //  CHUỘT
        // =====================================================================
        // Trạm gần điểm (x, y) trên màn hình nhất trong bán kính cho trước (pixel); -1 nếu không có
        public int StopAt(int x, int y, double radius)
        {
            if (RoadNet == null) return -1;
            int best = -1;
            double bestD = radius * radius;
            for (int i = 0; i < RoadNet.Stops.Count; i++)
            {
                BusStop s = RoadNet.Stops[i];
                GPoint p = FromLatLngToLocal(new PointLatLng(s.Lat, s.Lng));
                double dx = p.X - x, dy = p.Y - y;
                double d = dx * dx + dy * dy;
                if (d < bestD) { bestD = d; best = i; }
            }
            return best;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (RoadNet == null || e.Button != MouseButtons.None) return;

            int best = (ShowStops || ShowPath) ? StopAt(e.X, e.Y, 12) : -1;
            if (best != hoverStop)
            {
                hoverStop = best;
                Cursor = best >= 0 ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (hoverStop != -1) { hoverStop = -1; Invalidate(); }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            downPoint = e.Location;
            base.OnMouseDown(e);
        }

        // Bấm (không kéo) chuột trái lên trạm thì báo cho Form
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left || RoadNet == null || StopClicked == null) return;
            int dx = e.X - downPoint.X, dy = e.Y - downPoint.Y;
            if (dx * dx + dy * dy > 25) return;             // đã kéo bản đồ, không phải bấm
            int s = StopAt(e.X, e.Y, 12);
            if (s >= 0) StopClicked(s);
        }

        // =====================================================================
        //  HÀM PHỤ
        // =====================================================================
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

        public static bool InView(double lat, double lng, RectLatLng v)
        {
            return lat <= v.Top && lat >= v.Bottom && lng >= v.Left && lng <= v.Right;
        }

        private static GraphicsPath RoundRect(RectangleF r, float radius)
        {
            float d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
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
    }
}
