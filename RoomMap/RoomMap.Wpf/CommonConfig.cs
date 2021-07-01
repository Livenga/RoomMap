namespace RoomMap.Wpf {
  using System;
  using System.IO;
  using System.Text.Json;
  using System.Threading.Tasks;

  /// <summary></summary>
  public class CommonConfig {
    /// <summary></summary>
    public string OutputDirectory { set; get; } = string.Empty;

    /// <summary></summary>
    public bool IsSaveEnabled { set; get; } = false;

    /// <summary></summary>
    public DepthSensorOption DepthSensorOption { private set; get; } = new DepthSensorOption();

    /// <summary></summary>
    public ColorSensorOption ColorSensorOption { private set; get; } = new ColorSensorOption();


    /// <summary></summary>
    public async Task SaveAsync(Stream stream) {
      var opt = new JsonSerializerOptions() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      await JsonSerializer.SerializeAsync<CommonConfig>(
          utf8Json: stream,
          value:    this,
          options:  opt);

      await stream.FlushAsync();
    }

    /// <summary></summary>
    public async Task SaveAsync(string path) {
      var mode = File.Exists(path) ? FileMode.Truncate : FileMode.CreateNew;
      using(var stream = File.Open(path, mode, FileAccess.Write)) {
        await SaveAsync(stream);
      }
    }


    /// <summary></summary>
    public static async Task<CommonConfig> LoadAsync(Stream stream) {
      var opt = new JsonSerializerOptions() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      var cfg = await JsonSerializer.DeserializeAsync<CommonConfig>(
          utf8Json: stream,
          options:  opt);
      if(cfg == null) {
        throw new NullReferenceException();
      }

      return cfg;
    }

    /// <summary></summary>
    public static async Task<CommonConfig> LoadAsync(string path) {
      using(var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
        return await LoadAsync(stream);
      }
    }
  }
}
