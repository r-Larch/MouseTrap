using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;


namespace MouseTrap.Forms {
    public partial class EdgeSlider : UserControl {
        public LayoutStyle LayoutStyle { get; set; }
        internal Bar Bar { get; private set; }

        internal int BarLength => Math.Max(Width, Height);
        internal int BarSize => Math.Min(Width, Height);

        private int _topOffset;
        public int TopOffset {
            get => _topOffset;
            set => _topOffset = Math.Max(Math.Min(value, (BarLength - (BottomOffset + BarSize * 2))), 0);
        }

        private int _bottomOffset;
        public int BottomOffset {
            get => _bottomOffset;
            set => _bottomOffset = Math.Max(Math.Min(value, (BarLength - (TopOffset + BarSize * 2))), 0);
        }

        public EdgeSlider()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.Transparent;

            this.Layout += (s, e) => { Bar = GetBar(Size, TopOffset, BottomOffset); };
            this.MouseEnter += (s, e) => { HandleHover(); };
            this.MouseHover += (s, e) => { HandleHover(); };
            this.MouseMove += (s, e) => { HandleHover(); };
            this.MouseLeave += (s, e) => { HandleHover(); };

            this.MouseDown += (s, e) => {
                Bar.Top.Active = Bar.Top.Hover;
                Bar.Bottom.Active = Bar.Bottom.Hover;

                if (Bar.Top.Active) {
                    var pos = this.PointToClient(Cursor.Position);
                    var loc = Bar.Top.GetBounds().Location;
                    var offset = pos - new Size(loc.X, loc.Y);
                    Bar.Top.CursorPos = new Size(offset.X, offset.Y);
                }

                if (Bar.Bottom.Active) {
                    var pos = this.PointToClient(Cursor.Position);
                    var loc = Bar.Bottom.GetBounds().Location;
                    var offset = pos - new Size(loc.X, loc.Y);
                    Bar.Bottom.CursorPos = Bar.Bottom.GetBounds().Size - new Size(offset.X, offset.Y);
                }
            };
            this.MouseUp += (s, e) => {
                Bar.Top.Active = false;
                Bar.Bottom.Active = false;
            };
            this.MouseMove += (s, e) => {
                var pos = this.PointToClient(Cursor.Position);
                if (Bar.Top.Active) {
                    var offset = Bar.Top.CursorPos;
                    pos = pos - offset;

                    TopOffset = LayoutStyle == LayoutStyle.Left || LayoutStyle == LayoutStyle.Right ? pos.Y : pos.X;
                    Bar = GetBar(Size, TopOffset, BottomOffset);
                    Bar.Top.Hover = Bar.Top.Active = true;
                    Bar.Top.CursorPos = offset;
                    this.Invalidate();
                }

                if (Bar.Bottom.Active) {
                    var offset = Bar.Bottom.CursorPos;
                    pos = pos + offset;

                    BottomOffset = LayoutStyle == LayoutStyle.Left || LayoutStyle == LayoutStyle.Right ? Height - pos.Y : Width - pos.X;
                    Bar = GetBar(Size, TopOffset, BottomOffset);
                    Bar.Bottom.Hover = Bar.Bottom.Active = true;
                    Bar.Bottom.CursorPos = offset;
                    this.Invalidate();
                }
            };
        }

        private void HandleHover()
        {
            var pos = this.PointToClient(Cursor.Position);
            if (Bar.Top.Contains(pos)) {
                this.Cursor = LayoutStyle == LayoutStyle.Left || LayoutStyle == LayoutStyle.Right ? Cursors.SizeNS : Cursors.SizeWE;
                if (!Bar.Top.Hover) {
                    Bar.Top.Hover = true;
                    this.Invalidate(Bar.Top);
                }
            }
            else if (Bar.Top.Hover) {
                Bar.Top.Hover = false;
                this.Cursor = Cursors.Default;
                this.Invalidate(Bar.Top);
            }

            if (Bar.Bottom.Contains(pos)) {
                this.Cursor = LayoutStyle == LayoutStyle.Left || LayoutStyle == LayoutStyle.Right ? Cursors.SizeNS : Cursors.SizeWE;
                if (!Bar.Bottom.Hover) {
                    Bar.Bottom.Hover = true;
                    this.Invalidate(Bar.Bottom);
                }
            }
            else if (Bar.Bottom.Hover) {
                Bar.Bottom.Hover = false;
                this.Cursor = Cursors.Default;
                this.Invalidate(Bar.Bottom);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
            var bar = GetBar(Size, 0, 0);
            bar.Draw(e.Graphics, BackColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Draw(e.Graphics, e.ClipRectangle);
        }

        private void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            Bar.Draw(graphics);
        }


        private Bar GetBar(Size rect, int topOffset, int bottomOffset)
        {
            var size = Math.Min(rect.Width, rect.Height);

            switch (LayoutStyle) {
                case LayoutStyle.Top:
                    return new Bar(
                        new Triangle(0, 0, size, 0, size, size) + new Size(topOffset, 0),
                        new Rectangle(size + topOffset, 0, rect.Width - size * 2 - topOffset - bottomOffset, rect.Height),
                        new Triangle(size, 0, 0, 0, 0, size) + new Size(rect.Width - size - bottomOffset, 0)
                    );
                case LayoutStyle.Left:
                    return new Bar(
                        new Triangle(0, 0, 0, size, size, size) + new Size(0, topOffset),
                        new Rectangle(0, size + topOffset, rect.Width, rect.Height - size * 2 - topOffset - bottomOffset),
                        new Triangle(0, size, 0, 0, size, 0) + new Size(0, rect.Height - size - bottomOffset)
                    );
                case LayoutStyle.Right:
                    return new Bar(
                        new Triangle(0, size, size, size, size, 0) + new Size(0, topOffset),
                        new Rectangle(0, size + topOffset, rect.Width, rect.Height - size * 2 - topOffset - bottomOffset),
                        new Triangle(0, 0, size, size, size, 0) + new Size(0, rect.Height - size - bottomOffset)
                    );
                case LayoutStyle.Bottom:
                    return new Bar(
                        new Triangle(0, size, size, size, size, 0) + new Size(topOffset, 0),
                        new Rectangle(size + topOffset, 0, rect.Width - size * 2 - topOffset - bottomOffset, rect.Height),
                        new Triangle(size, size, 0, 0, 0, size) + new Size(rect.Width - size - bottomOffset, 0)
                    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(LayoutStyle));
            }
        }
    }


    internal class Bar {
        public Triangle Top { get; }
        public Rectangle Body { get; }
        public Triangle Bottom { get; }

        public Bar(Triangle top, Rectangle body, Triangle bottom)
        {
            Top = top;
            Body = body;
            Bottom = bottom;
        }

        public void Draw(Graphics g, Color? bgColor = null)
        {
            using (var bodyBg = new SolidBrush(bgColor ?? Color.Blue)) {
                g.FillRectangle(bodyBg, Body);

                Top.Draw(g, bgColor);
                Bottom.Draw(g, bgColor);
            }
        }
    }

    internal class Triangle {
        public bool Hover { get; set; }

        public Triangle(int x, int y, int x1, int y1, int x2, int y2)
        {
            Points = new[] {new Point(x, y), new Point(x1, y1), new Point(x2, y2)};
        }

        public Triangle(Point p1, Point p2, Point p3)
        {
            Points = new[] {p1, p2, p3};
        }

        public Point[] Points { get; }

        public bool Contains(Point p)
        {
            var p0 = Points[0];
            var p1 = Points[1];
            var p2 = Points[2];

            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var a = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;

            return a < 0 ? (s <= 0 && s + t >= a) : (s >= 0 && s + t <= a);
        }

        public GraphicsPath Path {
            get {
                var path = new GraphicsPath();
                path.AddPolygon(Points);
                return path;
            }
        }

        public Region Region => new Region(Path);
        public bool Active { get; set; }
        public Size CursorPos { get; set; }

        public static Triangle operator +(Triangle t, Size offset)
        {
            return new Triangle(
                t.Points[0] + offset,
                t.Points[1] + offset,
                t.Points[2] + offset
            );
        }

        public static implicit operator Region(Triangle triangle)
        {
            return triangle.Region;
        }

        public static implicit operator GraphicsPath(Triangle triangle)
        {
            return triangle.Path;
        }

        public void Draw(Graphics g, Color? bgColor)
        {
            var color = bgColor ?? Color.Red;
            if (bgColor == null) {
                if (Hover) {
                    color = Color.FromArgb(128, color);
                }
            }

            using (var pan = new Pen(Color.FromArgb(100, Color.Black), 3))
            using (var bg = new SolidBrush(color)) {
                g.FillPolygon(bg, Points);

                if (bgColor == null) {
                    g.DrawPolygon(pan, Scale(0.9f));
                }
            }
        }

        private PointF[] Scale(float s)
        {
            var A = Points[0];
            var B = Points[1];
            var C = Points[2];

            var a = Distance(B, C);
            var b = Distance(A, C);
            var c = Distance(A, B);

            float Distance(Point p1, Point p2) => (float) Math.Sqrt(Math.Abs(Math.Pow(p1.X - p2.X, 2)) + Math.Abs(Math.Pow(p1.Y - p2.Y, 2)));

            var p = a + b + c;

            var centerX = (a * A.X + b * B.X + c * C.X) / p;
            var centerY = (a * A.Y + b * B.Y + c * C.Y) / p;

            float MapX(int x) => (x - centerX) * s + centerX;
            float MapY(int y) => (y - centerY) * s + centerY;

            return new[] {
                new PointF(MapX(Points[0].X), MapY(Points[0].Y)),
                new PointF(MapX(Points[1].X), MapY(Points[1].Y)),
                new PointF(MapX(Points[2].X), MapY(Points[2].Y)),
            };
        }

        public Rectangle GetBounds()
        {
            var x = Points.Min(_ => _.X);
            var y = Points.Min(_ => _.Y);
            return new Rectangle(
                x,
                y,
                Points.Max(_ => _.X) - x,
                Points.Max(_ => _.Y) - y
            );
        }
    }

    public enum LayoutStyle {
        Top,
        Left,
        Right,
        Bottom
    }
}
