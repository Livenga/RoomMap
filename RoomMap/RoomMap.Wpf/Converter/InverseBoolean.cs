namespace RoomMap.Wpf.Converter {
  using System;
  using System.Globalization;
  using System.Windows.Data;


  /// <summary></summary>
  public sealed class InverseBooleanConverter : IValueConverter {
    /// <summary></summary>
    public object? Convert(
        object?     value,
        Type        targetType,
        object?     param,
        CultureInfo culture) {
      if(value == null || ! (value is bool)) {
        return false;
      }

      return ! (bool)value;
    }

    /// <summary></summary>
    public object? ConvertBack(
        object?     value,
        Type        targetType,
        object?     param,
        CultureInfo culture) {
      if(value == null || ! (value is bool)) {
        return false;
      }

      return ! (bool)value;
    }
  }
}
