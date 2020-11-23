using System;
using System.Drawing;
using System.Windows.Forms;
using MouseTrap.Forms;
using MouseTrap.Models;
using MouseTrap.Service;


namespace MouseTrap {
    public class MouseTrapTrayIcon : TrayIcon {
        public ServiceThread Service;

        public MouseTrapTrayIcon(ServiceThread service)
        {
            Service = service;

            Icon = App.Icon;
            Text = App.Name;

            ContextMenu.Items.Add("Settings", null, (s, e) => OpenSettings());
            ContextMenu.Items.Add("Reinit", null, (s, e) => Reinit());
            ContextMenu.Items.Add("Exit", null, (s, e) => Close());

            // make first option bold
            ContextMenu.Items[0].Font = WithFontStyle(ContextMenu.Items[0].Font, FontStyle.Bold);

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


        /// <summary>
        /// A tiny helper to set font style!
        /// </summary>
        private static Font WithFontStyle(Font font, FontStyle style)
        {
            return new Font(font.Name, font.Size, style, font.Unit);
        }
    }
}
