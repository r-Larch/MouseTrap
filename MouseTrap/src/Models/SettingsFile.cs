using System.Text.Json;
using System.Text.Json.Serialization;


namespace MouseTrap.Models;

public class SettingsFile {
    protected static string SavePath(string name) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), App.Name, name + ".json");

    public static void Save<T>(T obj, string? filename = null)
    {
        var path = SavePath(filename ?? typeof(T).Name);

        var json = JsonSerializer.Serialize(obj, JsonSettings);

        if (!Directory.Exists(Path.GetDirectoryName(path))) {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        }

        File.WriteAllText(path, json);
    }

    private static readonly JsonSerializerOptions JsonSettings = new() {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static T Load<T>(string? fileName = null) where T : class, new()
    {
        var path = SavePath(fileName ?? typeof(T).Name);
        var json = File.Exists(path) ? File.ReadAllText(path) : null;

        return json != null ? JsonSerializer.Deserialize<T>(json) ?? new T() : new T();
    }
}
