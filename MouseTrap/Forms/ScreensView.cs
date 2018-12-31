using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace MouseTrap {
    public partial class ScreensView : UserControl {
        public ScreensView()
        {
            InitializeComponent();

            Screens = Screen.AllScreens;
        }

        public Screen[] Screens { get; set; }
        public float CurrentScale { get; set; }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
            e.Graphics.Clear(BackColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            CurrentScale = GetScale();
            Draw(e.Graphics, e.ClipRectangle);
        }


        public int InnerWidth => Width - Padding.Left - Padding.Right;
        public int InnerHeight => Height - Padding.Top - Padding.Bottom;


        private void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            //using (var bgBrush = new SolidBrush(this.BackColor))
            using (var screen1Brush = new SolidBrush(HexColor("384b5e")))
            using (var screen2Brush = new SolidBrush(HexColor("314150")))
            using (var textBrush = new SolidBrush(HexColor("f2f2f2"))) {
                // Draw screens
                foreach (var screen in Screens) {
                    var rect = ScaleRect(screen.Bounds);

                    graphics.FillRectangle(screen1Brush, rect);
                    graphics.FillPolygon(screen2Brush, new[] {
                        new PointF(rect.X + (rect.Width * .05f), rect.Y + rect.Height),
                        new PointF(rect.X + rect.Width, rect.Y + rect.Height),
                        new PointF(rect.X + rect.Width, rect.Y + (rect.Height * .05f))
                    });

                    var textRect = rect;
                    textRect.Inflate(-10, -10);
                    var infoString = $"{screen.DeviceFriendlyName()}\r\n" +
                                     $"{screen.Bounds.Width}x{screen.Bounds.Height}\r\n" +
                                     $"offset x: {screen.Bounds.X}, y: {screen.Bounds.Y}\r\n" +
                                     $"{(screen.Primary ? "primary screen" : "")}\r\n";

                    graphics.DrawString(infoString, Font, textBrush, textRect, new StringFormat {Alignment = StringAlignment.Center});
                }
            }
        }

        private RectangleF ScaleRect(Rectangle rect)
        {
            var scale = CurrentScale;
            return new RectangleF(Padding.Left + rect.X * scale, Padding.Top + rect.Y * scale, rect.Width * scale, rect.Height * scale);
        }

        private float GetScale()
        {
            var bounds = Screens.Aggregate(Rectangle.Empty, (rect, screen) => Rectangle.Union(rect, screen.Bounds));
            var scale = InnerWidth / (float) bounds.Width;
            if (bounds.Height * scale > InnerHeight) {
                scale = InnerHeight / (float) bounds.Height;
            }

            return scale;
        }

        private Color HexColor(string hex, float alpha = 1)
        {
            if (hex == null || hex.Length != 6) {
                throw new ArgumentOutOfRangeException(nameof(hex));
            }

            var rgb = Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToInt32(hex.Substring(x, 2), 16))
                .ToArray();

            return Color.FromArgb((int) (alpha * 255), rgb[0], rgb[1], rgb[2]);
        }
    }
}
