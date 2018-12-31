using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using MouseTrap.Forms;
using MouseTrap.Models;


// ReSharper disable LocalizableElement
namespace MouseTrap {
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
            Settings.Configured = Screens.Any(_ => _.HasBriges);

            InitializeComponent();
            this.ResizeRedraw = true;
            this.InfoText.Visible = Settings.Configured == false;
            this.BtnConfigure.Click += (s, e) => { ShowForms(); };
            this.EnableAutoStart.Checked = Settings.AutoStartEnabled;
            this.EnableAutoStart.CheckedChanged += (s, e) => {
                var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
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

        private void ShowForms()
        {
            foreach (var screen in Screens) {
                var form = new ScreenConfigForm(screen) {
                    GetTargetScreenId = (sourceScreen, position) => {
                        var others = Screens.Where(_ => _ != sourceScreen).ToArray();
                        var target = others.Length > 1 ? Prompt.ChooseScreenDialog(Screens, sourceScreen) : others.Single();
                        //var target = Prompt.ChooseScreenDialog(screens, exclude: sourceScreen);

                        Forms.Single(_ => _.Screen.ScreenId == target.ScreenId)
                            .AddTargetBarForPosition(position, sourceScreen.ScreenId);

                        return target.ScreenId;
                    }
                };

                form.RemoveBar += (s, position, targetScreenId) => {
                    Forms.Single(_ => _.Screen.ScreenId == targetScreenId)
                        .RemoveTargetBarForPosition(position, targetScreenId);
                };

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
                    var config = GetConfig();
                    Settings.Configured = config.Any(_ => _.HasBriges);
                    this.InfoText.Visible = Settings.Configured == false;
                    config.Save();
                    Worker.RestartWorker();
                };
                form.CancelBtn.Click += (s, e) => { Forms.ForEach(_ => _.Close()); };

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
