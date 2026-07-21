using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SongPlayer.Models;

namespace SongPlayer.Controls
{
    public class MoodTile : Control
    {
        private bool selected = false;
        public bool Selected { get => selected; set { selected = value; Invalidate(); } }
        public Mood Mood { get; set; }
        private int count;
        public int Count { get => count; set { count = value; Invalidate(); } }

        // Updated to match app dark palette (cards / panels)
        public Color Accent { get; set; } = ColorTranslator.FromHtml("#1DB954");
        public Color TileColor { get; set; } = ColorTranslator.FromHtml("#252525");
        public Color TileHover { get; set; } = ColorTranslator.FromHtml("#1C1C1E");

        private bool hovering = false;

        public MoodTile()
        {
            Height = 48;
            Width = 220;
            DoubleBuffered = true;
            try { Font = new Font("Segoe UI Variable", 9f, FontStyle.Bold); } catch { Font = new Font("Segoe UI", 9f, FontStyle.Bold); }
            // Avoid using Color.Transparent to prevent ArgumentException on some hosts.
            // Use the tile color as the control background and enable support for transparent backcolors
            // so child rendering that relies on transparency behaves correctly if needed.
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = TileColor;
            Margin = new Padding(6);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            hovering = true; Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hovering = false; Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            // bubble click so parent can handle
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // background with soft rounded corners and subtle gradient
            using (var path = RoundedRect(rect, 12))
            using (var bg = new LinearGradientBrush(rect, hovering ? TileHover : TileColor, Color.FromArgb(26, 26, 30), 90f))
            {
                g.FillPath(bg, path);
            }

            // selected highlight (soft glow)
            if (Selected)
            {
                using (var glow = new SolidBrush(Color.FromArgb(40, Accent.R, Accent.G, Accent.B)))
                {
                    var glowRect = Rectangle.Inflate(rect, -2, -2);
                    using (var gpath = RoundedRect(glowRect, 12))
                        g.FillPath(glow, gpath);
                }
            }

            // icon circle
            var circleRect = new Rectangle(10, 8, 32, 32);
            using (var gShadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillEllipse(gShadow, circleRect.X + 1, circleRect.Y + 1, circleRect.Width, circleRect.Height);
            }
            using (var brush = new SolidBrush(Accent))
            {
                g.FillEllipse(brush, circleRect);
            }

            // draw mood letter (first char) inside circle
            string letter = Mood.ToString().Substring(0, 1);
            using (var f = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (var b = new SolidBrush(Color.White))
            {
                var sz = g.MeasureString(letter, f);
                g.DrawString(letter, f, b, circleRect.X + (circleRect.Width - sz.Width) / 2f, circleRect.Y + (circleRect.Height - sz.Height) / 2f - 1);
            }

            // draw mood text
            using (var b = new SolidBrush(Color.White))
            {
                var moodText = "";
                try { moodText = Mood.ToString(); } catch { moodText = "Unknown"; }
                g.DrawString(moodText, Font, b, 54, 12);
            }

            // count bubble
            var countText = Count.ToString();
            using (var f = new Font("Segoe UI", 8f, FontStyle.Regular))
            {
                var csz = g.MeasureString(countText, f);
                var br = new Rectangle(Width - (int)csz.Width - 26, (Height - 20) / 2, (int)csz.Width + 14, 20);
                using (var b2 = new SolidBrush(Color.FromArgb(40, 40, 48)))
                using (var pen = new Pen(Color.FromArgb(60, 60, 66)))
                {
                    using (var path2 = RoundedRect(br, 10))
                    {
                        g.FillPath(b2, path2);
                        g.DrawPath(pen, path2);
                    }
                }
                using (var b3 = new SolidBrush(Color.White))
                {
                    g.DrawString(countText, f, b3, br.X + 7, br.Y + 2);
                }
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
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
    }
}
