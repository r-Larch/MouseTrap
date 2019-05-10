using System;
using System.Windows.Forms;
using MouseTrap.Forms;
using MouseTrap.Models;


namespace MouseTrap {
    public class MouseTrapTrayIcon : TrayIcon {
        public ServiceThread Service;

        public MouseTrapTrayIcon(ServiceThread service)
        {
            Service = service;

            Icon = App.Icon;
            Text = App.Name;
            ContextMenu = new ContextMenu(new[] {
                new MenuItem("Settings", (s, e) => OpenSettings()) {DefaultItem = true},
                new MenuItem("Reinit", (s, e) => Reinit()),
                new MenuItem("Exit", (s, e) => Close()),
            });
            Visible = true;

            // show config form on first startup
            var settings = Settings.Load();
            if (!settings.Configured) {
                OpenSettings();
            }
        }

        private WeakReference<ConfigFrom> _configFromRef;

        public void OpenSettings()
        {
            if (_configFromRef == null || !_configFromRef.TryGetTarget(out var configFrom) || configFrom.Disposing || configFrom.IsDisposed) {
                configFrom = new ConfigFrom(Service);
                configFrom.Show();
                _configFromRef = new WeakReference<ConfigFrom>(configFrom);
            }
            else {
                configFrom.Activate();
                configFrom.TopMost = true;
                configFrom.TopMost = false;
            }
        }

        public void Reinit()
        {
            Service.RestartService();
        }


        protected override void WndProc(ref Message m)
        {
            if (MutexRunner.IsMutexMessageOpen(ref m)) {
                OpenSettings();
            }
            else if (MutexRunner.IsMutexMessageClose(ref m)) {
                Close();
            }

            Service.WndProc(ref m);

            base.WndProc(ref m);
        }
    }
}
