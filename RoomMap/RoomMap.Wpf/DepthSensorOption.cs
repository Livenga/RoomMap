namespace RoomMap.Wpf {
  using RoomMap.Wpf.Attributes;
  using Intel.RealSense;


  /// <summary></summary>
  public class DepthSensorOption : ISensorOption {
    /// <summary></summary>
    [RealSenseSensorOption(Option.EnableAutoExposure, SensorType = SensorType.Depth)]
    public bool IsAutoExposureEnabled { set; get; } = true;

    /// <summary></summary>
    [RealSenseSensorOption(Option.EmitterEnabled, SensorType = SensorType.Depth)]
    public bool IsEmitterEnabled { set; get; } = true;

    /// <summary></summary>
    [RealSenseSensorOption(Option.EmitterOnOff, SensorType = SensorType.Depth)]
    public bool IsEmitterOn { set; get; } = false;

    /// <summary></summary>
    [RealSenseSensorOption(Option.EmitterAlwaysOn, SensorType = SensorType.Depth)]
    public bool IsEmitterAlwaysOn { set; get; } = false;

    /// <summary></summary>
    [RealSenseSensorOption(Option.ThermalCompensation, SensorType = SensorType.Depth)]
    public bool IsThermalCompensationEnabled { set; get; } = true;

    /// <summary></summary>
    [RealSenseSensorOption(Option.HdrEnabled, SensorType = SensorType.Depth)]
    public bool IsHdrEnabled { set; get; } = false;

    /// <summary></summary>
    [RealSenseSensorOption(Option.Exposure, SensorType = SensorType.Depth)]
    public float Exposure { set; get; } = 33000f;

    /// <summary></summary>
    [RealSenseSensorOption(Option.Gain, SensorType = SensorType.Depth)]
    public float Gain { set; get; } = 16f;

    /// <summary></summary>
    [RealSenseSensorOption(Option.LaserPower, SensorType = SensorType.Depth)]
    public float LaserPower { set; get; } = 150f;
  }
}
