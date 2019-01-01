using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;


namespace MouseTrap.Models {
    [Serializable]
    public class ScreenConfigCollection : List<ScreenConfig> {
        public ScreenConfigCollection()
        {
        }

        public ScreenConfigCollection(IEnumerable<ScreenConfig> enumerable) : base(enumerable)
        {
        }


        #region Reload on configuration change

        private static readonly List<WeakReference> Instances;

        static ScreenConfigCollection()
        {
            Instances = new List<WeakReference>();
            SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
        }

        public static void StaticDispose()
        {
            SystemEvents.DisplaySettingsChanged -= DisplaySettingsChanged;
            Instances.Clear();
        }

        private static void DisplaySettingsChanged(object sender, EventArgs eventArgs)
        {
            lock (StaticReloadLock) {
                Instances.RemoveAll(x => !x.IsAlive);
                // create a new list to iterate over, because the original gets modified while iterating
                var current = Instances.ToList();
                foreach (var weakReference in current) {
                    if (weakReference.IsAlive && weakReference.Target is ScreenConfigCollection target) {
                        target.Reload();
                    }
                }
            }
        }

        private static readonly object StaticReloadLock = new object();
        private readonly object _reloadLock = new object();

        private void Reload()
        {
            lock (_reloadLock) {
                var loaded = Load();

                var toUpdate = this.Where(x => loaded.Select(_ => _.ScreenId).Contains(x.ScreenId)).ToArray();
                var toRemove = new List<ScreenConfig>();

                for (var i = 0; i < this.Count; i++) {
                    if (toUpdate.Contains(this[i])) {
                        this[i] = loaded.First(_ => _.ScreenId == this[i].ScreenId);
                    }
                    else {
                        toRemove.Add(this[i]);
                    }
                }

                foreach (var config in toRemove) {
                    this.Remove(config);
                }
            }
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
                var config = loaded.FirstOrDefault(x => x.ScreenId == i) ?? new ScreenConfig() {ScreenId = i};
                config.Screen = screens[i];

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

            // auto update this instance if system configuration changes
            Instances.Add(new WeakReference(obj));

            return obj;
        }
    }
}
