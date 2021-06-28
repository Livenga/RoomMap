namespace RoomMap.Wpf.Data {
  /// <summary></summary>
  public class MotionData {
    /// <summary></summary>
    public double X => x;

    /// <summary></summary>
    public double Y => y;

    /// <summary></summary>
    public double Z => z;


    private readonly double x;
    private readonly double y;
    private readonly double z;


    /// <summary></summary>
    public MotionData(
        double x,
        double y,
        double z) {
      this.x = x;
      this.y = y;
      this.z = z;
    }
  }
}
