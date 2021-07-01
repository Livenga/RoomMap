namespace RoomMap.Wpf {
  /// <summary></summary>
  public class DepthSensorOption : ISensorOption {
    /// <summary></summary>
    public float Exposure { set; get; } = 33000f;

    /// <summary></summary>
    public float Gain { set; get; } = 16f;

    /// <summary></summary>
    public float LaserPower { set; get; } = 150f;
  }
}
