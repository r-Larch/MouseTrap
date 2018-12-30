using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace MouseTrap.Models {
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ScreenBriges {
        public Screen Screen;
        [JsonProperty]
        public int ScreenId { get; set; }
        public string ScreenNum => (ScreenId + 1).ToString();

        public bool HasBriges => TopBrige != null || LeftBrige != null || RightBrige != null || BottomBrige != null;

        [JsonProperty]
        public Brige TopBrige { get; set; }
        [JsonProperty]
        public Brige LeftBrige { get; set; }
        [JsonProperty]
        public Brige RightBrige { get; set; }
        [JsonProperty]
        public Brige BottomBrige { get; set; }

        private const int space = 1;

        public Rectangle RightHotSpace => RightBrige != null
            ? new Rectangle(
                Screen.Bounds.X + Screen.Bounds.Width - space,
                Screen.Bounds.Y + RightBrige.TopOffset,
                space,
                Screen.Bounds.Height - RightBrige.TopOffset - RightBrige.BottomOffset
            )
            : Rectangle.Empty;

        public Rectangle LeftHotSpace => LeftBrige != null
            ? new Rectangle(
                Screen.Bounds.X,
                Screen.Bounds.Y + LeftBrige.TopOffset,
                space,
                Screen.Bounds.Height - LeftBrige.TopOffset - LeftBrige.BottomOffset
            )
            : Rectangle.Empty;

        public Rectangle TopHotSpace => TopBrige != null
            ? new Rectangle(
                Screen.Bounds.X + TopBrige.TopOffset,
                Screen.Bounds.Y,
                Screen.Bounds.Width - TopBrige.TopOffset - TopBrige.BottomOffset,
                space
            )
            : Rectangle.Empty;

        public Rectangle BottomHotSpace => BottomBrige != null
            ? new Rectangle(
                Screen.Bounds.X + BottomBrige.TopOffset,
                Screen.Bounds.Y + Screen.Bounds.Height - space,
                Screen.Bounds.Width - BottomBrige.TopOffset - BottomBrige.BottomOffset,
                space
            )
            : Rectangle.Empty;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Brige {
        [JsonProperty]
        public int TopOffset { get; set; }
        [JsonProperty]
        public int BottomOffset { get; set; }
        [JsonProperty]
        public int TargetScreenId { get; set; }
    }
}
