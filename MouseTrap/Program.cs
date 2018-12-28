using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using MouseTrap.Properties;


namespace MouseTrap {
    public class Program {
        public static void Main(string[] args)
        {
            //Application.Run(new ConfigFrom());

            //while (true) {
            //    Debug.WriteLine(Cursor.Position);
            //    Thread.Sleep(1);
            //}

            var worker = new TrayWorker(new MouseBrige().Run);
            worker.TrayIcon = new NotifyIcon {
                Icon = Resources.AppIcon,
                Text = nameof(MouseTrap),
                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Info", (s, e) => new ConfigFrom().Show()) {DefaultItem = true},
                    new MenuItem("Reinit", (s, e) => worker.RestartWorker()),
                    new MenuItem("Full restart", (s, e) => FullRestart(worker)),
                    new MenuItem("Exit", (s, e) => worker.Exit()),
                }),
                Visible = true
            };
            worker.Start();
        }

        private static void FullRestart(TrayWorker worker)
        {
            worker.Exit();
            worker.Dispose();
            Process.Start(Assembly.GetExecutingAssembly().CodeBase);
            Environment.Exit(0);
        }
    }
}
