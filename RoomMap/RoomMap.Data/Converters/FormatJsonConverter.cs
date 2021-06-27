namespace RoomMap.Data.Converters {
  using System;
  using System.Text.Json.Serialization;
  using Intel.RealSense;


  public sealed class FormatJsonConverter : JsonConverter<Format> {
    public override Format Read(
        ref System.Text.Json.Utf8JsonReader reader,
        System.Type typeToConvert,
        System.Text.Json.JsonSerializerOptions options) {

      return (Format)Enum.Parse(typeof(Format), reader.GetString() ?? "Any");
    }

    public override void Write(
        System.Text.Json.Utf8JsonWriter writer,
        Format value,
        System.Text.Json.JsonSerializerOptions options) {
      writer.WriteStringValue(value.ToString());
    }
  }
}
