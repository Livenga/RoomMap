using System;
using System.Collections.Generic;
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
  using RoomMap.Wpf.Data;

  /// <summary>MotionControl.xaml の相互作用ロジック</summary>
  public partial class MotionControl : UserControl {
    /// <summary></summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register(
          nameof(Title),
          typeof(string),
          typeof(MotionControl),
          new PropertyMetadata("Hello"));

    /// <summary></summary>
    public static readonly DependencyProperty MotionDataProperty =
      DependencyProperty.Register(
          nameof(MotionData),
          typeof(MotionData),
          typeof(MotionControl),
          new PropertyMetadata(null));


    /// <summary></summary>
    public string Title {
      set => SetValue(TitleProperty, value);
      get => (string)GetValue(TitleProperty);
    }

    /// <summary></summary>
    public MotionData? MotionData {
      set => SetValue(MotionDataProperty, value);
      get => (MotionData?)GetValue(MotionDataProperty);
    }


    /// <summary></summary>
    public MotionControl() {
      InitializeComponent();
    }
  }
}
