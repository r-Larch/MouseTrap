using Newtonsoft.Json;


namespace MouseTrap.Models;

public class SettingsFile {
    protected static string SavePath(string name) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), App.Name, name + ".json");

    public static void Save<T>(T obj, string? filename = null)
    {
        var path = SavePath(filename ?? typeof(T).Name);

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, JsonSettings);

        if (!Directory.Exists(Path.GetDirectoryName(path))) {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        }

        File.WriteAllText(path, json);
    }

    public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
        NullValueHandling = NullValueHandling.Ignore
    };

    public static T Load<T>(string? fileName = null) where T : class, new()
    {
        var path = SavePath(fileName ?? typeof(T).Name);
        var json = File.Exists(path) ? File.ReadAllText(path) : null;

        return json != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json) ?? new T() : new T();
    }
}
