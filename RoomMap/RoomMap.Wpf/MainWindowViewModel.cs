namespace RoomMap.Wpf {
  using System.Collections.ObjectModel;
  using Intel.RealSense;
  using RoomMap.Wpf.Data;

  /// <summary>MainWindow ViewModel</summary>
  public sealed class MainWindowViewModel : ViewModels.AbsViewModel {
    /// <summary>検出デバイス一覧</summary>
    public ObservableCollection<Device> Devices { private set; get; } = new ();

    /// <summary>選択されたデバイス</summary>
    public Device? SelectedDevice {
      set => SetProperty(ref selectedDevice, value, nameof(SelectedDevice));
      get => selectedDevice;
    }

    /// <summary></summary>
    public ObservableCollection<StreamProfile> DepthStreamProfiles { private set; get; } = new ();
    /// <summary></summary>
    public StreamProfile? SelectedDepthStreamProfile {
      set => SetProperty(ref selectedDepthStreamProfile, value, nameof(SelectedDepthStreamProfile));
      get => selectedDepthStreamProfile;
    }

    /// <summary></summary>
    public ObservableCollection<StreamProfile> ColorStreamProfiles { private set; get; } = new ();
    /// <summary></summary>
    public StreamProfile? SelectedColorStreamProfile {
      set => SetProperty(ref selectedColorStreamProfile, value, nameof(SelectedColorStreamProfile));
      get => selectedColorStreamProfile;
    }

    /// <summary></summary>
    public ObservableCollection<StreamProfile> MotionStreamProfiles { private set; get; } = new ();
    /// <summary></summary>
    public StreamProfile? SelectedMotionStreamProfile {
      set => SetProperty(ref selectedMotionStreamProfile, value, nameof(SelectedMotionStreamProfile));
      get => selectedMotionStreamProfile;
    }

    /// <summary></summary>
    public bool IsSaveEnabled {
      set => SetProperty(ref isSaveEnabled, value, nameof(IsSaveEnabled));
      get => isSaveEnabled;
    }

    /// <summary></summary>
    public bool IsRecording {
      set => SetProperty(ref isRecording, value, nameof(IsRecording));
      get => isRecording;
    }

    /// <summary></summary>
    public string OutputDirectory {
      set => SetProperty(ref outputDirectory, value, nameof(OutputDirectory));
      get => outputDirectory;
    }

    /// <summary></summary>
    public string DepthFrameText {
      set => SetProperty(ref depthFrameText, value, nameof(DepthFrameText));
      get => depthFrameText;
    }

    /// <summary></summary>
    public string ColorFrameText {
      set => SetProperty(ref colorFrameText, value, nameof(ColorFrameText));
      get => colorFrameText;
    }

    /// <summary></summary>
    public MotionData? RealSenseAccel {
      set => SetProperty(ref realSenseAccel, value, nameof(RealSenseAccel));
      get => realSenseAccel;
    }


    private Device? selectedDevice = null;
    private StreamProfile? selectedDepthStreamProfile = null;
    private StreamProfile? selectedColorStreamProfile = null;
    private StreamProfile? selectedMotionStreamProfile = null;
    private bool isSaveEnabled = false;
    private bool isRecording = false;
    private string outputDirectory = string.Empty;

    private string depthFrameText = string.Empty;
    private string colorFrameText = string.Empty;
    private MotionData? realSenseAccel = null;
  }
}
