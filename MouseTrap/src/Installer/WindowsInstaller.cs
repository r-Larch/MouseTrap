using System.Runtime.Versioning;


namespace MouseTrap.Installer;

[SupportedOSPlatform("windows")]
public class WindowsInstaller : IInstaller {
    private readonly InstallerConfig _config;
    private readonly TaskInstaller _taskInstaller;


    public WindowsInstaller(InstallerConfig config)
    {
        _config = config;

        var user = System.Security.Principal.WindowsIdentity.GetCurrent();

        _taskInstaller = new TaskInstaller {
            TaskName = $"\\{App.Name}\\{user.Name.Replace("\\", "_")}",
            // task
            ExecutablePath = Application.ExecutablePath,
            Arguments = null,
            // user
            RunAsUser = user,
            HighestPrivileges = true,
        };
    }

    public void Install()
    {
        _taskInstaller.Install(_config.State<TaskInstaller>());

        _config.Save();
    }

    public void Uninstall()
    {
        _taskInstaller.Uninstall(_config.State<TaskInstaller>());

        _config.Save();
    }
}
