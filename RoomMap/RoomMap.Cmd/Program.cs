using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace RoomMap.Cmd {
  using RoomMap.Cmd.Data;
  using RoomMap.Cmd.Extensions;
  using Intel.RealSense;

  /// <summary></summary>
  public class Program {
    private static Guid targetId = Guid.Empty;
    private static CancellationTokenSource? tokenSource = null;
    private static string outputDirectoryPath = string.Empty;


    /// <summary></summary>
    public static async Task Main(string[] args) {
      using(var ctx = new Context()) {
        var devices = ctx.QueryDevices();
        if(devices.Count == 0) {
          Console.Error.WriteLine($"");
          return;
        }

        targetId = Guid.NewGuid();
        var device = devices.First();

        var depthSensor  = device.GetSensor(Extension.DepthSensor);
        var colorSensor  = device.GetSensor(Extension.ColorSensor);
        var motionSensor = device.GetSensor(Extension.MotionSensor);

        var depthProfile = depthSensor.StreamProfiles
          .Select(prof => prof.As<VideoStreamProfile>())
          .Where(prof => prof.Width == 1280 && prof.Height == 720 && prof.Stream == Stream.Depth)
          .OrderBy(prof => prof.Framerate)
          .First();

        var colorProfile = colorSensor.StreamProfiles
          .Select(prof => prof.As<VideoStreamProfile>())
          .Where(prof => prof.Width == 1280 && prof.Height == 720 && prof.Stream == Stream.Color && prof.Format == Format.Rgb8)
          .OrderBy(prof => prof.Framerate)
          .First();

        var accelProfile = motionSensor.StreamProfiles
          .Select(prof => prof.As<MotionStreamProfile>())
          .Where(prof => prof.Stream == Stream.Accel)
          .OrderBy(prof => prof.Framerate)
          .First();

        outputDirectoryPath = System.IO.Path.Join(
            ((args.Length > 0) ? args[0] : System.IO.Directory.GetCurrentDirectory()),
            targetId.ToString());
        if(! System.IO.Directory.Exists(outputDirectoryPath)) {
          System.IO.Directory.CreateDirectory(outputDirectoryPath);
        }
        Console.Error.WriteLine($"Data output directory: {outputDirectoryPath}");

        Console.Error.WriteLine($"d DepthProfile: {depthProfile.Width}x{depthProfile.Height} {depthProfile.Framerate} {depthProfile.Format}");
        Console.Error.WriteLine($"d ColorProfile: {colorProfile.Width}x{colorProfile.Height} {colorProfile.Framerate} {colorProfile.Format}");
        Console.Error.WriteLine($"d MotionProfile: {accelProfile.Framerate} {accelProfile.Format} {accelProfile.Stream}");


        var deviceInfo = DeviceInfo.Get(device);
        deviceInfo.Profiles = new ProfileInfo[] {
          ProfileInfo.Get(depthProfile),
          ProfileInfo.Get(colorProfile),
          ProfileInfo.Get(accelProfile)
        };

        await deviceInfo.SaveAsync(System.IO.Path.Join(outputDirectoryPath, "device_info.json"));

        var cfg = new Config();
        cfg.DisableAllStreams();

        cfg.EnableStream(
            stream_type: depthProfile.Stream,
            width:       depthProfile.Width,
            height:      depthProfile.Height,
            framerate:   depthProfile.Framerate,
            format:      depthProfile.Format);
        cfg.EnableStream(
            stream_type: colorProfile.Stream,
            width:       colorProfile.Width,
            height:      colorProfile.Height,
            framerate:   colorProfile.Framerate,
            format:      colorProfile.Format);
        cfg.EnableStream(
            stream_type: accelProfile.Stream,
            format:      accelProfile.Format,
            framerate:   accelProfile.Framerate);

        using(var pipeline = new Pipeline(ctx)) {
          pipeline.Start(cfg);

          using(tokenSource = new CancellationTokenSource()) {
            var task = Task.Factory.StartNew(OnPipelinePollingAsync, pipeline);

            Console.ReadLine();
          }

          pipeline.Stop();
        }
      }
    }

    /// <summary></summary>
    private static async Task OnPipelinePollingAsync(object? state) {
      if(state == null || ! (state is Pipeline)) {
        return;
      }

      var pipeline = (Pipeline)state;
      long serialNumber = 0;
      string path = string.Empty;

      while(! (tokenSource?.Token.IsCancellationRequested ?? true)) {
        try {
          using(var frames = pipeline.WaitForFrames(5000)) {
            var _frames = frames.Select(f => f.DisposeWith(frames)).ToArray();

            using(var df = _frames.First(f => f.Profile.Stream == Stream.Depth).As<DepthFrame>().DisposeWith(frames)) {
              path = System.IO.Path.Join(
                  outputDirectoryPath,
                  $"{serialNumber}.{df.Number}.{df.Profile.Stream}-{df.Profile.Format}");

              await df.SaveAsync(path);
            }

            using(var cf = _frames.First(f => f.Profile.Stream == Stream.Color).As<VideoFrame>().DisposeWith(frames)) {
              path = System.IO.Path.Join(
                  outputDirectoryPath,
                  $"{serialNumber}.{cf.Number}.{cf.Profile.Stream}-{cf.Profile.Format}");

              await cf.SaveAsync(path);
            }


            using(var mf = _frames.First(f => f.Profile.Stream == Stream.Accel).As<MotionFrame>().DisposeWith(frames)) {
              path = System.IO.Path.Join(
                  outputDirectoryPath,
                  $"{serialNumber}.{mf.Number}.{mf.Profile.Stream}-{mf.Profile.Format}");

              await mf.SaveAsync(path);
            }
          }
        } catch(Exception except) {
          Console.Error.WriteLine($"d {except.GetType().Name} {except.Message}");
          Console.Error.WriteLine($"d {except.StackTrace}");
          break;
        }

        ++serialNumber;
      }
    }
  }
}
