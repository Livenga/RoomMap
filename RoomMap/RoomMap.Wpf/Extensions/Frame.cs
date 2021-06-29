namespace RoomMap.Wpf.Extensions {
  using System;
  using System.Threading.Tasks;
  using System.Reflection;
  using Intel.RealSense;

  /// <summary></summary>
  public static class ExtendedVideoFrame {
    /// <summary></summary>
    public static async Task SaveAsync(
        this VideoFrame frame,
        string root,
        long   serialNumber = 0) {
      var fileName = $"{serialNumber}.{frame.Number}.{frame.Profile.Index}.{frame.Profile.Stream}-{frame.Profile.Format}";

      var buffer = new byte[frame.DataSize];
      frame.CopyTo<byte>(buffer);

      var path = System.IO.Path.Join(root, fileName);
      using(var stream = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
        await stream.WriteAsync(buffer, 0, frame.DataSize);
        await stream.FlushAsync();
      }
    }
  }
}
