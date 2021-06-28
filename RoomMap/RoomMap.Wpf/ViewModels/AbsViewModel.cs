using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace RoomMap.Wpf.ViewModels {
  /// <summary></summary>
  public abstract class AbsViewModel : INotifyPropertyChanged {
    /// <summary></summary>
    public event PropertyChangedEventHandler? PropertyChanged = null;

    /// <summary></summary>
    protected void SetProperty<T>(
        ref T storage,
        T value,
        [CallerMemberName]string? propName = null) {
      if(object.ReferenceEquals(storage, value)) {
        return;
      }
      storage = value;

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
  }
}
