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
  /// <summary>CheckBoxGroup.xaml の相互作用ロジック</summary>
  public partial class CheckBoxGroup : UserControl {
    /// <summary></summary>
    public static readonly DependencyProperty InnerContentProperty =
      DependencyProperty.Register(
          nameof(InnerContent),
          typeof(object),
          typeof(CheckBoxGroup),
          new PropertyMetadata(null));

    /// <summary></summary>
    public static readonly DependencyProperty HeaderTextProperty =
      DependencyProperty.Register(
          nameof(HeaderText),
          typeof(string),
          typeof(CheckBoxGroup),
          new PropertyMetadata(string.Empty));

    /// <summary></summary>
    public static readonly DependencyProperty IsCheckedProperty =
      DependencyProperty.Register(
          nameof(IsChecked),
          typeof(bool),
          typeof(CheckBoxGroup),
          new PropertyMetadata(false));


    /// <summary></summary>
    public string HeaderText {
      set => SetValue(HeaderTextProperty, value);
      get => (string)GetValue(HeaderTextProperty);
    }

    /// <summary></summary>
    public object? InnerContent {
      set => SetValue(InnerContentProperty, value);
      get => (object?)GetValue(InnerContentProperty);
    }

    /// <summary></summary>
    public bool IsChecked {
      set => SetValue(IsCheckedProperty, value);
      get => (bool)GetValue(IsCheckedProperty);
    }


    /// <summary></summary>
    public CheckBoxGroup() {
      InitializeComponent();
    }
  }
}
