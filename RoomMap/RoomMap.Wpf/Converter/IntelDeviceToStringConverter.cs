namespace RoomMap.Wpf.Converter {
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using Intel.RealSense;

    /// <summary></summary>
  public sealed class IntelDeviceToStringConverter : IValueConverter {
    /// <summary></summary>
    public object? Convert(
        object?     value,
        Type        targetType,
        object?     param,
        CultureInfo culture) {
      if(value == null || ! (value is Device)) {
        return null;
      }

      var device = (Device)value;
      return device.Info.GetInfo(CameraInfo.Name);
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
