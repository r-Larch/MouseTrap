using System;
using System.Diagnostics;
using System.Windows.Forms;
using MouseTrap.Forms;
using MouseTrap.Models;
using MouseTrap.Properties;


// ReSharper disable LocalizableElement
namespace MouseTrap {
    public class Program {
        public static void Main(string[] args)
        {

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            try {
                var worker = new TrayWorker<MouseBrigeWorker>();
                worker.TrayIcon = new NotifyIcon {
                    Icon = Resources.AppIcon,
                    Text = nameof(MouseTrap),
                    ContextMenu = new ContextMenu(new[] {
                        new MenuItem("Settings", (s, e) => new ConfigFrom(worker).Show()) {DefaultItem = true},
                        new MenuItem("Reinit", (s, e) => worker.RestartWorker()),
                        new MenuItem("Full restart", (s, e) => FullRestart(worker)),
                        new MenuItem("Exit", (s, e) => worker.Exit()),
                    }),
                    Visible = true
                };

                // show config form on first startup
                var settings = Settings.Load();
                if (!settings.Configured) {
                    worker.MainForm = new ConfigFrom(worker);
                }

                worker.Start();
            }
            catch (Exception e) {
                MessageBox.Show($"[{e.GetType().FullName}] {e.Message}\r\n{e.StackTrace}", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally {
                ScreenConfigCollection.StaticDispose();
            }
        }

        private static void FullRestart(TrayWorker<MouseBrigeWorker> worker)
        {
            worker.Exit();
            worker.Dispose();
            Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }
    }
}
