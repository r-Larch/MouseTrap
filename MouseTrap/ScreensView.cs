using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace MouseTrap {
    public partial class ScreensView : UserControl {
        public ScreensView()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Draw();
        }

        public int InnerWidth => Width - Padding.Left - Padding.Right;
        public int InnerHeight => Height - Padding.Top - Padding.Bottom;

        public void Draw()
        {
            var screens = Screen.AllScreens;
            var bounds = screens.Aggregate(Rectangle.Empty, (rect, screen) => Rectangle.Union(rect, screen.Bounds));

            var scale = InnerWidth / (float) bounds.Width;
            if (bounds.Height * scale > InnerHeight) {
                scale = InnerHeight / (float) bounds.Height;
            }

            using (var bgBrush = new SolidBrush(this.BackColor))
            using (var screen1Brush = new SolidBrush(HexColor("384b5e")))
            using (var screen2Brush = new SolidBrush(HexColor("314150")))
            using (var textBrush = new SolidBrush(HexColor("f2f2f2")))
            using (var graphic = this.CreateGraphics()) {
                // Draw bg
                graphic.FillRectangle(bgBrush, this.Bounds);

                // Draw screens
                foreach (var screen in screens) {
                    var rect = new RectangleF(Padding.Left + screen.Bounds.X * scale, Padding.Top + screen.Bounds.Y * scale, screen.Bounds.Width * scale, screen.Bounds.Height * scale);

                    graphic.FillRectangle(screen1Brush, rect);
                    graphic.FillPolygon(screen2Brush, new[] {
                        new PointF(rect.X + (rect.Width * .05f), rect.Y + rect.Height),
                        new PointF(rect.X + rect.Width, rect.Y + rect.Height),
                        new PointF(rect.X + rect.Width, rect.Y + (rect.Height * .05f))
                    });

                    var textRect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                    textRect.Inflate(-10, -10);
                    var infoString = $"{screen.DeviceFriendlyName()}\r\n" +
                                     $"{screen.Bounds.Width}x{screen.Bounds.Height}\r\n" +
                                     $"offset x: {screen.Bounds.X}, y: {screen.Bounds.Y}\r\n" +
                                     $"{(screen.Primary ? "primary screen" : "")}\r\n";

                    graphic.DrawString(infoString, Font, textBrush, textRect, new StringFormat {Alignment = StringAlignment.Center});
                }
            }
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
