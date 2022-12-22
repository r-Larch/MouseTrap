using System.ComponentModel;
using System.Reflection;
using MouseTrap.Installer;
using MouseTrap.Models;
using MouseTrap.Service;


// ReSharper disable LocalizableElement
namespace MouseTrap.Forms;

public partial class ConfigFrom : Form {
    public ServiceThread Service { get; set; }
    public ScreenConfigCollection Screens { get; set; }
    public Settings Settings { get; }

    public ConfigFrom(ServiceThread service)
    {
        this.Icon = App.Icon;

        Service = service;

        Screens = ScreenConfigCollection.Load();
        Settings = Settings.Load();
        Settings.Configured = Screens.Any(_ => _.HasBridges);

        InitializeComponent();

        this.Text += $" v{Assembly.GetEntryAssembly()?.GetName().Version}";

        this.ResizeRedraw = true;
        this.InfoText.Visible = Settings.Configured == false;
        this.BtnConfigure.Click += (s, e) => {
            ShowForms();
        };
        this.BtnDiagnostic.Click += (s, e) => {
            ShowDiagnosticForm();
        };
        this.EnableAutoStart.Checked = Settings.AutoStartEnabled;
        this.EnableAutoStart.CheckedChanged += delegate {
            if (EnableAutoStart.Checked) {
                Task.Run(() => new ProjectInstaller().Install());
                Settings.AutoStartEnabled = true;
            }
            else {
                Task.Run(() => new ProjectInstaller().Uninstall());
                Settings.AutoStartEnabled = false;
            }
        };
        if (!Settings.Configured && !Settings.AutoStartEnabled) {
            this.EnableAutoStart.Checked = true;
        }
    }


    private void MouseTrackTimer_Tick(object sender, EventArgs e)
    {
        CursorPosition.Text = $"{Cursor.Position.X}x{Cursor.Position.Y}";
    }


    private void ShowDiagnosticForm()
    {
        Settings.Save();

        new DiagnosticForm(Service).Show();
    }


    private void ShowForms()
    {
        Settings.Save();

        var forms = new List<ScreenConfigForm>();

        foreach (var screen in Screens) {
            var config = new ScreenConfig {
                Name = screen.Name,
                Bounds = screen.Bounds,
                Primary = screen.Primary,
                ScreenId = screen.ScreenId,
                TopBridge = screen.TopBridge,
                LeftBridge = screen.LeftBridge,
                RightBridge = screen.RightBridge,
                BottomBridge = screen.BottomBridge,
            };

            var form = new ScreenConfigForm(config) {
                GetTargetScreenId = GetTargetScreenId
            };

            form.RemoveBar += (s, position, targetScreenId) => {
                forms.SingleOrDefault(_ => _.Screen.ScreenId == targetScreenId)?.RemoveTargetBarForPosition(position);
            };

            form.TestBtn.Click += (s, e) => {
                Service.StopService();
                Service.StartService(new MouseBridgeService(GetConfig()));
                forms.ForEach(_ => _.TestBtn.Hide());
                forms.ForEach(_ => _.ResetBtn.Show());
            };
            form.ResetBtn.Click += (s, e) => {
                Service.RestoreOriginalState();
                forms.ForEach(_ => _.TestBtn.Show());
                forms.ForEach(_ => _.ResetBtn.Hide());
            };
            form.SaveBtn.Click += (s, e) => {
                var config = GetConfig();
                Settings.Configured = config.Any(_ => _.HasBridges);
                this.InfoText.Visible = Settings.Configured == false;
                config.Save();
                form.CancelBtn.PerformClick();
            };
            form.CancelBtn.Click += (s, e) => {
                forms.ForEach(_ => _.Close());
                forms.Clear();

                Service.RestoreOriginalState();
            };

            form.Show(this);
            forms.Add(form);
        }


        int GetTargetScreenId(int sourceScreenId, BridgePosition position)
        {
            var others = Screens.Where(_ => _.ScreenId != sourceScreenId).ToArray();

            var targetId = sourceScreenId;
            if (others.Length > 1) {
                targetId = Prompt.ChooseScreenDialog(Screens, sourceScreenId);
            }
            else if (others.Length == 1) {
                targetId = others.Single().ScreenId;
            }

            forms.Single(_ => _.Screen.ScreenId == targetId)
                .AddTargetBarForPosition(position, sourceScreenId);

            return targetId;
        }

        ScreenConfigCollection GetConfig()
        {
            return new ScreenConfigCollection(forms.Select(_ => _.GetConfig()));
        }
    }


    protected override void OnClosing(CancelEventArgs e)
    {
        Settings.Save();
        base.OnClosing(e);
    }
}

public enum BridgePosition {
    Top,
    Left,
    Right,
    Bottom
}
