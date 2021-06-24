namespace RoomMap.Cmd.Data {
  using System.Text.Json.Serialization;
  using Intel.RealSense;

  /// <summary></summary>
  public class ProfileInfo {
    /// <summary></summary>
    public int Index { set; get; } = 0;

    /// <summary></summary>
    [JsonConverter(typeof(Converters.StreamJsonConverter))]
    public Stream Stream { set; get; } = Stream.Any;

    /// <summary></summary>
    public int? Width { set; get; } = null;

    /// <summary></summary>
    public int? Height { set; get; } = null;

    /// <summary></summary>
    public int Framerate { set; get; } = 0;

    /// <summary></summary>
    [JsonConverter(typeof(Converters.FormatJsonConverter))]
    public Format Format { set; get; } = Format.Any;


    /// <summary></summary>
    public static ProfileInfo Get(StreamProfile prof) {
      var info = new ProfileInfo();

      foreach(var prop in prof.GetType().GetProperties()) {
        typeof(ProfileInfo)
          .GetProperty(prop.Name)?
          .SetValue(info, prop.GetValue(prof));
      }

      return info;
    }
  }
}
