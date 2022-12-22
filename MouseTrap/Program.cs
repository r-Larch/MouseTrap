using MouseTrap;
using MouseTrap.Installer;
using MouseTrap.Models;
using MouseTrap.Service;


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


static void StartProgram()
{
    var success = MutexRunner.MutexRun(RunUI);
    if (!success) {
        MutexRunner.NotifyRunningInstance();
    }
}


static void RunUI()
{
    try {
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
