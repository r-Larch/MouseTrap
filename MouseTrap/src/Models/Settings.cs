namespace MouseTrap.Models; 

[Serializable]
public class Settings {
    public void Save()
    {
        SettingsFile.Save(this);
    }

    public static Settings Load()
    {
        return SettingsFile.Load<Settings>();
    }

    public bool AutoStartEnabled { get; set; }
    public bool Configured { get; set; }
    public bool TeleportationActive { get; set; } = true;
}