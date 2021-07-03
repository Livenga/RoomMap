namespace RoomMap.Wpf {
  /// <summary></summary>
  public class ColorSensorOption : ISensorOption {
    /// <summary></summary>
    public bool IsAutoExposureEnabled { set; get; } = true;

    /// <summary></summary>
    public bool IsAutoWhiteBalanceEnabled { set; get; } = true;

    /// <summary></summary>
    public bool IsAutoExposurePriorityEnabled { set; get; } = true;

    /// <summary></summary>
    public bool IsBacklightCompensationEnabled { set; get; } = false;

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

    /// <summary></summary>
    public float Hue { set; get; } = 0;

    /// <summary></summary>
    public float Saturation { set; get; } = 64f;

    /// <summary></summary>
    public float Sharpness { set; get; } = 50f;

    /// <summary></summary>
    public float WhiteBalance { set; get; } = 4600f;
  }
}
