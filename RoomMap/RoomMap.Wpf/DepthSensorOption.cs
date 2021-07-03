namespace RoomMap.Wpf {
  /// <summary></summary>
  public class DepthSensorOption : ISensorOption {
    /// <summary></summary>
    public bool IsAutoExposureEnabled { set; get; } = true;

    /// <summary></summary>
    public bool IsEmitterEnabled { set; get; } = true;

    /// <summary></summary>
    public bool IsEmitterOn { set; get; } = false;

    /// <summary></summary>
    public bool IsEmitterAlwaysOn { set; get; } = false;

    /// <summary></summary>
    public bool IsThermalCompensationEnabled { set; get; } = true;

    /// <summary></summary>
    public bool IsHdrEnabled { set; get; } = false;

    /// <summary></summary>
    public float Exposure { set; get; } = 33000f;

    /// <summary></summary>
    public float Gain { set; get; } = 16f;

    /// <summary></summary>
    public float LaserPower { set; get; } = 150f;
  }
}
