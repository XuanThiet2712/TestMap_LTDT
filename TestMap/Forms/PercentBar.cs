using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Thanh tiến trình tự vẽ, hiện RÕ phần trăm ngay giữa thanh (ProgressBar của Windows không hiện được chữ).
    // Marquee = true: chưa biết tổng (đang tải từ mạng) nên chạy kiểu "đang chờ", chữ giữa thanh là Caption.
    public class PercentBar : Control
    {
        private int value;
        private bool marquee;
        private string caption = "";
        private float phase;
        private readonly Timer anim = new Timer();

        public PercentBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 22;
            anim.Interval = 30;
            anim.Tick += (s, e) => { phase += 0.018f; if (phase > 1f) phase = 0f; Invalidate(); };
        }

        [DefaultValue(0)]
        public int Value
        {
            get { return value; }
            set { this.value = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        [DefaultValue(false)]
        public bool Marquee
        {
            get { return marquee; }
            set
            {
                marquee = value;
                if (value && !DesignMode) anim.Start(); else anim.Stop();
                Invalidate();
            }
        }

        [DefaultValue("")]
        public string Caption
        {
            get { return caption; }
            set { caption = value ?? ""; Invalidate(); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) anim.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Parent != null ? Parent.BackColor : BackColor);

            var track = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
            float rad = track.Height / 2;
            using (GraphicsPath tp = Pill(track, rad))
            {
                using (var b = new SolidBrush(Color.FromArgb(51, 65, 85))) g.FillPath(b, tp);

                // phần đã chạy
                RectangleF fill;
                if (marquee)
                {
                    float w = track.Width * 0.30f;
                    float x = track.X + (track.Width - w) * Math.Abs((float)Math.Sin(phase * Math.PI));
                    fill = new RectangleF(x, track.Y, w, track.Height);
                }
                else fill = new RectangleF(track.X, track.Y, track.Width * value / 100f, track.Height);

                if (fill.Width > 1f)
                {
                    GraphicsState st = g.Save();
                    g.SetClip(tp, CombineMode.Intersect);
                    using (var lg = new LinearGradientBrush(new RectangleF(fill.X, fill.Y - 1, Math.Max(2f, fill.Width), fill.Height + 2),
                               Color.FromArgb(34, 197, 94), Color.FromArgb(37, 99, 235), LinearGradientMode.Horizontal))
                        g.FillRectangle(lg, fill);
                    g.Restore(st);
                }
                using (var edge = new Pen(Color.FromArgb(100, 116, 139), 1f)) g.DrawPath(edge, tp);
            }

            // chữ giữa thanh: "45%" (hoặc Caption khi đang chờ)
            string text = marquee ? caption : value + "%";
            if (text.Length == 0) return;
            using (var f = new Font("Segoe UI", Math.Max(8f, Height * 0.42f), FontStyle.Bold))
            {
                SizeF s = g.MeasureString(text, f);
                float tx = (Width - s.Width) / 2, ty = (Height - s.Height) / 2;
                using (var shadow = new SolidBrush(Color.FromArgb(160, 15, 23, 42))) g.DrawString(text, f, shadow, tx + 1, ty + 1);
                g.DrawString(text, f, Brushes.White, tx, ty);
            }
        }

        private static GraphicsPath Pill(RectangleF r, float rad)
        {
            var p = new GraphicsPath();
            float d = Math.Min(rad * 2, Math.Min(r.Width, r.Height));
            p.AddArc(r.X, r.Y, d, d, 90, 180);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 180);
            p.CloseFigure();
            return p;
        }
    }
}
