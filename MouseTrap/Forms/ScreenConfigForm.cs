using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;


namespace MouseTrap.Forms {
    public partial class ScreenConfigForm : Form {
        public Button BtnTop;
        public Button BtnLeft;
        public Button BtnRight;
        public Button BtnBottom;
        public EdgeSlider BarTop;
        public EdgeSlider BarLeft;
        public EdgeSlider BarRight;
        public EdgeSlider BarBottom;

        public ScreenConfigForm()
        {
            InitializeComponent();
        }

        public ScreenConfigForm(Screen screen) : this()
        {
            StartPosition = FormStartPosition.Manual;
            Bounds = screen.Bounds;

            //this.SuspendLayout();
            //Panel.SuspendLayout();

            var inner = new Rectangle(0, 0, Bounds.Size.Width, Bounds.Size.Height);
            inner.Inflate(-5 - 40, -5 - 40);

            BtnTop = new Button {
                Location = new Point((inner.Width / 2) - 20, inner.X),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnTop.Click += Top_Clicked;
            Panel.Controls.Add(BtnTop);

            BtnLeft = new Button {
                Location = new Point(inner.X, (inner.Height / 2) - 20),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnLeft.Click += Left_Clicked;
            Panel.Controls.Add(BtnLeft);

            BtnRight = new Button {
                Location = new Point(inner.Width, (inner.Height / 2) - 20),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnRight.Click += Right_Clicked;
            Panel.Controls.Add(BtnRight);

            BtnBottom = new Button {
                Location = new Point((inner.Width / 2) - 20, inner.Height),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnBottom.Click += Bottom_Clicked;
            Panel.Controls.Add(BtnBottom);

            // Bars
            BarTop = new EdgeSlider {
                Bounds = new Rectangle(0, 0, Width, 60),
                LayoutStyle = LayoutStyle.Top,
                TopOffset = 0,
                BottomOffset = 0
            };
            BarTop.Hide();
            Panel.Controls.Add(BarTop);

            BarBottom = new EdgeSlider {
                Bounds = new Rectangle(0, Height - 60, Width, 60),
                LayoutStyle = LayoutStyle.Bottom,
                TopOffset = 0,
                BottomOffset = 0
            };
            BarBottom.Hide();
            Panel.Controls.Add(BarBottom);

            BarLeft = new EdgeSlider {
                Bounds = new Rectangle(0, 0, 60, Height),
                LayoutStyle = LayoutStyle.Left,
                TopOffset = 0,
                BottomOffset = 0
            };
            BarLeft.Hide();
            Panel.Controls.Add(BarLeft);

            BarRight = new EdgeSlider {
                Bounds = new Rectangle(Width - 60, 0, 60, Height),
                LayoutStyle = LayoutStyle.Right,
                TopOffset = 0,
                BottomOffset = 0
            };
            BarRight.Hide();
            Panel.Controls.Add(BarRight);

            //Panel.ResumeLayout(false);
            //this.ResumeLayout(false);
        }


        private void Left_Clicked(object sender, EventArgs e)
        {
            if (!BarLeft.Visible) {
                BtnLeft.Location = new Point(BtnLeft.Location.X + 20, BtnLeft.Location.Y);
                BtnLeft.Text = "-";
                BarLeft.Show();
            }
            else {
                BtnLeft.Location = new Point(BtnLeft.Location.X - 20, BtnLeft.Location.Y);
                BtnLeft.Text = "+";
                BarLeft.Hide();
            }
        }

        private void Right_Clicked(object sender, EventArgs e)
        {
            if (!BarRight.Visible) {
                BtnRight.Location = new Point(BtnRight.Location.X - 20, BtnRight.Location.Y);
                BtnRight.Text = "-";
                BarRight.Show();
            }
            else {
                BtnRight.Location = new Point(BtnRight.Location.X + 20, BtnRight.Location.Y);
                BtnRight.Text = "+";
                BarRight.Hide();
            }
        }

        private void Top_Clicked(object sender, EventArgs e)
        {
            if (!BarTop.Visible) {
                BtnTop.Location = new Point(BtnTop.Location.X, BtnTop.Location.Y + 20);
                BtnTop.Text = "-";
                BarTop.Show();
            }
            else {
                BtnTop.Location = new Point(BtnTop.Location.X, BtnTop.Location.Y - 20);
                BtnTop.Text = "+";
                BarTop.Hide();
            }
        }

        private void Bottom_Clicked(object sender, EventArgs e)
        {
            if (!BarBottom.Visible) {
                BtnBottom.Location = new Point(BtnBottom.Location.X, BtnBottom.Location.Y - 20);
                BtnBottom.Text = "-";
                BarBottom.Show();
            }
            else {
                BtnBottom.Location = new Point(BtnBottom.Location.X, BtnBottom.Location.Y + 20);
                BtnBottom.Text = "+";
                BarBottom.Hide();
            }
        }
    }
}
