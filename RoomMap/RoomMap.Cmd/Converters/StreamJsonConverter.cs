namespace RoomMap.Cmd.Converters {
  using System;
  using System.Text.Json.Serialization;
  using Intel.RealSense;


  /// <summary></summary>
  public sealed class StreamJsonConverter : JsonConverter<Stream> {
    /// <summary></summary>
    public override Stream Read(
        ref System.Text.Json.Utf8JsonReader reader,
        System.Type typeToConvert,
        System.Text.Json.JsonSerializerOptions options) {
      return (Stream)Enum.Parse(typeof(Stream), reader.GetString() ?? "Any");
    }

    /// <summary></summary>
    public override void Write(
        System.Text.Json.Utf8JsonWriter writer,
        Stream value,
        System.Text.Json.JsonSerializerOptions options) {
      writer.WriteStringValue(value.ToString());
    }
  }
}
