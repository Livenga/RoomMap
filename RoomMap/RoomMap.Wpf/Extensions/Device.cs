namespace RoomMap.Wpf.Extensions {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.Json;
  using System.Text.Json.Serialization;
  using System.Threading.Tasks;
  using System.Reflection;
  using Intel.RealSense;


  /// <summary></summary>
  public static class ExtendedDevice {
    /// <summary></summary>
    public static async Task SaveInfoAsync(this Device device, System.IO.Stream stream) {
      var requestCameraInfo = new CameraInfo[] {
        CameraInfo.Name,
        CameraInfo.SerialNumber,
        CameraInfo.FirmwareVersion,
        CameraInfo.RecommendedFirmwareVersion,
        CameraInfo.AdvancedMode,
        CameraInfo.ProductId
      };

      var dict = device.Info.Where(info => requestCameraInfo.Any(_ => _ == info.Key)).ToDictionary(
          info => info.Key.ToString(),
          info => (object)info.Value);

      dict.Add("Sensors", device.QuerySensors()
        .Select(sensor => new {
            Name       = sensor.Info.GetInfo(CameraInfo.Name),
            DepthScale = sensor.Is(Extension.DepthSensor) ? (float?)sensor.DepthScale : null,
            Options    = Enum.GetValues(typeof(Option))
              .Cast<Option>()
              .Where(opt => sensor.Options.Supports(opt))
              .ToDictionary(
                  opt => opt.ToString(),
                  opt => new {
                    Key         = sensor.Options[opt].Key,
                    Value       = sensor.Options[opt].Value,
                    Description = sensor.Options[opt].Description,
                    Step        = sensor.Options[opt].Step })
        })
        .ToArray());

      var opts = new JsonSerializerOptions();
      opts.WriteIndented        = true;
      opts.IgnoreNullValues     = true;
      opts.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

      await JsonSerializer.SerializeAsync(
          utf8Json: stream,
          value:    dict,
          options:  opts);
    }

    /// <summary></summary>
    public static async Task SaveInfoAsync(
        this Device device,
        string path) {
      var mode = System.IO.File.Exists(path) ? System.IO.FileMode.Truncate : System.IO.FileMode.Create;
      using(var stream = System.IO.File.Open(path, mode, System.IO.FileAccess.Write)) {
        await device.SaveInfoAsync(stream);
        await stream.FlushAsync();
      }
    }

    /// <summary></summary>
    public static async Task<string> ToInfoJsonStringAsync(this Device device) {
      using(var m = new System.IO.MemoryStream()) {
        await device.SaveInfoAsync(m);
        await m.FlushAsync();

        return Encoding.UTF8.GetString(m.ToArray());
      }
    }
  }
}
