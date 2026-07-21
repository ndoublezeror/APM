using System;
using System.Drawing;
using System.Windows.Forms;

namespace SongPlayer.Controls
{
    public class IconButton : Button
    {
        public AppIcon Icon { get; set; }
        public Color IdleColor { get; set; } = Color.FromArgb(40, 40, 50);
        public Color HoverColor { get; set; } = Color.FromArgb(64, 64, 76);
        public bool Filled { get; set; }
        public bool CircleBackground { get; set; }
        public Color AccentBackground { get; set; } = ColorTranslator.FromHtml("#8A6AFF");
        public float IconScale { get; set; } = 1f;

        private static readonly System.Collections.Generic.Dictionary<AppIcon, string> Glyphs =
            new System.Collections.Generic.Dictionary<AppIcon, string>
        {
            { AppIcon.Close, "E8BB" },       // x
            { AppIcon.Minimize, "E921" },    // minimize
            { AppIcon.Plus, "E710" },        // plus
            { AppIcon.Trash, "E74D" },       // delete
            { AppIcon.Shuffle, "E8B1" },     // shuffle
            { AppIcon.Previous, "E892" },    // previous
            { AppIcon.Play, "E768" },        // play
            { AppIcon.Pause, "E769" },       // pause
            { AppIcon.Next, "E893" },        // next
            { AppIcon.Repeat, "E9FF" },      // repeat
            { AppIcon.Volume, "E995" }       // volume
        };

        public IconButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = IdleColor;
            ForeColor = Color.White;
            Text = "";
            Size = new Size(34, 34);
            try { Font = new Font("Segoe MDL2 Assets", 14f); } catch { Font = new Font(SystemFonts.DefaultFont.FontFamily, 12f); }
            AccentBackground = ColorTranslator.FromHtml("#1DB954");
            IdleColor = Color.FromArgb(36, 36, 38);
            HoverColor = Color.FromArgb(56, 56, 60);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = HoverColor;
            // subtle icon enlarge on hover without resizing control
            IconScale = 1.12f;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            BackColor = IdleColor;
            IconScale = 1f;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            var g = pevent.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // draw background circle if CircleBackground
            if (CircleBackground)
            {
                using (var b = new SolidBrush(BackColor))
                using (var pen = new Pen(Color.FromArgb(30, 30, 36)))
                {
                    g.FillEllipse(b, 0, 0, Width - 1, Height - 1);
                    g.DrawEllipse(pen, 0, 0, Width - 1, Height - 1);
                }
            }

            // draw glyph
            string glyph = Glyphs.ContainsKey(Icon) ? char.ConvertFromUtf32(Convert.ToInt32(Glyphs[Icon], 16)) : "?";
            using (var b = new SolidBrush(ForeColor))
            using (var f = new Font("Segoe MDL2 Assets", 14f * IconScale, FontStyle.Regular, GraphicsUnit.Point))
            {
                var sz = g.MeasureString(glyph, f);
                g.DrawString(glyph, f, b, (Width - sz.Width) / 2f, (Height - sz.Height) / 2f - 1);
            }
        }
    }
}
