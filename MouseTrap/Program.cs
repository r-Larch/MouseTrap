using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using MouseTrap.Properties;


namespace MouseTrap {
    public class Program {
        public static void Main(string[] args)
        {
            var worker = new TrayWorker<MouseBrigeWorker>();
            worker.TrayIcon = new NotifyIcon {
                Icon = Resources.AppIcon,
                Text = nameof(MouseTrap),
                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Info", (s, e) => new ConfigFrom(worker).Show()) {DefaultItem = true},
                    new MenuItem("Reinit", (s, e) => worker.RestartWorker()),
                    new MenuItem("Full restart", (s, e) => FullRestart(worker)),
                    new MenuItem("Exit", (s, e) => worker.Exit()),
                }),
                Visible = true
            };
            worker.Start();
        }

        private static void FullRestart(TrayWorker<MouseBrigeWorker> worker)
        {
            worker.Exit();
            worker.Dispose();
            Process.Start(Assembly.GetExecutingAssembly().CodeBase);
            Environment.Exit(0);
        }
    }
}
