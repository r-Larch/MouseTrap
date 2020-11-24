using System;
using System.Drawing;
using Newtonsoft.Json;


namespace MouseTrap.Models {
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ScreenConfig {
        [JsonProperty]
        public int ScreenId { get; set; }
        public string ScreenNum => (ScreenId + 1).ToString();

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public bool Primary { get; set; }

        public bool HasBridges => TopBridge != null || LeftBridge != null || RightBridge != null || BottomBridge != null;

        [JsonProperty]
        public Bridge TopBridge { get; set; }
        [JsonProperty]
        public Bridge LeftBridge { get; set; }
        [JsonProperty]
        public Bridge RightBridge { get; set; }
        [JsonProperty]
        public Bridge BottomBridge { get; set; }

        [JsonProperty] public Rectangle Bounds;


        private const int Space = 2;

        public Rectangle RightHotSpace => RightBridge != null
            ? new Rectangle(
                Bounds.X + Bounds.Width - Space,
                Bounds.Y + RightBridge.TopOffset,
                Space,
                Bounds.Height - RightBridge.TopOffset - RightBridge.BottomOffset
            )
            : Rectangle.Empty;

        public Rectangle LeftHotSpace => LeftBridge != null
            ? new Rectangle(
                Bounds.X,
                Bounds.Y + LeftBridge.TopOffset,
                Space,
                Bounds.Height - LeftBridge.TopOffset - LeftBridge.BottomOffset
            )
            : Rectangle.Empty;

        public Rectangle TopHotSpace => TopBridge != null
            ? new Rectangle(
                Bounds.X + TopBridge.TopOffset,
                Bounds.Y,
                Bounds.Width - TopBridge.TopOffset - TopBridge.BottomOffset,
                Space
            )
            : Rectangle.Empty;

        public Rectangle BottomHotSpace => BottomBridge != null
            ? new Rectangle(
                Bounds.X + BottomBridge.TopOffset,
                Bounds.Y + Bounds.Height - Space,
                Bounds.Width - BottomBridge.TopOffset - BottomBridge.BottomOffset,
                Space
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
