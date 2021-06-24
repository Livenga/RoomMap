namespace RoomMap.Cmd.Extensions {
  using System;
  using System.Text.Json;
  using System.Threading.Tasks;
  using RoomMap.Cmd.Data;


  public static class ExtendedFrame {
    /// <summary></summary>
    public static async Task SaveAsync(
        this Intel.RealSense.Frame frame,
        System.IO.Stream stream) {
      //System.Console.Error.WriteLine($"{frame.GetType().Name}");
      if(frame is Intel.RealSense.VideoFrame vf) {
        var buffer = new byte[vf.DataSize];
        vf.CopyTo<byte>(buffer);

        await stream.WriteAsync(buffer, 0, buffer.Length);
      } else if(frame is Intel.RealSense.MotionFrame mf) {
        var motionData = new MotionData() {
          X = mf.MotionData.x,
          Y = mf.MotionData.y,
          Z = mf.MotionData.z
        };

        await JsonSerializer.SerializeAsync(stream, motionData);
      } else {
        throw new NotSupportedException();
      }
    }

    /// <summary></summary>
    public static async Task SaveAsync(
        this Intel.RealSense.Frame frame,
        string path) {
      try {
        using(var stream = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
          await frame.SaveAsync(stream);
        }
      } catch {
        if(System.IO.File.Exists(path)) {
          System.IO.File.Delete(path);
        }
        throw;
      }
    }
  }
}
