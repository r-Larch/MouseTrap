using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MouseTrap.Models {
    [Serializable]
    public class Settings {



        public void Save()
        {
            SettingsFile.Save(this);
        }

        public static Settings Load()
        {
            return SettingsFile.Load<Settings>() ?? new Settings();
        }
    }
}
