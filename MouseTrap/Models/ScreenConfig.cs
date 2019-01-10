using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace MouseTrap.Models {
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ScreenConfig {
        public Screen Screen;
        [JsonProperty]
        public int ScreenId { get; set; }
        public string ScreenNum => (ScreenId + 1).ToString();

        [JsonProperty]
        public string Name => Screen.DeviceFriendlyName();

        public bool HasBridges => TopBridge != null || LeftBridge != null || RightBridge != null || BottomBridge != null;

        [JsonProperty]
        public Bridge TopBridge { get; set; }
        [JsonProperty]
        public Bridge LeftBridge { get; set; }
        [JsonProperty]
        public Bridge RightBridge { get; set; }
        [JsonProperty]
        public Bridge BottomBridge { get; set; }

        private const int space = 2;

        public Rectangle RightHotSpace => RightBridge != null
            ? new Rectangle(
                Screen.Bounds.X + Screen.Bounds.Width - space,
                Screen.Bounds.Y + RightBridge.TopOffset,
                space,
                Screen.Bounds.Height - RightBridge.TopOffset - RightBridge.BottomOffset
            )
            : Rectangle.Empty;

        public Rectangle LeftHotSpace => LeftBridge != null
            ? new Rectangle(
                Screen.Bounds.X,
                Screen.Bounds.Y + LeftBridge.TopOffset,
                space,
                Screen.Bounds.Height - LeftBridge.TopOffset - LeftBridge.BottomOffset
            )
            : Rectangle.Empty;

        public Rectangle TopHotSpace => TopBridge != null
            ? new Rectangle(
                Screen.Bounds.X + TopBridge.TopOffset,
                Screen.Bounds.Y,
                Screen.Bounds.Width - TopBridge.TopOffset - TopBridge.BottomOffset,
                space
            )
            : Rectangle.Empty;

        public Rectangle BottomHotSpace => BottomBridge != null
            ? new Rectangle(
                Screen.Bounds.X + BottomBridge.TopOffset,
                Screen.Bounds.Y + Screen.Bounds.Height - space,
                Screen.Bounds.Width - BottomBridge.TopOffset - BottomBridge.BottomOffset,
                space
            )
            : Rectangle.Empty;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Bridge {
        [JsonProperty]
        public int TopOffset { get; set; }
        [JsonProperty]
        public int BottomOffset { get; set; }
        [JsonProperty]
        public int TargetScreenId { get; set; }
    }
}
