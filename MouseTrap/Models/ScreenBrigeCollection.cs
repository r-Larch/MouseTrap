using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace MouseTrap.Models {
    [Serializable]
    public class ScreenBrigesCollection : List<ScreenBriges> {
        public ScreenBrigesCollection()
        {
        }

        public ScreenBrigesCollection(IEnumerable<ScreenBriges> enumerable) : base(enumerable)
        {
        }

        public void Save()
        {
            SettingsFile.Save(this);
        }

        public static ScreenBrigesCollection Load()
        {
            var obj = SettingsFile.Load<ScreenBrigesCollection>();

            var screens = Screen.AllScreens;

            for (var i = 0; i < screens.Length; i++) {
                var index = obj.FindIndex(x => x.ScreenId == i);
                if (index == -1) {
                    index = obj.Count;
                    obj.Add(new ScreenBriges() {ScreenId = i});
                }

                obj[index].Screen = screens[i];
            }

            return obj;
        }
    }
}
