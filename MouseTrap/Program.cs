using MouseTrap.Installer;
using MouseTrap.Models;
using MouseTrap.Service;


// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming
namespace MouseTrap;

public class Program {
    [STAThread]
    public static void Main(string[] args)
    {
        switch (args.FirstOrDefault()) {
            case "-i":
                new ProjectInstaller().Install();
                return;
            case "-u":
                MutexRunner.CloseRunningInstance();
                new ProjectInstaller().Uninstall();
                return;
            case "--reinit":
                ServiceThread.NotifyRestartWorker();
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
            // It is unfortunate but we have to set it to Unknown first.
            //Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
            //Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            ApplicationConfiguration.Initialize();

            var service = new ServiceThread {
                ServiceFactory = () => new MouseBridgeService()
            };

            var tray = new MouseTrapTrayIcon(service);

            var app = new TrayApplication(tray);
            app.BeforeStart += (s, e) => {
                if (Settings.Load().TeleportationActive) {
                    service.StartService();
                }
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