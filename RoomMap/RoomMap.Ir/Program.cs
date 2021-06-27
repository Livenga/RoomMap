namespace RoomMap.Ir {
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Intel.RealSense;
  using RoomMap.Data;
  using RoomMap.Data.Extensions;


  public class Program {
    private static CancellationTokenSource? tokenSource = null;
    private static string outputDiretory = string.Empty;


    public static async Task Main(string[] args) {
      using(var ctx = new Context()) {
        var devices = ctx.QueryDevices();
        if(devices.Count == 0) {
          Console.Error.WriteLine($"device not found.");
        }

        if(args.Length == 0) {
          outputDiretory = System.IO.Path.Join(
              $"{Guid.NewGuid().ToString()}");
        } else {
          outputDiretory = System.IO.Path.Join(
              args[0],
              $"{Guid.NewGuid().ToString()}");
        }
        Console.Error.WriteLine($"d Output directory: {outputDiretory}");

        try {
          if(! System.IO.Directory.Exists(outputDiretory)) {
            System.IO.Directory.CreateDirectory(outputDiretory);
          }
        } catch(Exception except) {
          Console.Error.WriteLine($"d {except.GetType().Name} {except.Message}");
          return;
        }


        var device = devices[0];

        var depthSensor = device.QuerySensors()
          .First(sensor => sensor.Is(Extension.DepthStereoSensor));
        var colorSensor = device.QuerySensors()
          .First(sensor => sensor.Is(Extension.ColorSensor));
        var motionSensor = device.QuerySensors()
          .First(sensor => sensor.Is(Extension.MotionSensor));

        /*
        foreach(var vprof in colorSensor
            .StreamProfiles
            .Where(prof => prof.Is(Extension.VideoProfile))
            .Select(prof => prof.As<VideoStreamProfile>())) {
          Console.Error.WriteLine($"{vprof.Stream} {vprof.UniqueID} {vprof.Index} {vprof.Width}x{vprof.Height} {vprof.Framerate} {vprof.Format}");
        }
        */

        var infraredProfiles = depthSensor.StreamProfiles
          .Where(prof => prof.Stream == Stream.Infrared)
          .Where(prof => prof.Is(Extension.VideoProfile))
          .Select(prof => prof.As<VideoStreamProfile>())
          .Where(prof => (prof.Index == 1 || prof.Index == 2) && prof.Width == 1280 && prof.Height == 800 && prof.Format == Format.Y16 && prof.Framerate == 15)
          .ToArray();

        var colorProfile = colorSensor.StreamProfiles
          .Where(prof => prof.Stream == Stream.Color)
          .Where(prof => prof.Is(Extension.VideoProfile))
          .Select(prof => prof.As<VideoStreamProfile>())
          .First(prof => prof.Width == 1280 && prof.Height == 800 && prof.Framerate == 5 && prof.Format == Format.Rgb8);

        var motionProfile = motionSensor.StreamProfiles
          .Where(prof => prof.Stream == Stream.Accel)
          .Where(prof => prof.Is(Extension.MotionProfile))
          .Select(prof => prof.As<MotionStreamProfile>())
          .First(prof => prof.Framerate == 63 && prof.Format == Format.MotionXyz32f);


        var deviceInfo = DeviceInfo.Get(device);
        // XXX: D455 を使用する場合には問題ない. (Infrared Profile 添字)
        deviceInfo.Profiles = new ProfileInfo[] {
          ProfileInfo.Get(infraredProfiles[0]),
          ProfileInfo.Get(infraredProfiles[1]),
          ProfileInfo.Get(colorProfile),
          ProfileInfo.Get(motionProfile),
        };
        Console.Error.WriteLine($"{await deviceInfo.ToJsonStringAsync()}");
        await deviceInfo.SaveAsync(System.IO.Path.Join(outputDiretory, "device_info.json"));

        foreach(var prof in infraredProfiles) {
          Console.Error.WriteLine($"d {prof.Stream} {prof.Index} {prof.Width}x{prof.Height} {prof.Framerate} {prof.Format}");
        }
        Console.Error.WriteLine($"d {colorProfile.Stream} {colorProfile.Index} {colorProfile.Width}x{colorProfile.Height} {colorProfile.Framerate} {colorProfile.Format}");
        Console.Error.WriteLine($"d {motionProfile.Stream} {motionProfile.Format} {motionProfile.Index} {motionProfile.Framerate}");


        using(var pipeline = new Pipeline(ctx)) {
          var cfg = new Config();
          cfg.DisableAllStreams();

          foreach(var p in infraredProfiles) {
            cfg.EnableStream(
                stream_type: p.Stream,
                stream_index: p.Index,
                width: p.Width,
                height: p.Height,
                format: p.Format,
                framerate: p.Framerate);
          }

          cfg.EnableStream(
              stream_type: colorProfile.Stream,
              stream_index: colorProfile.Index,
              width: colorProfile.Width,
              height: colorProfile.Height,
              format: colorProfile.Format,
              framerate: colorProfile.Framerate);

          cfg.EnableStream(
              stream_type: motionProfile.Stream,
              stream_index: motionProfile.Index,
              format: motionProfile.Format,
              framerate: motionProfile.Framerate);

          using(tokenSource = new CancellationTokenSource()) {
            pipeline.Start(cfg);

            var task = Task.Factory.StartNew(
                function:          OnPipelineFrameReceivedAsync,
                state:             pipeline,
                cancellationToken: tokenSource.Token);

            Console.ReadLine();
            tokenSource.Cancel(true);
            task.Wait(3000);

            pipeline.Stop();
          }
        }
      }
    }

    private static long serialNumber = 0;
    private static async Task OnPipelineFrameReceivedAsync(object? state) {
      if(state == null || ! (state is Pipeline)) {
        return;
      }

      var pipeline = (Pipeline)state;
      while(! (tokenSource?.Token.IsCancellationRequested ?? true)) {
        using(var frames = pipeline.WaitForFrames(3000)) {
          var _frames = frames.Select(f => f.DisposeWith(frames)).ToArray();

          Console.Error.WriteLine($"d frame: {serialNumber}");
          foreach(var frame in _frames.Where(f => f.Is(Extension.VideoFrame))) {
            using(var _frame = frame.As<VideoFrame>().DisposeWith(frames)) {
              //Console.Error.WriteLine($"d {_frame.ToFileName(serialNumber)} {_frame.Width}x{_frame.Height} {_frame.DataSize}");
              await _frame.SaveAsync(System.IO.Path.Join(outputDiretory, _frame.ToFileName(serialNumber)));
            }
          }

          using(var _frame = _frames.First(f => f.Is(Extension.MotionFrame)).As<MotionFrame>().DisposeWith(frames)) {
            var data = _frame.MotionData;
            //Console.Error.WriteLine($"d {_frame.ToFileName(serialNumber)} {_frame.DataSize} ({data.x}, {data.y}, {data.z})");
            await _frame.SaveAsync(System.IO.Path.Join(outputDiretory, _frame.ToFileName(serialNumber)));
          }
        }

        ++serialNumber;
      }
    }

    /// <summary></summary>
    private static Extension[] GetExtensions(object obj) {
      var method = obj.GetType().GetMethod("Is");
      if(method == null ||
          method.GetParameters()[0].ParameterType != typeof(Extension)) {
        throw new NotSupportedException();
      }

      return Enum.GetValues(typeof(Extension))
        .Cast<Extension>()
        .Where(ext => (bool?)method.Invoke(obj, new object[] { ext }) ?? false )
        .ToArray();
    }
  }
}
