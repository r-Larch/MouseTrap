using System.Collections.Generic;
using MouseTrap.Models;
using Newtonsoft.Json;


namespace MouseTrap.Installer {
    public class InstallerConfig {
        private Dictionary<string, IDictionary<string, string>> InstallerState { get; set; }

        public static InstallerConfig Load()
        {
            return new InstallerConfig {
                InstallerState = SettingsFile.Load<Dictionary<string, IDictionary<string, string>>>(nameof(InstallerState)),
            };
        }

        public void Save()
        {
            SettingsFile.Save(InstallerState, nameof(InstallerState));
        }


        public InstallerState State<T>()
        {
            var key = typeof(T).Name;
            if (!InstallerState.TryGetValue(key, out var state)) {
                InstallerState.Add(key, state = new Dictionary<string, string>());
            }

            return new InstallerState(state);
        }
    }


    public class InstallerState {
        private readonly IDictionary<string, string> _value;
        public InstallerState(IDictionary<string, string> value) => _value = value;

        public string this[string key] {
            get => _value.TryGetValue(key, out var state) ? state : default;
            set {
                if (!_value.TryAdd(key, value)) _value[key] = value;
            }
        }
    }
}
