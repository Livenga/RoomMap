namespace RoomMap.Data.Extensions {
  using System.Linq;
  using Intel.RealSense;

  /// <summary></summary>
  public static class ExtendedDevice {
    /// <summary></summary>
    public static Sensor GetSensor(
        this Device device,
        Extension extension) =>  device.QuerySensors().First(s => s.Is(extension));
  }
}
