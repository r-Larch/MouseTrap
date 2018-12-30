using System;
using System.Drawing;
using System.Windows.Forms;
using MouseTrap.Models;


namespace MouseTrap.Forms {
    // ReSharper disable LocalizableElement
    public partial class ScreenConfigForm : Form {
        public Button BtnTop;
        public Button BtnLeft;
        public Button BtnRight;
        public Button BtnBottom;
        public EdgeSlider BarTop;
        public EdgeSlider BarLeft;
        public EdgeSlider BarRight;
        public EdgeSlider BarBottom;

        public Button ResetBtn;
        public Button TestBtn;
        public Button CancelBtn;
        public Button SaveBtn;

        public event RemoveBarEvent RemoveBar;

        public ScreenBriges Screen { get; }

        public ScreenConfigForm()
        {
            InitializeComponent();
        }

        public ScreenConfigForm(ScreenBriges screen) : this()
        {
            Screen = screen;

            this.SuspendLayout();
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = Screen.Screen.Bounds;
            this.CancelButton = CancelBtn;
            this.KeyPreview = true;
            this.KeyDown += (sender, args) => {
                if (args.KeyCode == Keys.Escape) {
                    CancelBtn.PerformClick();
                }
            };

            Panel.SuspendLayout();

            SetupButtons();
            SetupBars();
            SetupInfos();

            Panel.ResumeLayout(false);
            this.ResumeLayout(false);
        }


        private void SetupInfos()
        {
            var inner = new Rectangle(0, 0, Bounds.Size.Width, Bounds.Size.Height);
            inner.Inflate(-150, -150);

            var table = new TableLayoutPanel() {
                Bounds = inner,
                ColumnCount = 1,
                ColumnStyles = {new ColumnStyle(SizeType.Percent, 100)},
                RowCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Percent, 60),
                    new RowStyle(SizeType.Percent, 40)
                }
            };

            table.Controls.Add(new Label {
                Text = Screen.ScreenNum,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Location = Point.Empty,
                Size = new SizeF(inner.Width, inner.Height * .6f).ToSize(),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font.FontFamily, inner.Height * .6f * .6f, GraphicsUnit.Pixel)
            }, 0, 0);

            TestBtn = new Button {
                Text = "Test current settings",
                Anchor = AnchorStyles.None,
                Size = new Size(200, 40)
            };
            ResetBtn = new Button {
                Text = "Reset to previous",
                Anchor = AnchorStyles.None,
                Size = new Size(200, 40),
                Visible = false,
            };
            SaveBtn = new Button {
                Text = "Save",
                Anchor = AnchorStyles.None,
                Size = new Size(80, 40),
            };
            CancelBtn = new Button {
                Text = "Close",
                Anchor = AnchorStyles.None,
                Size = new Size(80, 40),
            };

            table.Controls.Add(new FlowLayoutPanel() {
                Size = new Size(6 + TestBtn.Width + 6 + SaveBtn.Width + 6 + CancelBtn.Width + 6, 40 + 6),
                Padding = new Padding(3),
                Anchor = AnchorStyles.None,
                Controls = {TestBtn, ResetBtn, SaveBtn, CancelBtn}
            }, 0, 1);

            Panel.Controls.Add(table);
        }


        private void SetupButtons()
        {
            var inner = new Rectangle(0, 0, Bounds.Size.Width, Bounds.Size.Height);
            inner.Inflate(-5 - 40, -5 - 40);

            BtnTop = new Button {
                Location = new Point((inner.Width / 2) - 20, inner.X),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnTop.Click += (s, e) => ToggleTop();
            Panel.Controls.Add(BtnTop);

            BtnLeft = new Button {
                Location = new Point(inner.X, (inner.Height / 2) - 20),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnLeft.Click += (s, e) => ToggleLeft();
            Panel.Controls.Add(BtnLeft);

            BtnRight = new Button {
                Location = new Point(inner.Width, (inner.Height / 2) - 20),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnRight.Click += (s, e) => ToggleRight();
            Panel.Controls.Add(BtnRight);

            BtnBottom = new Button {
                Location = new Point((inner.Width / 2) - 20, inner.Height),
                Size = new Size(40, 40),
                Text = "+",
                UseVisualStyleBackColor = true
            };
            BtnBottom.Click += (s, e) => ToggleBottom();
            Panel.Controls.Add(BtnBottom);
        }

        private void SetupBars()
        {
            BarTop = new EdgeSlider(Panel) {
                Bounds = new Rectangle(0, 0, Width, 60),
                LayoutStyle = LayoutStyle.Top,
                TopOffset = Screen.TopBrige?.TopOffset ?? 0,
                BottomOffset = Screen.TopBrige?.BottomOffset ?? 0,
                //Visible = screen.TopBrige != null
            };
            if (Screen.TopBrige != null) {
                ToggleTop(Screen.TopBrige.TargetScreenId, forceEnable: true);
            }

            BarBottom = new EdgeSlider(Panel) {
                Bounds = new Rectangle(0, Height - 60, Width, 60),
                LayoutStyle = LayoutStyle.Bottom,
                TopOffset = Screen.BottomBrige?.TopOffset ?? 0,
                BottomOffset = Screen.BottomBrige?.BottomOffset ?? 0,
                //Visible = screen.BottomBrige != null
            };
            if (Screen.BottomBrige != null) {
                ToggleBottom(Screen.BottomBrige.TargetScreenId, forceEnable: true);
            }

            BarLeft = new EdgeSlider(Panel) {
                Bounds = new Rectangle(0, 0, 60, Height),
                LayoutStyle = LayoutStyle.Left,
                TopOffset = Screen.LeftBrige?.TopOffset ?? 0,
                BottomOffset = Screen.LeftBrige?.BottomOffset ?? 0,
                //Visible = screen.LeftBrige != null
            };
            if (Screen.LeftBrige != null) {
                ToggleLeft(Screen.LeftBrige.TargetScreenId, forceEnable: true);
            }

            BarRight = new EdgeSlider(Panel) {
                Bounds = new Rectangle(Width - 60, 0, 60, Height),
                LayoutStyle = LayoutStyle.Right,
                TopOffset = Screen.RightBrige?.TopOffset ?? 0,
                BottomOffset = Screen.RightBrige?.BottomOffset ?? 0,
                //Visible = screen.RightBrige != null
            };
            if (Screen.RightBrige != null) {
                ToggleRight(Screen.RightBrige.TargetScreenId, forceEnable: true);
            }
        }


        public void ToggleLeft(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
        {
            if (!BarLeft.Visible && !forceDisable) {
                BarLeft.TargetScreenId = targetId ?? GetTargetScreenId(Screen, BrigePosition.Left);
                BtnLeft.Location = new Point(BtnLeft.Location.X + 20, BtnLeft.Location.Y);
                BtnLeft.Text = "-";
                BarLeft.Show();
            }
            else if (!forceEnable) {
                BtnLeft.Location = new Point(BtnLeft.Location.X - 20, BtnLeft.Location.Y);
                BtnLeft.Text = "+";
                BarLeft.Hide();
                if (!forceDisable) {
                    this.RemoveBar?.Invoke(this, BrigePosition.Left, BarLeft.TargetScreenId);
                }
            }
        }

        public void ToggleRight(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
        {
            if (!BarRight.Visible && !forceDisable) {
                BarRight.TargetScreenId = targetId ?? GetTargetScreenId(Screen, BrigePosition.Right);
                BtnRight.Location = new Point(BtnRight.Location.X - 20, BtnRight.Location.Y);
                BtnRight.Text = "-";
                BarRight.Show();
            }
            else if (!forceEnable) {
                BtnRight.Location = new Point(BtnRight.Location.X + 20, BtnRight.Location.Y);
                BtnRight.Text = "+";
                BarRight.Hide();
                if (!forceDisable) {
                    this.RemoveBar?.Invoke(this, BrigePosition.Right, BarLeft.TargetScreenId);
                }
            }
        }

        public void ToggleTop(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
        {
            if (!BarTop.Visible && !forceDisable) {
                BarTop.TargetScreenId = targetId ?? GetTargetScreenId(Screen, BrigePosition.Top);
                BtnTop.Location = new Point(BtnTop.Location.X, BtnTop.Location.Y + 20);
                BtnTop.Text = "-";
                BarTop.Show();
            }
            else if (!forceEnable) {
                BtnTop.Location = new Point(BtnTop.Location.X, BtnTop.Location.Y - 20);
                BtnTop.Text = "+";
                BarTop.Hide();
                if (!forceDisable) {
                    this.RemoveBar?.Invoke(this, BrigePosition.Top, BarLeft.TargetScreenId);
                }
            }
        }

        public void ToggleBottom(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
        {
            if (!BarBottom.Visible && !forceDisable) {
                BarBottom.TargetScreenId = targetId ?? GetTargetScreenId(Screen, BrigePosition.Bottom);
                BtnBottom.Location = new Point(BtnBottom.Location.X, BtnBottom.Location.Y - 20);
                BtnBottom.Text = "-";
                BarBottom.Show();
            }
            else if (!forceEnable) {
                BtnBottom.Location = new Point(BtnBottom.Location.X, BtnBottom.Location.Y + 20);
                BtnBottom.Text = "+";
                BarBottom.Hide();
                if (!forceDisable) {
                    this.RemoveBar?.Invoke(this, BrigePosition.Bottom, BarLeft.TargetScreenId);
                }
            }
        }


        public Func<ScreenBriges, BrigePosition, int> GetTargetScreenId { get; set; }

        public ScreenBriges GetConfig()
        {
            Screen.TopBrige = BarTop.Visible
                ? new Brige() {
                    TopOffset = BarTop.TopOffset,
                    BottomOffset = BarTop.BottomOffset,
                    TargetScreenId = BarTop.TargetScreenId
                }
                : null;

            Screen.BottomBrige = BarBottom.Visible
                ? new Brige() {
                    TopOffset = BarBottom.TopOffset,
                    BottomOffset = BarBottom.BottomOffset,
                    TargetScreenId = BarBottom.TargetScreenId
                }
                : null;

            Screen.LeftBrige = BarLeft.Visible
                ? new Brige() {
                    TopOffset = BarLeft.TopOffset,
                    BottomOffset = BarLeft.BottomOffset,
                    TargetScreenId = BarLeft.TargetScreenId
                }
                : null;

            Screen.RightBrige = BarRight.Visible
                ? new Brige() {
                    TopOffset = BarRight.TopOffset,
                    BottomOffset = BarRight.BottomOffset,
                    TargetScreenId = BarRight.TargetScreenId
                }
                : null;

            return Screen;
        }

        public void AddTargetBarForPosition(BrigePosition position, int targetScreenId)
        {
            switch (position) {
                case BrigePosition.Top:
                    ToggleBottom(targetScreenId, forceEnable: true);
                    break;
                case BrigePosition.Left:
                    ToggleRight(targetScreenId, forceEnable: true);
                    break;
                case BrigePosition.Right:
                    ToggleLeft(targetScreenId, forceEnable: true);
                    break;
                case BrigePosition.Bottom:
                    ToggleTop(targetScreenId, forceEnable: true);
                    break;
            }
        }

        public void RemoveTargetBarForPosition(BrigePosition position, int targetScreenId)
        {
            switch (position) {
                case BrigePosition.Top:
                    if (Screen.ScreenId == targetScreenId) ToggleBottom(forceDisable: true);
                    break;
                case BrigePosition.Left:
                    if (Screen.ScreenId == targetScreenId) ToggleRight(forceDisable: true);
                    break;
                case BrigePosition.Right:
                    if (Screen.ScreenId == targetScreenId) ToggleLeft(forceDisable: true);
                    break;
                case BrigePosition.Bottom:
                    if (Screen.ScreenId == targetScreenId) ToggleTop(forceDisable: true);
                    break;
            }
        }
    }

    public delegate void RemoveBarEvent(ScreenConfigForm sender, BrigePosition position, int targetScreenId);
}
