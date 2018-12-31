using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace MouseTrap.Models {
    [Serializable]
    public class ScreenConfigCollection : List<ScreenConfig> {
        public ScreenConfigCollection()
        {
        }

        public ScreenConfigCollection(IEnumerable<ScreenConfig> enumerable) : base(enumerable)
        {
        }

        public void Save()
        {
            SettingsFile.Save(this);
        }

        public static ScreenConfigCollection Load()
        {
            var obj = SettingsFile.Load<ScreenConfigCollection>() ?? new ScreenConfigCollection();

            var screens = Screen.AllScreens;

            for (var i = 0; i < screens.Length; i++) {
                var index = obj.FindIndex(x => x.ScreenId == i);
                if (index == -1) {
                    index = obj.Count;
                    obj.Add(new ScreenConfig() {ScreenId = i});
                }

                obj[index].Screen = screens[i];
            }

            return obj;
        }
    }
}
