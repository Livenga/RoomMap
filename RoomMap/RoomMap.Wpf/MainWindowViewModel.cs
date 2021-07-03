namespace RoomMap.Wpf {
  using System.Collections.ObjectModel;
  using Intel.RealSense;
  using RoomMap.Wpf.Attributes;
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
    [RealSenseSensorOption(Option.EnableAutoExposure, SensorType = SensorType.Depth)]
    public bool IsDepthAutoExposureEnabled {
      set => SetProperty(ref isDepthAutoExposureEnabled, value, nameof(IsDepthAutoExposureEnabled));
      get => isDepthAutoExposureEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.EmitterEnabled, SensorType = SensorType.Depth)]
    public bool IsDepthEmitterEnabled {
      set => SetProperty(ref isDepthEmitterEnabled, value, nameof(IsDepthEmitterEnabled));
      get => isDepthEmitterEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.EmitterAlwaysOn, SensorType = SensorType.Depth)]
    public bool IsDepthEmitterAlwaysOn {
      set => SetProperty(ref isDepthEmitterAlwaysOn, value, nameof(IsDepthEmitterAlwaysOn));
      get => isDepthEmitterAlwaysOn;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.EmitterOnOff, SensorType = SensorType.Depth)]
    public bool IsDepthEmitterOn {
      set => SetProperty(ref isDepthEmitterOn, value, nameof(IsDepthEmitterOn));
      get => isDepthEmitterOn;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.ThermalCompensation, SensorType = SensorType.Depth)]
    public bool IsDepthThermalCompensationEnabled {
      set => SetProperty(ref isDepthThermalCompensationEnabled, value, nameof(IsDepthThermalCompensationEnabled));
      get => isDepthThermalCompensationEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.HdrEnabled, SensorType = SensorType.Depth)]
    public bool IsDepthHdrEnabled {
      set => SetProperty(ref isDepthHdrEnabled, value, nameof(IsDepthHdrEnabled));
      get => isDepthHdrEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Exposure, SensorType = SensorType.Depth)]
    public float DepthExposure {
      set => SetProperty(ref depthExposure, value, nameof(DepthExposure));
      get => depthExposure;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Gain, SensorType = SensorType.Depth)]
    public float DepthGain {
      set => SetProperty(ref depthGain, value, nameof(DepthGain));
      get => depthGain;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.LaserPower, SensorType = SensorType.Depth)]
    public float DepthLaserPower {
      set => SetProperty(ref depthLaserPower, value, nameof(DepthLaserPower));
      get => depthLaserPower;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.EnableAutoExposure, SensorType = SensorType.Color)]
    public bool IsColorAutoExposureEnabled {
      set => SetProperty(ref isColorAutoExposureEnabled, value, nameof(IsColorAutoExposureEnabled));
      get => isColorAutoExposureEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.EnableAutoWhiteBalance, SensorType = SensorType.Color)]
    public bool IsColorAutoWhiteBalanceEnabled {
      set => SetProperty(ref isColorAutoWhiteBalanceEnabled, value, nameof(IsColorAutoWhiteBalanceEnabled));
      get => isColorAutoWhiteBalanceEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.AutoExposurePriority, SensorType = SensorType.Color)]
    public bool IsColorAutoExposurePriorityEnabled {
      set => SetProperty(ref isColorAutoExposurePriorityEnabled, value, nameof(IsColorAutoExposurePriorityEnabled));
      get => isColorAutoExposurePriorityEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.BacklightCompensation, SensorType = SensorType.Color)]
    public bool IsColorBacklightCompensationEnabled {
      set => SetProperty(ref isColorBacklightCompensationEnabled, value, nameof(IsColorBacklightCompensationEnabled));
      get => isColorBacklightCompensationEnabled;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Brightness, SensorType = SensorType.Color)]
    public float ColorBrightness {
      set => SetProperty(ref colorBrightness, value, nameof(ColorBrightness));
      get => colorBrightness;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Contrast, SensorType = SensorType.Color)]
    public float ColorContrast {
      set => SetProperty(ref colorContrast, value, nameof(ColorContrast));
      get => colorContrast;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Exposure, SensorType = SensorType.Color)]
    public float ColorExposure {
      set => SetProperty(ref colorExposure, value, nameof(ColorExposure));
      get => colorExposure;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Gain, SensorType = SensorType.Color)]
    public float ColorGain {
      set => SetProperty(ref colorGain, value, nameof(ColorGain));
      get => colorGain;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Gamma, SensorType = SensorType.Color)]
    public float ColorGamma {
      set => SetProperty(ref colorGamma, value, nameof(ColorGamma));
      get => colorGamma;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Hue, SensorType = SensorType.Color)]
    public float ColorHue {
      set => SetProperty(ref colorHue, value, nameof(ColorHue));
      get => colorHue;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Saturation, SensorType = SensorType.Color)]
    public float ColorSaturation {
      set => SetProperty(ref colorSaturation, value, nameof(ColorSaturation));
      get => colorSaturation;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.Sharpness, SensorType = SensorType.Color)]
    public float ColorSharpness {
      set => SetProperty(ref colorSharpness, value, nameof(ColorSharpness));
      get => colorSharpness;
    }

    /// <summary></summary>
    [RealSenseSensorOption(Option.WhiteBalance, SensorType = SensorType.Color)]
    public float ColorWhiteBalance {
      set => SetProperty(ref colorWhiteBalance, value, nameof(ColorWhiteBalance));
      get => colorWhiteBalance;
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

    private bool isDepthAutoExposureEnabled = true;
    private bool isDepthEmitterEnabled = true;
    private bool isDepthEmitterOn = false;
    private bool isDepthEmitterAlwaysOn = false;
    private bool isDepthThermalCompensationEnabled = true;
    private bool isDepthHdrEnabled = false;
    private float depthExposure   = 33000f;
    private float depthGain       = 16f;
    private float depthLaserPower = 150f;

    private bool isColorAutoExposureEnabled = true;
    private bool isColorAutoWhiteBalanceEnabled = true;
    private bool isColorAutoExposurePriorityEnabled = true;
    private bool isColorBacklightCompensationEnabled = false;
    private float colorBrightness = 0f;
    private float colorContrast = 50f;
    private float colorExposure = 156f;
    private float colorGain = 64f;
    private float colorGamma = 300f;
    private float colorHue = 0f;
    private float colorSaturation = 64f;
    private float colorSharpness = 50f;
    private float colorWhiteBalance = 4600f;

    private string depthFrameText = string.Empty;
    private string colorFrameText = string.Empty;
    private MotionData? realSenseAccel = null;
  }
}
