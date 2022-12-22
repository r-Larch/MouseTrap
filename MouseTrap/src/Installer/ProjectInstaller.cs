namespace MouseTrap.Installer; 

public class ProjectInstaller : IInstaller {
    private readonly IInstaller _installer;

    public ProjectInstaller()
    {
        var config = InstallerConfig.Load();

        if (OperatingSystem.IsWindows()) {
            _installer = new WindowsInstaller(config);
        }
        else {
            throw new PlatformNotSupportedException();
        }
    }

    public void Install()
    {
        _installer.Install();
    }

    public void Uninstall()
    {
        _installer.Uninstall();
    }
}


public interface IInstaller {
    public void Install();
    public void Uninstall();
}