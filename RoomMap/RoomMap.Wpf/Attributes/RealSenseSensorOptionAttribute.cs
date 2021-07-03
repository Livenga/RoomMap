namespace RoomMap.Wpf.Attributes {
  using System;
  using Intel.RealSense;

  /// <summary></summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public sealed class RealSenseSensorOptionAttribute : Attribute {
    /// <summary></summary>
    public Option Option => option;

    /// <summary></summary>
    public SensorType SensorType { set; get; } = SensorType.Any;


    private readonly Option option;


    /// <summary></summary>
    public RealSenseSensorOptionAttribute(Option option) {
      this.option = option;
    }
  }
}
