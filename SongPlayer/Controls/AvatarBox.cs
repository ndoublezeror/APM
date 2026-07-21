using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SongPlayer.Controls
{
    public class AvatarBox : Control
    {
        private Image image;
        public Image Image
        {
            get => image;
            set { image = value; Invalidate(); }
        }

        // compatibility with PictureBox API used elsewhere
        public PictureBoxSizeMode SizeMode { get; set; } = PictureBoxSizeMode.Zoom;

        public AvatarBox()
        {
            Size = new Size(360, 360);
            DoubleBuffered = true;
            // Allow transparent background colors safely for this custom-painted control.
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Ensure the SupportsTransparentBackColor style is set and only then assign Transparent.
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        // support alpha blending for crossfade
        private Image overlayImage;
        private float overlayAlpha = 0f;

        public void CrossfadeTo(Image img)
        {
            overlayImage = img;
            overlayAlpha = 0f;
            var t = new Timer { Interval = 25 };
            t.Tick += (s, e) =>
            {
                overlayAlpha += 0.08f;
                if (overlayAlpha >= 1f)
                {
                    image = overlayImage;
                    overlayImage = null;
                    overlayAlpha = 0f;
                    t.Stop();
                    t.Dispose();
                }
                Invalidate();
            };
            t.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width, Height);

            // drop shadow
            using (var sh = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                var shadowRect = new Rectangle(6, 8, Width - 8, Height - 8);
                g.FillEllipse(sh, shadowRect);
            }

            // rounded image
            var imgRect = new Rectangle(0, 0, Width - 8, Height - 8);
            imgRect.Offset(4, 4);
            using (var path = RoundedRect(imgRect, 12))
            {
                g.SetClip(path);
                if (image != null)
                {
                    try
                    {
                        g.DrawImage(image, imgRect);
                    }
                    catch { }
                }
                else
                {
                    using (var brush = new LinearGradientBrush(imgRect, Color.FromArgb(40,40,48), Color.FromArgb(26,26,30), 45f))
                    {
                        g.FillPath(brush, path);
                    }
                }
                g.ResetClip();
                using (var pen = new Pen(Color.FromArgb(60, 60, 66), 2))
                {
                    g.DrawPath(pen, path);
                }
            }
            // draw overlay for crossfade
            if (overlayImage != null)
            {
                try
                {
                    var cm = new System.Drawing.Imaging.ColorMatrix();
                    cm.Matrix33 = Math.Min(1f, overlayAlpha);
                    var ia = new System.Drawing.Imaging.ImageAttributes();
                    ia.SetColorMatrix(cm, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    g.DrawImage(overlayImage, imgRect, 0, 0, overlayImage.Width, overlayImage.Height, GraphicsUnit.Pixel, ia);
                }
                catch { }
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
