using Microsoft.Win32;


namespace MouseTrap.Models;

[Serializable]
public class ScreenConfigCollection : List<ScreenConfig> {
    public ScreenConfigCollection()
    {
    }

    public ScreenConfigCollection(IEnumerable<ScreenConfig> enumerable) : base(enumerable)
    {
    }


    #region Reload on configuration change

    static ScreenConfigCollection()
    {
        if (OperatingSystem.IsWindows()) {
            SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
        }
    }

    public static event ScreenConfigChanged? OnChanged;

    private static void DisplaySettingsChanged(object? sender, EventArgs eventArgs)
    {
        var configCollection = Load();
        OnChanged?.Invoke(configCollection);
    }

    #endregion


    public void Save()
    {
        SettingsFile.Save(this);
    }

    public static ScreenConfigCollection Load()
    {
        var loaded = SettingsFile.Load<ScreenConfigCollection>();

        var screens = Screen.AllScreens;

        var obj = new ScreenConfigCollection();
        for (var i = 0; i < screens.Length; i++) {
            var config = loaded.FirstOrDefault(x => x.ScreenId == i) ?? new ScreenConfig() { ScreenId = i };

            config.Name = screens[i].DeviceFriendlyName();
            config.Bounds = screens[i].Bounds;
            config.Primary = screens[i].Primary;

            // if TargetScreen does not exist anymore
            if (config.TopBridge != null && config.TopBridge.TargetScreenId >= screens.Length) {
                config.TopBridge = null;
            }

            if (config.LeftBridge != null && config.LeftBridge.TargetScreenId >= screens.Length) {
                config.LeftBridge = null;
            }

            if (config.RightBridge != null && config.RightBridge.TargetScreenId >= screens.Length) {
                config.RightBridge = null;
            }

            if (config.BottomBridge != null && config.BottomBridge.TargetScreenId >= screens.Length) {
                config.BottomBridge = null;
            }

            obj.Add(config);
        }

        return obj;
    }
}

public delegate void ScreenConfigChanged(ScreenConfigCollection config);
