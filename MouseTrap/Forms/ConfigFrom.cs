using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using MouseTrap.Models;


// ReSharper disable LocalizableElement
namespace MouseTrap.Forms {
    public partial class ConfigFrom : Form {
        public TrayWorker<MouseBrigeWorker> Worker { get; set; }
        public List<ScreenConfigForm> Forms { get; set; }
        public ScreenConfigCollection Screens { get; set; }
        public Settings Settings { get; }

        public ConfigFrom(TrayWorker<MouseBrigeWorker> worker)
        {
            Worker = worker;
            Forms = new List<ScreenConfigForm>();
            Screens = ScreenConfigCollection.Load();
            Settings = Settings.Load();
            Settings.Configured = Screens.Any(_ => _.HasBridges);

            InitializeComponent();
            this.ResizeRedraw = true;
            this.InfoText.Visible = Settings.Configured == false;
            this.BtnConfigure.Click += (s, e) => { ShowForms(); };
            this.EnableAutoStart.Checked = Settings.AutoStartEnabled;
            this.EnableAutoStart.CheckedChanged += (s, e) => {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (EnableAutoStart.Checked) {
                    key?.SetValue(nameof(MouseTrap), Application.ExecutablePath);
                    Settings.AutoStartEnabled = true;
                }
                else {
                    key?.DeleteValue(nameof(MouseTrap), throwOnMissingValue: false);
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

        private int GetTargetScreenId(int sourceScreenId, BrigePosition position)
        {
            var others = Screens.Where(_ => _.ScreenId != sourceScreenId).ToArray();

            var targetId = sourceScreenId;
            if (others.Length > 1) {
                targetId = Prompt.ChooseScreenDialog(Screens, sourceScreenId);
            }
            else if (others.Length == 1) {
                targetId = others.Single().ScreenId;
            }

            Forms.Single(_ => _.Screen.ScreenId == targetId)
                .AddTargetBarForPosition(position, sourceScreenId);

            return targetId;
        }

        private void ShowForms()
        {
            foreach (var screen in Screens) {
                var form = new ScreenConfigForm(screen) {
                    GetTargetScreenId = GetTargetScreenId
                };

                form.RemoveBar += (s, position, targetScreenId) => { Forms.SingleOrDefault(_ => _.Screen.ScreenId == targetScreenId)?.RemoveTargetBarForPosition(position); };

                ResetWorker test = null;
                form.TestBtn.Click += (s, e) => {
                    test = Worker.SwapWorker(new MouseBrigeWorker(GetConfig()));
                    Forms.ForEach(_ => _.TestBtn.Hide());
                    Forms.ForEach(_ => _.ResetBtn.Show());
                };
                form.ResetBtn.Click += (s, e) => {
                    test?.Reset();
                    Forms.ForEach(_ => _.TestBtn.Show());
                    Forms.ForEach(_ => _.ResetBtn.Hide());
                };
                form.SaveBtn.Click += (s, e) => {
                    form.ResetBtn.PerformClick();
                    var config = GetConfig();
                    Settings.Configured = config.Any(_ => _.HasBridges);
                    this.InfoText.Visible = Settings.Configured == false;
                    config.Save();
                    Worker.RestartWorker();
                };
                form.CancelBtn.Click += (s, e) => {
                    form.ResetBtn.PerformClick();
                    Forms.ForEach(_ => _.Close());
                };

                form.Show(this);
                Forms.Add(form);
            }
        }


        public ScreenConfigCollection GetConfig()
        {
            return new ScreenConfigCollection(Forms.Select(_ => _.GetConfig()));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Save();
            base.OnClosing(e);
        }
    }

    public enum BrigePosition {
        Top,
        Left,
        Right,
        Bottom
    }
}
