using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MouseTrap.Forms;
using MouseTrap.Models;


// ReSharper disable LocalizableElement
namespace MouseTrap {
    public partial class ConfigFrom : Form {
        public TrayWorker<MouseBrigeWorker> Worker { get; set; }
        public List<ScreenConfigForm> Forms { get; set; }

        public ConfigFrom(TrayWorker<MouseBrigeWorker> worker)
        {
            InitializeComponent();
            ResizeRedraw = true;
            BtnConfigure.Click += (s, e) => { ShowForms(); };
            Worker = worker;
            Forms = new List<ScreenConfigForm>();
        }

        private void MouseTrackTimer_Tick(object sender, EventArgs e)
        {
            CursorPosition.Text = $"{Cursor.Position.X}x{Cursor.Position.Y}";
        }

        private void ShowForms()
        {
            var screens = ScreenConfigCollection.Load();

            foreach (var screen in screens) {
                var form = new ScreenConfigForm(screen) {
                    GetTargetScreenId = (sourceScreen, position) => {
                        var others = screens.Where(_ => _ != sourceScreen).ToArray();
                        var target = others.Length > 1 ? Prompt.ChooseScreenDialog(screens, sourceScreen) : others.Single();
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
                    GetConfig().Save();
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
    }

    public enum BrigePosition {
        Top,
        Left,
        Right,
        Bottom
    }
}
