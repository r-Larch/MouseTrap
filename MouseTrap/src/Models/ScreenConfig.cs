using System.Text.Json.Serialization;


namespace MouseTrap.Models;

[Serializable]
public class ScreenConfig {
    public int ScreenId { get; set; }
    [JsonIgnore]
    public string ScreenNum => (ScreenId + 1).ToString();

    public string? Name { get; set; }

    public bool Primary { get; set; }

    [JsonIgnore]
    public bool HasBridges => TopBridge != null || LeftBridge != null || RightBridge != null || BottomBridge != null;

    public Bridge? TopBridge { get; set; }
    public Bridge? LeftBridge { get; set; }
    public Bridge? RightBridge { get; set; }
    public Bridge? BottomBridge { get; set; }

    public Rectangle Bounds;


    private const int Space = 2;

    [JsonIgnore]
    public Rectangle RightHotSpace => RightBridge != null
        ? new Rectangle(
            Bounds.X + Bounds.Width - Space,
            Bounds.Y + RightBridge.TopOffset,
            Space,
            Bounds.Height - RightBridge.TopOffset - RightBridge.BottomOffset
        )
        : Rectangle.Empty;

    [JsonIgnore]
    public Rectangle LeftHotSpace => LeftBridge != null
        ? new Rectangle(
            Bounds.X,
            Bounds.Y + LeftBridge.TopOffset,
            Space,
            Bounds.Height - LeftBridge.TopOffset - LeftBridge.BottomOffset
        )
        : Rectangle.Empty;

    [JsonIgnore]
    public Rectangle TopHotSpace => TopBridge != null
        ? new Rectangle(
            Bounds.X + TopBridge.TopOffset,
            Bounds.Y,
            Bounds.Width - TopBridge.TopOffset - TopBridge.BottomOffset,
            Space
        )
        : Rectangle.Empty;

    [JsonIgnore]
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
public class Bridge {
    public int TopOffset { get; set; }
    public int BottomOffset { get; set; }
    public int TargetScreenId { get; set; }
}
