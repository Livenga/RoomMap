using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RoomMap.Wpf.Controls {
  /// <summary>SliderGroup.xaml の相互作用ロジック</summary>
  public partial class SliderGroup : UserControl {
    /// <summary></summary>
    public static readonly DependencyProperty HeaderTextProperty =
      DependencyProperty.Register(
          nameof(HeaderText),
          typeof(string),
          typeof(SliderGroup),
          new PropertyMetadata(string.Empty));

    /// <summary></summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register(
          nameof(Minimum),
          typeof(double),
          typeof(SliderGroup),
          new PropertyMetadata(0d));

    /// <summary></summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register(
          nameof(Maximum),
          typeof(double),
          typeof(SliderGroup),
          new PropertyMetadata(10d));

    /// <summary></summary>
    public static readonly DependencyProperty SmallChangeProperty =
      DependencyProperty.Register(
          nameof(SmallChange),
          typeof(double),
          typeof(SliderGroup),
          new PropertyMetadata(1d));

    /// <summary></summary>
    public static readonly DependencyProperty TickFrequencyProperty =
      DependencyProperty.Register(
          nameof(TickFrequency),
          typeof(double),
          typeof(SliderGroup),
          new PropertyMetadata(1d));

    /// <summary></summary>
    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register(
          nameof(Value),
          typeof(double),
          typeof(SliderGroup),
          new PropertyMetadata(0d));

    /// <summary></summary>
    public static readonly DependencyProperty DefaultValueProperty =
      DependencyProperty.Register(
          nameof(DefaultValue),
          typeof(double),
          typeof(SliderGroup),
          new PropertyMetadata(0d));

    /// <summary></summary>
    public string HeaderText {
      set => SetValue(HeaderTextProperty, value);
      get => (string)GetValue(HeaderTextProperty);
    }

    /// <summary></summary>
    public double Minimum {
      set => SetValue(MinimumProperty, value);
      get => (double)GetValue(MinimumProperty);
    }

    /// <summary></summary>
    public double Maximum {
      set => SetValue(MaximumProperty, value);
      get => (double)GetValue(MaximumProperty);
    }

    /// <summary></summary>
    public double SmallChange {
      set => SetValue(SmallChangeProperty, value);
      get => (double)GetValue(SmallChangeProperty);
    }

    /// <summary></summary>
    public double TickFrequency {
      set => SetValue(TickFrequencyProperty, value);
      get => (double)GetValue(TickFrequencyProperty);
    }

    /// <summary></summary>
    public double Value {
      set => SetValue(ValueProperty, value);
      get => (double)GetValue(ValueProperty);
    }

    public double DefaultValue {
      set => SetValue(DefaultValueProperty, value);
      get => (double)GetValue(DefaultValueProperty);
    }

    /// <summary></summary>
    public SliderGroup() {
      InitializeComponent();
    }

    /// <summary></summary>
    private void OnSliderMouseDoubleClick(object source, MouseEventArgs e) {
      if(e.LeftButton == MouseButtonState.Pressed) {
        Value = DefaultValue;
      }
    }
  }
}
