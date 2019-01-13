using System;
using System.IO;
using System.Windows.Forms;


namespace MouseTrap.Models {
    public class SettingsFile {
        protected static string SavePath(string name) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), App.Name, name + ".json");

        public static void Save<T>(T obj)
        {
            var path = SavePath(typeof(T).Name);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            File.WriteAllText(path, json);
        }

        public static T Load<T>() where T : class, new()
        {
            var path = SavePath(typeof(T).Name);
            var json = File.Exists(path) ? File.ReadAllText(path) : null;

            return json != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json) : new T();
        }
    }
}
