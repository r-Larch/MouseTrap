using MouseTrap.Forms;
using MouseTrap.Models;
using MouseTrap.Service;


namespace MouseTrap {
    public class MouseTrapTrayIcon : TrayIcon {
        private readonly ServiceThread _service;
        private WeakReference<ConfigFrom>? _configFromRef;


        public MouseTrapTrayIcon(ServiceThread service)
        {
            _service = service;

            Icon = App.Icon;
            Text = App.Name;

            ContextMenu.Items.Add(new ToolStripMenuItem("Settings", null, (s, e) => OpenSettings()) {
                ToolTipText = "Open configuration screen",
            });
            ContextMenu.Items.Add(new ToolStripMenuItem("Mouse teleportation", null, (sender, args) => ToggleTeleportation((ToolStripMenuItem) sender!)) {
                Checked = true,
                CheckOnClick = true,
                ToolTipText = "Turn off mouse teleportation e.g. while gaming",
            });
            ContextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) => Close()) {
                ToolTipText = "Fully exit MouseTrap process",
            });

            // make first option bold
            ContextMenu.Items[0].Font = WithFontStyle(ContextMenu.Items[0].Font, FontStyle.Bold);

            Visible = true;

            // show config form on first startup
            var settings = Settings.Load();
            if (!settings.Configured) {
                OpenSettings();
            }
        }


        private void ToggleTeleportation(ToolStripMenuItem checkBox)
        {
            if (checkBox.Checked) {
                _service.StartService();
            }
            else {
                _service.StopService();
            }
        }


        public void OpenSettings()
        {
            if (_configFromRef == null || !_configFromRef.TryGetTarget(out var configFrom) || configFrom.Disposing || configFrom.IsDisposed) {
                configFrom = new ConfigFrom(_service);
                configFrom.Show();
                _configFromRef = new WeakReference<ConfigFrom>(configFrom);
            }
            else {
                configFrom.Activate();
                configFrom.TopMost = true;
                configFrom.TopMost = false;
            }
        }


        protected override void WndProc(ref Message m)
        {
            if (MutexRunner.IsMutexMessageOpen(ref m)) {
                OpenSettings();
            }
            else if (MutexRunner.IsMutexMessageClose(ref m)) {
                Close();
            }

            _service.WndProc(ref m);

            base.WndProc(ref m);
        }


        /// <summary>
        /// A tiny helper to set font style!
        /// </summary>
        private static Font WithFontStyle(Font font, FontStyle style)
        {
            return new Font(font.Name, font.Size, style, font.Unit);
        }
    }
}
