using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Vẽ sơ đồ mô hình: các trạm là vòng tròn, mỗi tuyến xe là một đường màu, số trên đường là thời gian (giây).
    public class ModelCanvas : Panel
    {
        public ModelData Data;
        public int Step;                    // đã hoàn thành bao nhiêu bước của cách đi (Legs.Count = xong hết)
        public bool ShowWeights = true;

        private static readonly Font NameFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font SmallFont = new Font("Segoe UI", 8f);
        private static readonly Font TitleFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        private static readonly Color Ink = Color.FromArgb(15, 23, 42);

        public ModelCanvas()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (Data == null || Data.Nodes.Count == 0)
            {
                using (var b = new SolidBrush(Color.FromArgb(100, 116, 139)))
                    g.DrawString("Chưa có cách đi để vẽ.", NameFont, b, 20, 20);
                return;
            }

            PointF[] pos = Project();
            var used = new List<RectangleF>();

            // tiêu đề + tổng đã cộng
            g.DrawString(Data.Title, TitleFont, new SolidBrush(Ink), 14, 10);
            double sum = 0;
            for (int i = 0; i < Step && i < Data.Legs.Count; i++) sum += Data.Legs[i].Seconds;
            string total = "Tổng trọng số đã cộng: " + (int)Math.Round(sum) + " giây";
            SizeF ts = g.MeasureString(total, NameFont);
            var tb = new RectangleF(Width - ts.Width - 30, 8, ts.Width + 18, ts.Height + 8);
            using (GraphicsPath tp = Round(tb, 8))
            using (var f = new SolidBrush(Color.FromArgb(254, 243, 199)))
                g.FillPath(f, tp);
            g.DrawString(total, NameFont, new SolidBrush(Color.FromArgb(146, 64, 14)), tb.X + 9, tb.Y + 4);

            // 1) các cạnh không thuộc cách đi (nền mờ), 2) các cạnh thuộc cách đi
            foreach (ModelEdge ed in Data.Edges) if (ed.PathLeg < 0) DrawEdge(g, ed, pos, used, false);
            foreach (ModelEdge ed in Data.Edges) if (ed.PathLeg >= 0) DrawEdge(g, ed, pos, used, true);

            // đỉnh
            for (int i = 0; i < Data.Nodes.Count; i++) DrawNode(g, i, pos[i]);

            // thẻ "chờ xe" ở trạm lên xe
            for (int li = 0; li < Data.Legs.Count && li < Step; li++)
            {
                ModelLeg l = Data.Legs[li];
                if (l.Kind != "wait" || l.Node < 0) continue;
                string t = "chờ ~" + Math.Max(1, (int)Math.Round(l.Seconds / 60.0)) + " phút (" + (int)l.Seconds + " s)";
                SizeF s = g.MeasureString(t, SmallFont);
                PointF p = pos[l.Node];
                var r = new RectangleF(p.X - s.Width / 2 - 6, p.Y - 44, s.Width + 12, s.Height + 4);
                using (GraphicsPath rp = Round(r, 6))
                using (var f = new SolidBrush(Color.FromArgb(245, 158, 11)))
                    g.FillPath(f, rp);
                g.DrawString(t, SmallFont, Brushes.White, r.X + 6, r.Y + 2);
                used.Add(r);
            }

            // tên trạm: trạm quan trọng trước (đầu, cuối, đổi tuyến, trên đường đi), rồi các trạm còn lại nếu không chồng chữ
            var onPath = new HashSet<int>();
            foreach (ModelEdge ed in Data.Edges) if (ed.PathLeg >= 0) { onPath.Add(ed.A); onPath.Add(ed.B); }
            for (int pass = 0; pass < 2; pass++)
                for (int i = 0; i < Data.Nodes.Count; i++)
                {
                    ModelNode n = Data.Nodes[i];
                    bool important = n.Start || n.End || n.Transfer || onPath.Contains(i);
                    if ((pass == 0) != important) continue;
                    DrawName(g, n.Name, pos[i], used, important);
                }

            DrawLegend(g);
        }

        // Chiếu kinh độ / vĩ độ lên khung vẽ, giữ đúng tỉ lệ
        private PointF[] Project()
        {
            double minLat = double.MaxValue, maxLat = double.MinValue, minLng = double.MaxValue, maxLng = double.MinValue;
            foreach (ModelNode n in Data.Nodes)
            {
                minLat = Math.Min(minLat, n.Lat); maxLat = Math.Max(maxLat, n.Lat);
                minLng = Math.Min(minLng, n.Lng); maxLng = Math.Max(maxLng, n.Lng);
            }
            double kx = Math.Cos((minLat + maxLat) / 2 * Math.PI / 180);
            double w = Math.Max(1e-9, (maxLng - minLng) * kx), h = Math.Max(1e-9, maxLat - minLat);
            float mx = 90, top = 70, bottom = 60;
            double aw = Math.Max(50, Width - 2 * mx), ah = Math.Max(50, Height - top - bottom);
            double scale = Math.Min(aw / w, ah / h);
            double ox = mx + (aw - w * scale) / 2, oy = top + (ah - h * scale) / 2;
            var res = new PointF[Data.Nodes.Count];
            for (int i = 0; i < res.Length; i++)
            {
                ModelNode n = Data.Nodes[i];
                res[i] = new PointF((float)(ox + (n.Lng - minLng) * kx * scale), (float)(oy + (maxLat - n.Lat) * scale));
            }
            return res;
        }

        private void DrawEdge(Graphics g, ModelEdge ed, PointF[] pos, List<RectangleF> used, bool pathPass)
        {
            PointF a = pos[ed.A], b = pos[ed.B];
            float dx = b.X - a.X, dy = b.Y - a.Y, len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 1) return;
            float off = ed.Walk ? 0 : (ed.Lane - (Data.LaneCount - 1) / 2f) * 7f;     // các tuyến chung đường chạy song song
            float nx = -dy / len * off, ny = dx / len * off;
            PointF p1 = new PointF(a.X + nx, a.Y + ny), p2 = new PointF(b.X + nx, b.Y + ny);

            bool done = ed.PathLeg >= 0 && ed.PathLeg < Step;
            bool active = ed.PathLeg >= 0 && ed.PathLeg == Step - 1;

            if (!pathPass)
            {
                using (var pen = new Pen(Color.FromArgb(ed.Walk ? 120 : 110, ed.Color), 4f))
                {
                    if (ed.Walk) pen.DashStyle = DashStyle.Dash;
                    pen.StartCap = pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, p1, p2);
                }
            }
            else if (!done)
            {
                // thuộc cách đi nhưng chưa tới bước này: nét mờ có viền
                using (var pen = new Pen(Color.FromArgb(150, ed.Color), 5f))
                {
                    pen.DashStyle = DashStyle.Dot;
                    g.DrawLine(pen, p1, p2);
                }
            }
            else
            {
                float w = active ? 10f : 8f;
                using (var halo = new Pen(Color.White, w + 5)) { halo.StartCap = halo.EndCap = LineCap.Round; g.DrawLine(halo, p1, p2); }
                using (var pen = new Pen(ed.Color, w))
                {
                    pen.StartCap = pen.EndCap = LineCap.Round;
                    if (ed.Walk) pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, p1, p2);
                }
                if (active) DrawArrow(g, p1, p2, ed.Color);
            }

            if (!ShowWeights) return;
            string text = (int)Math.Round(ed.Seconds) + " s";
            SizeF ts = g.MeasureString(text, SmallFont);
            float t = 0.5f;
            var rc = new RectangleF(p1.X + (p2.X - p1.X) * t - ts.Width / 2 - 4, p1.Y + (p2.Y - p1.Y) * t - ts.Height / 2 - 1, ts.Width + 8, ts.Height + 2);
            bool strong = ed.PathLeg >= 0 && done;
            if (!strong && Collides(rc, used)) return;
            used.Add(rc);
            using (GraphicsPath cp = Round(rc, 4))
            {
                using (var f = new SolidBrush(strong ? ed.Color : Color.FromArgb(235, 255, 255, 255))) g.FillPath(f, cp);
                using (var pen = new Pen(strong ? Color.White : Color.FromArgb(148, 163, 184), 1f)) g.DrawPath(pen, cp);
            }
            g.DrawString(text, SmallFont, strong ? Brushes.White : Brushes.Black, rc.X + 4, rc.Y + 1);
        }

        private static void DrawArrow(Graphics g, PointF a, PointF b, Color c)
        {
            float dx = b.X - a.X, dy = b.Y - a.Y, len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 30) return;
            float ux = dx / len, uy = dy / len;
            PointF m = new PointF(a.X + dx * 0.72f, a.Y + dy * 0.72f);
            var tri = new[] {
                new PointF(m.X + ux * 8, m.Y + uy * 8),
                new PointF(m.X - ux * 4 - uy * 7, m.Y - uy * 4 + ux * 7),
                new PointF(m.X - ux * 4 + uy * 7, m.Y - uy * 4 - ux * 7) };
            using (var f = new SolidBrush(Color.White)) g.FillPolygon(f, tri);
        }

        private void DrawNode(Graphics g, int i, PointF p)
        {
            ModelNode n = Data.Nodes[i];
            float r = (n.Start || n.End) ? 13f : 8f;
            if (n.Transfer && !(n.Start || n.End))
                using (var ring = new Pen(Color.FromArgb(100, 116, 139), 2f)) g.DrawEllipse(ring, p.X - r - 4, p.Y - r - 4, 2 * (r + 4), 2 * (r + 4));
            Color fill = n.Start ? Color.FromArgb(22, 163, 74) : (n.End ? Color.FromArgb(220, 38, 38) : Color.White);
            using (var f = new SolidBrush(fill)) g.FillEllipse(f, p.X - r, p.Y - r, 2 * r, 2 * r);
            using (var pen = new Pen(n.Start || n.End ? Color.White : Ink, 2.2f)) g.DrawEllipse(pen, p.X - r, p.Y - r, 2 * r, 2 * r);
            if (n.Start || n.End)
            {
                string l = n.Start ? "A" : "B";
                SizeF s = g.MeasureString(l, NameFont);
                g.DrawString(l, NameFont, Brushes.White, p.X - s.Width / 2, p.Y - s.Height / 2);
            }
        }

        private void DrawName(Graphics g, string text, PointF p, List<RectangleF> used, bool important)
        {
            SizeF s = g.MeasureString(text, NameFont);
            // thử đặt chữ ở bên phải, trái, dưới, trên của đỉnh
            PointF[] tries = {
                new PointF(p.X + 15, p.Y - s.Height / 2), new PointF(p.X - 15 - s.Width, p.Y - s.Height / 2),
                new PointF(p.X - s.Width / 2, p.Y + 14), new PointF(p.X - s.Width / 2, p.Y - 14 - s.Height) };
            foreach (PointF t in tries)
            {
                var r = new RectangleF(t.X, t.Y, s.Width, s.Height);
                if (Collides(r, used) && !important) continue;
                if (r.X < 2 || r.Right > Width - 2 || r.Y < 2 || r.Bottom > Height - 2) continue;
                using (var halo = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++) g.DrawString(text, NameFont, halo, r.X + dx, r.Y + dy);
                g.DrawString(text, NameFont, new SolidBrush(Ink), r.X, r.Y);
                used.Add(r);
                return;
            }
        }

        private void DrawLegend(Graphics g)
        {
            float y = Height - 34;
            using (var f = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                g.DrawString("Vòng tròn = trạm (đỉnh)    Đường màu = tuyến xe (cạnh)    Nét đứt = đi bộ    Vòng đôi = chỗ đổi tuyến    Số trên đường = trọng số = thời gian (giây)", SmallFont, f, 14, y);
            }
        }

        private static bool Collides(RectangleF r, List<RectangleF> used)
        {
            RectangleF p = RectangleF.Inflate(r, 2, 1);
            foreach (RectangleF u in used) if (u.IntersectsWith(p)) return true;
            return false;
        }

        private static GraphicsPath Round(RectangleF r, float rad)
        {
            var path = new GraphicsPath();
            float d = rad * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
