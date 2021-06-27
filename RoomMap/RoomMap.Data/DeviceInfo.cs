namespace RoomMap.Data {
  using System;
  using System.Linq;
  using System.Text;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Intel.RealSense;


  /// <summary></summary>
  public class DeviceInfo {
    /// <summary></summary>
    public string Name { set; get; } = string.Empty;

    /// <summary></summary>
    public string ProductId { set; get; } = string.Empty;

    /// <summary></summary>
    public string FirmwareVersion { set; get; } = string.Empty;

    /// <summary></summary>
    public string SerialNumber { set; get; } = string.Empty;

    /// <summary></summary>
    public float? DepthScale { set; get; } = null;

    /// <summary></summary>
    public string RecommendedFirmwareVersion { set; get; } = string.Empty;

    /// <summary></summary>
    public ProfileInfo[] Profiles { set; get; } = Array.Empty<ProfileInfo>();


    /// <summary></summary>
    public static DeviceInfo Get(Device device) {
      var info = new DeviceInfo();

      foreach(var name in Enum.GetNames(typeof(CameraInfo))) {
        var e = (CameraInfo)Enum.Parse(typeof(CameraInfo), name);
        typeof(DeviceInfo).GetProperty(name)?.SetValue(info, device.Info.GetInfo(e));
      }

      info.DepthScale = device.Sensors
        .FirstOrDefault(s => s.Is(Extension.DepthSensor))?
        .DepthScale;

      return info;
    }


    /// <summary></summary>
    public async Task SaveAsync(System.IO.Stream stream) {
      var opts = new JsonSerializerOptions() {
        IgnoreNullValues = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
      };

      await JsonSerializer.SerializeAsync(stream, this, opts);
      await stream.FlushAsync();
    }

    /// <summary></summary>
    public async Task SaveAsync(string path) {
      var mode = System.IO.File.Exists(path)
        ? System.IO.FileMode.Truncate
        : System.IO.FileMode.Create;

      using(var stream = System.IO.File.Open(path, mode, System.IO.FileAccess.Write)) {
        await SaveAsync(stream);
      }
    }


    /// <summary></summary>
    public async Task<string> ToJsonStringAsync() {
      using(var m = new System.IO.MemoryStream()) {
        await SaveAsync(m);

        m.Position = 0;

        return Encoding.UTF8.GetString(m.ToArray());
      }
    }
  }
}
