using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SongPlayer.Controls
{
    public class SeekBar : Control
    {
        private bool dragging;
        private double value;
        public double Value
        {
            get => value;
            set
            {
                this.value = Math.Max(0, Math.Min(1, value));
                Invalidate();
            }
        }

        public Color TrackColor { get; set; } = Color.FromArgb(80, 80, 90);
        public Color FillColor { get; set; } = Color.DodgerBlue;
        public Color ThumbColor { get; set; } = Color.White;

        public event EventHandler ValueChangedByUser;

        public bool IsDragging => dragging;

        public SeekBar()
        {
            DoubleBuffered = true;
            Height = 18;
            SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var trackRect = new Rectangle(8, Height / 2 - 4, Width - 16, 8);
            // track background
            using (var bg = new LinearGradientBrush(trackRect, Color.FromArgb(50, 50, 56), Color.FromArgb(35, 35, 40), 0f))
            {
                using (var path = RoundedRect(trackRect, 6)) g.FillPath(bg, path);
            }
            var fillW = (int)(trackRect.Width * value);
            var fillRect = new Rectangle(trackRect.X, trackRect.Y, fillW, trackRect.Height);
            if (fillRect.Width > 0)
            {
                using (var f = new LinearGradientBrush(fillRect, FillColor, ControlPaint.Light(FillColor), 0f))
                {
                    using (var path = RoundedRect(fillRect, 6)) g.FillPath(f, path);
                }
            }
            int thumbX = trackRect.X + Math.Max(6, fillW) - 10;
            var thumbRect = new Rectangle(thumbX, trackRect.Y - 6, 20, 20);
            using (var th = new SolidBrush(ThumbColor)) g.FillEllipse(th, thumbRect);
            using (var pen = new Pen(Color.FromArgb(60, 60, 70))) g.DrawEllipse(pen, thumbRect);
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Capture = true;
            dragging = true;
            UpdateValueFromMouse(e.X);
            ValueChangedByUser?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragging)
            {
                UpdateValueFromMouse(e.X);
                ValueChangedByUser?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Capture = false;
            dragging = false;
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            int trackX = 6;
            int trackW = Math.Max(1, Width - 12);
            double v = (double)(mouseX - trackX) / trackW;
            Value = v;
        }
    }
}
