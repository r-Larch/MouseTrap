using MouseTrap.Models;


namespace MouseTrap.Forms; 

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

    public ScreenConfig Screen { get; }

#pragma warning disable CS8618 // Non-nullable variable must contain a non-null value when exiting constructor. Consider declaring it as nullable.
    public ScreenConfigForm()
#pragma warning restore CS8618
    {
        InitializeComponent();
    }

    public ScreenConfigForm(ScreenConfig screen) : this()
    {
        Screen = screen;

        this.SuspendLayout();
        this.StartPosition = FormStartPosition.Manual;
        this.Bounds = Screen.Bounds;
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
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
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
            Controls = { TestBtn, ResetBtn, SaveBtn, CancelBtn }
        }, 0, 1);

        Panel.Controls.Add(table);
    }


    private void SetupButtons()
    {
        var inner = new Rectangle(0, 0, Bounds.Size.Width, Bounds.Size.Height);
        inner.Inflate(-5 - 40, -5 - 40);

        BtnTop = new Button {
            Location = new Point((Width / 2) - 20, inner.X),
            Size = new Size(40, 40),
            Text = "+",
            UseVisualStyleBackColor = true
        };
        BtnTop.Click += (s, e) => ToggleTop();
        Panel.Controls.Add(BtnTop);

        BtnLeft = new Button {
            Location = new Point(inner.X, (Height / 2) - 20),
            Size = new Size(40, 40),
            Text = "+",
            UseVisualStyleBackColor = true
        };
        BtnLeft.Click += (s, e) => ToggleLeft();
        Panel.Controls.Add(BtnLeft);

        BtnRight = new Button {
            Location = new Point(inner.Width, (Height / 2) - 20),
            Size = new Size(40, 40),
            Text = "+",
            UseVisualStyleBackColor = true
        };
        BtnRight.Click += (s, e) => ToggleRight();
        Panel.Controls.Add(BtnRight);

        BtnBottom = new Button {
            Size = new Size(40, 40),
            Location = new Point((Width / 2) - 20, inner.Height),
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
            TopOffset = Screen.TopBridge?.TopOffset ?? 0,
            BottomOffset = Screen.TopBridge?.BottomOffset ?? 0,
            //Visible = screen.TopBridge != null
        };
        if (Screen.TopBridge != null) {
            ToggleTop(Screen.TopBridge.TargetScreenId, forceEnable: true);
        }

        BarBottom = new EdgeSlider(Panel) {
            Bounds = new Rectangle(0, Height - 60, Width, 60),
            LayoutStyle = LayoutStyle.Bottom,
            TopOffset = Screen.BottomBridge?.TopOffset ?? 0,
            BottomOffset = Screen.BottomBridge?.BottomOffset ?? 0,
            //Visible = screen.BottomBridge != null
        };
        if (Screen.BottomBridge != null) {
            ToggleBottom(Screen.BottomBridge.TargetScreenId, forceEnable: true);
        }

        BarLeft = new EdgeSlider(Panel) {
            Bounds = new Rectangle(0, 0, 60, Height),
            LayoutStyle = LayoutStyle.Left,
            TopOffset = Screen.LeftBridge?.TopOffset ?? 0,
            BottomOffset = Screen.LeftBridge?.BottomOffset ?? 0,
            //Visible = screen.LeftBridge != null
        };
        if (Screen.LeftBridge != null) {
            ToggleLeft(Screen.LeftBridge.TargetScreenId, forceEnable: true);
        }

        BarRight = new EdgeSlider(Panel) {
            Bounds = new Rectangle(Width - 60, 0, 60, Height),
            LayoutStyle = LayoutStyle.Right,
            TopOffset = Screen.RightBridge?.TopOffset ?? 0,
            BottomOffset = Screen.RightBridge?.BottomOffset ?? 0,
            //Visible = screen.RightBridge != null
        };
        if (Screen.RightBridge != null) {
            ToggleRight(Screen.RightBridge.TargetScreenId, forceEnable: true);
        }
    }


    public void ToggleLeft(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
    {
        if (!BarLeft.Visible && !forceDisable) {
            BarLeft.TargetScreenId = targetId ?? GetTargetScreenId(Screen.ScreenId, BridgePosition.Left);
            BtnLeft.Location = new Point(BtnLeft.Location.X + 20, BtnLeft.Location.Y);
            BtnLeft.Text = "-";
            BarLeft.Show();
        }
        else if (!forceEnable) {
            BtnLeft.Location = new Point(BtnLeft.Location.X - 20, BtnLeft.Location.Y);
            BtnLeft.Text = "+";
            BarLeft.Hide();
            if (!forceDisable) {
                this.RemoveBar?.Invoke(this, BridgePosition.Left, BarLeft.TargetScreenId);
            }
        }
    }

    public void ToggleRight(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
    {
        if (!BarRight.Visible && !forceDisable) {
            BarRight.TargetScreenId = targetId ?? GetTargetScreenId(Screen.ScreenId, BridgePosition.Right);
            BtnRight.Location = new Point(BtnRight.Location.X - 20, BtnRight.Location.Y);
            BtnRight.Text = "-";
            BarRight.Show();
        }
        else if (!forceEnable) {
            BtnRight.Location = new Point(BtnRight.Location.X + 20, BtnRight.Location.Y);
            BtnRight.Text = "+";
            BarRight.Hide();
            if (!forceDisable) {
                this.RemoveBar?.Invoke(this, BridgePosition.Right, BarLeft.TargetScreenId);
            }
        }
    }

    public void ToggleTop(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
    {
        if (!BarTop.Visible && !forceDisable) {
            BarTop.TargetScreenId = targetId ?? GetTargetScreenId(Screen.ScreenId, BridgePosition.Top);
            BtnTop.Location = new Point(BtnTop.Location.X, BtnTop.Location.Y + 20);
            BtnTop.Text = "-";
            BarTop.Show();
        }
        else if (!forceEnable) {
            BtnTop.Location = new Point(BtnTop.Location.X, BtnTop.Location.Y - 20);
            BtnTop.Text = "+";
            BarTop.Hide();
            if (!forceDisable) {
                this.RemoveBar?.Invoke(this, BridgePosition.Top, BarLeft.TargetScreenId);
            }
        }
    }

    public void ToggleBottom(int? targetId = null, bool forceEnable = false, bool forceDisable = false)
    {
        if (!BarBottom.Visible && !forceDisable) {
            BarBottom.TargetScreenId = targetId ?? GetTargetScreenId(Screen.ScreenId, BridgePosition.Bottom);
            BtnBottom.Location = new Point(BtnBottom.Location.X, BtnBottom.Location.Y - 20);
            BtnBottom.Text = "-";
            BarBottom.Show();
        }
        else if (!forceEnable) {
            BtnBottom.Location = new Point(BtnBottom.Location.X, BtnBottom.Location.Y + 20);
            BtnBottom.Text = "+";
            BarBottom.Hide();
            if (!forceDisable) {
                this.RemoveBar?.Invoke(this, BridgePosition.Bottom, BarLeft.TargetScreenId);
            }
        }
    }


    public TargetScreenIdGetter GetTargetScreenId { get; set; }

    public ScreenConfig GetConfig()
    {
        Screen.TopBridge = BarTop.Visible
            ? new Bridge() {
                TopOffset = BarTop.TopOffset,
                BottomOffset = BarTop.BottomOffset,
                TargetScreenId = BarTop.TargetScreenId
            }
            : null;

        Screen.BottomBridge = BarBottom.Visible
            ? new Bridge() {
                TopOffset = BarBottom.TopOffset,
                BottomOffset = BarBottom.BottomOffset,
                TargetScreenId = BarBottom.TargetScreenId
            }
            : null;

        Screen.LeftBridge = BarLeft.Visible
            ? new Bridge() {
                TopOffset = BarLeft.TopOffset,
                BottomOffset = BarLeft.BottomOffset,
                TargetScreenId = BarLeft.TargetScreenId
            }
            : null;

        Screen.RightBridge = BarRight.Visible
            ? new Bridge() {
                TopOffset = BarRight.TopOffset,
                BottomOffset = BarRight.BottomOffset,
                TargetScreenId = BarRight.TargetScreenId
            }
            : null;

        return Screen;
    }

    public void AddTargetBarForPosition(BridgePosition position, int sourceScreenId)
    {
        switch (position) {
            case BridgePosition.Top:
                ToggleBottom(sourceScreenId, forceEnable: true);
                break;
            case BridgePosition.Left:
                ToggleRight(sourceScreenId, forceEnable: true);
                break;
            case BridgePosition.Right:
                ToggleLeft(sourceScreenId, forceEnable: true);
                break;
            case BridgePosition.Bottom:
                ToggleTop(sourceScreenId, forceEnable: true);
                break;
        }
    }

    public void RemoveTargetBarForPosition(BridgePosition position)
    {
        switch (position) {
            case BridgePosition.Top:
                ToggleBottom(forceDisable: true);
                break;
            case BridgePosition.Left:
                ToggleRight(forceDisable: true);
                break;
            case BridgePosition.Right:
                ToggleLeft(forceDisable: true);
                break;
            case BridgePosition.Bottom:
                ToggleTop(forceDisable: true);
                break;
        }
    }
}

public delegate void RemoveBarEvent(ScreenConfigForm sender, BridgePosition position, int targetScreenId);

public delegate int TargetScreenIdGetter(int sourceScreenId, BridgePosition position);