namespace RoomMap.Wpf.Converter {
  using System;
  //using System.Linq;
  using System.Globalization;
  using System.Windows.Data;
  using Intel.RealSense;

    /// <summary></summary>
  public sealed class StreamProfileToStringConverter : IValueConverter {
    /// <summary></summary>
    public object? Convert(
        object?     value,
        Type        targetType,
        object?     param,
        CultureInfo culture) {
      if(value == null || ! (value is StreamProfile)) {
        return null;
      }

      var prof = (StreamProfile)value;
      //var str = string.Join(",", Enum.GetValues(typeof(Extension)).Cast<Extension>().Where(e => prof.Is(e)).Select(e => e.ToString()).ToArray());
      //System.Diagnostics.Debug.WriteLine(str);
      if(prof.Is(Extension.VideoProfile)) {
        var vprof = prof.As<VideoStreamProfile>();

        return $"[{vprof.Index}] {vprof.Width,4}x{vprof.Height,4} {vprof.Framerate,2} {vprof.Stream} {vprof.Format}";
      } else if(prof.Is(Extension.MotionProfile)) {
        var mprof = prof.As<MotionStreamProfile>();

        return $"[{mprof.Index}] {mprof.Framerate,3} {mprof.Stream} {mprof.Format}";
      }

      throw new NotSupportedException();
    }

    /// <summary></summary>
    public object? ConvertBack(
        object?     value,
        Type        targetType,
        object?     param,
        CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}
