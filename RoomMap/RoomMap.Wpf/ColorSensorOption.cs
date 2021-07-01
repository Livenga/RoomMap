namespace RoomMap.Wpf {
  /// <summary></summary>
  public class ColorSensorOption : ISensorOption {
    /// <summary></summary>
    public float Brightness { set; get; } = 0f;

    /// <summary></summary>
    public float Contrast { set; get; } = 50f;

    /// <summary></summary>
    public float Exposure { set; get; } = 156f;

    /// <summary></summary>
    public float Gain { set; get; } = 64f;

    /// <summary></summary>
    public float Gamma { set; get; } = 300f;
  }
}
