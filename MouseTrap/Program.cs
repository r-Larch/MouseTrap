using System;
using System.Linq;
using System.Windows.Forms;
using MouseTrap.Models;


// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming
namespace MouseTrap {
    public class Program {
        [STAThread]
        public static void Main(string[] args)
        {
            switch (args.FirstOrDefault()) {
                case "-i":
                    ProjectInstaller.Install();
                    return;
                case "-u":
                    MutexRunner.CloseRunningInstance();
                    ProjectInstaller.Uninstall();
                    return;
                default:
                    StartProgram();
                    return;
            }
        }


        public static void StartProgram()
        {
            var success = MutexRunner.MutexRun(RunUI);
            if (!success) {
                MutexRunner.NotifyRunningInstance();
            }
        }


        private static void RunUI()
        {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);

                var service = new ServiceThread {
                    ServiceFactory = () => new MouseBridgeService()
                };

                var tray = new MouseTrapTrayIcon(service);

                var app = new TrayApplication(tray);
                app.BeforeStart += (s, e) => {
                    service.StartService();
                };
                app.BeforeExit += (s, e) => {
                    service.StopService();
                };
                app.Start();
            }
            catch (Exception e) {
                Logger.Error(e.Message, e);
                MessageBox.Show($"[{e.GetType().FullName}] {e.Message}\r\n{e.StackTrace}", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
