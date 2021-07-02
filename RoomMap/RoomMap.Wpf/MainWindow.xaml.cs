using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace RoomMap.Wpf {
  using Microsoft.WindowsAPICodePack.Dialogs;
  using Intel.RealSense;
  using RoomMap.Wpf.Data;
  using RoomMap.Wpf.Extensions;


    /// <summary>Interaction logic for MainWindow.xaml</summary>
  public partial class MainWindow : Window {
    /// <summary>ViewModel</summary>
    public MainWindowViewModel ViewModel =>
      (MainWindowViewModel)DataContext;

    private readonly Colorizer colorizer = new();


    /// <summary>ctor</summary>
    public MainWindow() {
      InitializeComponent();
      DataContext = new MainWindowViewModel();
    }


    /// <summary>
    /// Loaded Event
    /// 認識したデバイスの反映と該当するセンサを取得
    /// </summary>
    private async void OnWindowLoaded(
        object source,
        RoutedEventArgs e) {
      if(Application.Current is App _app) {
        ViewModel.OutputDirectory = _app.LocalPath;
      }

      try {
        // 設定反映
        var cfg = await CommonConfig.LoadAsync("app.conf");

        ViewModel.OutputDirectory = cfg.OutputDirectory;
        ViewModel.IsSaveEnabled   = cfg.IsSaveEnabled;

        ViewModel.DepthExposure   = cfg.DepthSensorOption.Exposure;
        ViewModel.DepthGain       = cfg.DepthSensorOption.Gain;
        ViewModel.DepthLaserPower = cfg.DepthSensorOption.LaserPower;

        ViewModel.ColorBrightness = cfg.ColorSensorOption.Brightness;
        ViewModel.ColorContrast   = cfg.ColorSensorOption.Contrast;
        ViewModel.ColorExposure   = cfg.ColorSensorOption.Exposure;
        ViewModel.ColorGain       = cfg.ColorSensorOption.Gain;
        ViewModel.ColorGamma      = cfg.ColorSensorOption.Gamma;
      } catch(Exception except) {
#if DEBUG
        Debug.WriteLine($"w {except.GetType().Name} {except.Message}");
        Debug.WriteLine(except.StackTrace);
#endif
      }

      using(var ctx = new Context()) {
        var devices = ctx.QueryDevices();
        if(devices.Count == 0) {
          MessageBox.Show(
              caption:        "警告",
              messageBoxText: "Intel RealSense デバイスが検出されませんでした.",
              icon:           MessageBoxImage.Warning,
              button:         MessageBoxButton.OK);

          return;
        }

        foreach(var dev in devices) {
          ViewModel.Devices.Add(dev);
        }
        ViewModel.SelectedDevice = ViewModel.Devices.FirstOrDefault();
      }
    }

    /// <summary>
    /// Closing Event
    /// </summary>
    private async void OnWindowClosing(
        object source,
        CancelEventArgs e) {
      if(ViewModel.IsRecording) {
        e.Cancel = true;
        return;
      }

      var cfg = new CommonConfig() {
        OutputDirectory = ViewModel.OutputDirectory,
        IsSaveEnabled = ViewModel.IsSaveEnabled
      };
      cfg.DepthSensorOption.Exposure   = ViewModel.DepthExposure;
      cfg.DepthSensorOption.Gain       = ViewModel.DepthGain;
      cfg.DepthSensorOption.LaserPower = ViewModel.DepthLaserPower;

      cfg.ColorSensorOption.Brightness = ViewModel.ColorBrightness;
      cfg.ColorSensorOption.Contrast   = ViewModel.ColorContrast;
      cfg.ColorSensorOption.Exposure   = ViewModel.ColorExposure;
      cfg.ColorSensorOption.Gain       = ViewModel.ColorGain;
      cfg.ColorSensorOption.Gamma      = ViewModel.ColorGamma;

      try {
        // 設定保存
        await cfg.SaveAsync("app.conf");
      } catch(Exception except) {
#if DEBUG
        Debug.WriteLine($"d {except.GetType().Name} {except.Message}");
        Debug.WriteLine($"{except.StackTrace}");
#endif
      }
    }


    private Sensor? depthSensor = null;
    private Sensor? colorSensor = null;
    private Sensor? motionSensor = null;

    /// <summary>
    /// 操作対象デバイス変更
    /// </summary>
    private  void OnDeviceListSelectionChanged(
        object source,
        SelectionChangedEventArgs e) {
      ViewModel.DepthStreamProfiles.Clear();
      ViewModel.ColorStreamProfiles.Clear();
      ViewModel.MotionStreamProfiles.Clear();

      if(ViewModel.SelectedDevice == null) {
      } else {
        var dev = ViewModel.SelectedDevice;

        depthSensor = dev.QuerySensors()
          .FirstOrDefault(s => s.Is(Extension.DepthStereoSensor));
        colorSensor = dev.QuerySensors()
          .FirstOrDefault(s => s.Is(Extension.ColorSensor));
        motionSensor = dev.QuerySensors()
          .FirstOrDefault(s => s.Is(Extension.MotionSensor));

        if(depthSensor != null) {
          /* Debug.WriteLine($"d Depth Options");
          foreach(var opt in Enum.GetValues(typeof(Option))
              .Cast<Option>()
              .Where(opt => depthSensor.Options.Supports(opt))
              .Select(opt => depthSensor.Options[opt])) {
            Debug.WriteLine($"\t{opt.Key} = {opt.Value} [{opt.Min} - {opt.Max}]");
          } */

          foreach(var prof in depthSensor.StreamProfiles
              .Where(prof => prof.Is(Extension.VideoProfile))
              .Select(prof => prof.As<VideoStreamProfile>())
              .Where(prof => prof.Stream == Stream.Depth && prof.Format == Format.Z16)) {
            ViewModel.DepthStreamProfiles.Add(prof);
          }
          ViewModel.SelectedDepthStreamProfile = ViewModel.DepthStreamProfiles.FirstOrDefault();
        }
        if(colorSensor != null) {
          /* Debug.WriteLine($"d Color Options");
          foreach(var opt in Enum.GetValues(typeof(Option))
              .Cast<Option>()
              .Where(opt => colorSensor.Options.Supports(opt))
              .Select(opt => colorSensor.Options[opt])) {
            Debug.WriteLine($"\t{opt.Key} = {opt.Value} [{opt.Min} - {opt.Max}]");
          } */

          foreach(var prof in colorSensor.StreamProfiles
              .Where(prof => prof.Is(Extension.VideoProfile))
              .Select(prof => prof.As<VideoStreamProfile>())
              .Where(prof => prof.Stream == Stream.Color && prof.Format == Format.Rgb8)) {
            ViewModel.ColorStreamProfiles.Add(prof);
          }
          ViewModel.SelectedColorStreamProfile = ViewModel.ColorStreamProfiles.FirstOrDefault();
        }
        if(motionSensor != null) {
          /* Debug.WriteLine($"d Motion Options");
          foreach(var opt in Enum.GetValues(typeof(Option))
              .Cast<Option>()
              .Where(opt => motionSensor.Options.Supports(opt))
              .Select(opt => motionSensor.Options[opt])) {
            Debug.WriteLine($"\t{opt.Key} = {opt.Value} [{opt.Min} - {opt.Max}]");
          } */

          foreach(var prof in motionSensor.StreamProfiles) {
            ViewModel.MotionStreamProfiles.Add(prof);
          }
          ViewModel.SelectedMotionStreamProfile = ViewModel.MotionStreamProfiles.FirstOrDefault();
        }
      }
    }


    /// <summary>
    /// 出力先選択押下
    /// </summary>
    private void OnChooseOutputDirectoryClick(object source, RoutedEventArgs e) {
      using(var dialog = new CommonOpenFileDialog()) {
        dialog.Title            = "出力先ディレクトリを指定してください.";
        dialog.RestoreDirectory = true;
        dialog.IsFolderPicker   = true;
        dialog.Multiselect      = false;
        dialog.InitialDirectory = ViewModel.OutputDirectory;

        var result = dialog.ShowDialog(this);
        if(result == CommonFileDialogResult.Ok) {
          ViewModel.OutputDirectory = dialog.FileName;
        }
      }
    }

    /// <summary>
    /// 保存状態の切り替え
    /// </summary>
    private void OnSwitchIsSaveEnable(object source, RoutedEventArgs e) {
      isSaveEnabled = ViewModel.IsSaveEnabled;
    }


    private Pipeline? pipeline = null;
    private Context? context = null;

    private CancellationTokenSource? tokenSource = null;
    private Task? pipelineTask = null;
    private long serialNumber = 0;
    // UI Thread 外から操作で使用
    private bool isSaveEnabled = false;
    private Guid targetId = Guid.Empty;

    private WriteableBitmap? depthBitmap = null;
    private WriteableBitmap? colorBitmap = null;

    /// <summary>
    /// 記録の開始 / 停止
    /// </summary>
    private async void OnRecordingClick(
        object          source,
        RoutedEventArgs e) {
      if(! ViewModel.IsRecording) {
        // 開始
        serialNumber = 0;
        targetId     = Guid.NewGuid();

        var outputDirectory = System.IO.Path.Join(
            ViewModel.OutputDirectory,
            targetId.ToString());

        if(isSaveEnabled && ! System.IO.Directory.Exists(outputDirectory)) {
          // ディレクトリの作成
          try {
            System.IO.Directory.CreateDirectory(outputDirectory);
          } catch {
            MessageBox.Show(
                caption:        "エラー",
                messageBoxText: $"出力先ディレクトリ({outputDirectory})\n作成に失敗しました.",
                icon:           MessageBoxImage.Error,
                button:         MessageBoxButton.OK);

            return;
          }
        }
        context  = new Context();
        pipeline = new Pipeline(context);

        if(depthSensor != null) {
          depthSensor.Options[Option.Exposure].Value   = ViewModel.DepthExposure;
          depthSensor.Options[Option.Gain].Value       = ViewModel.DepthGain;
          depthSensor.Options[Option.LaserPower].Value = ViewModel.DepthLaserPower;
        }
        if(colorSensor != null) {
          colorSensor.Options[Option.Brightness].Value = ViewModel.ColorBrightness;
          colorSensor.Options[Option.Contrast].Value   = ViewModel.ColorContrast;
          colorSensor.Options[Option.Exposure].Value   = ViewModel.ColorExposure;
          colorSensor.Options[Option.Gain].Value       = ViewModel.ColorGain;
          colorSensor.Options[Option.Gamma].Value      = ViewModel.ColorGamma;
        }

        var cfg = new Config();
        cfg.DisableAllStreams();

        if(ViewModel.SelectedDepthStreamProfile != null) {
          var prof = ViewModel.SelectedDepthStreamProfile.As<VideoStreamProfile>();

          depthBitmap = new WriteableBitmap(
              pixelWidth:  prof.Width,
              pixelHeight: prof.Height,
              dpiX:        96d,
              dpiY:        96d,
              pixelFormat: PixelFormats.Rgb24,
              palette:     null);
          DepthImage.Source = depthBitmap;

          // DpethStream 有効化
          cfg.EnableStream(
              stream_type:  prof.Stream,
              stream_index: prof.Index,
              width:        prof.Width,
              height:       prof.Height,
              framerate:    prof.Framerate,
              format:       prof.Format);
        }
        if(ViewModel.SelectedColorStreamProfile != null) {
          var prof = ViewModel.SelectedColorStreamProfile.As<VideoStreamProfile>();

          colorBitmap = new WriteableBitmap(
              pixelWidth:  prof.Width,
              pixelHeight: prof.Height,
              dpiX:        96d,
              dpiY:        96d,
              pixelFormat: PixelFormats.Rgb24,
              palette:     null);
          ColorImage.Source = colorBitmap;

          // Color Stream 有効化
          cfg.EnableStream(
              stream_type:  prof.Stream,
              stream_index: prof.Index,
              width:        prof.Width,
              height:       prof.Height,
              framerate:    prof.Framerate,
              format:       prof.Format);
        }
        if(ViewModel.SelectedMotionStreamProfile != null) {
          var prof = ViewModel.SelectedMotionStreamProfile.As<MotionStreamProfile>();

          // Accel Stream 有効化
          cfg.EnableStream(
              stream_type:  prof.Stream,
              stream_index: prof.Index,
              framerate:    prof.Framerate,
              format:       prof.Format);
        }

        if(isSaveEnabled && ViewModel.SelectedDevice != null) {
          try {
            await ViewModel
              .SelectedDevice
              .SaveInfoAsync(System.IO.Path.Join(
                    ViewModel.OutputDirectory,
                    targetId.ToString(),
                    "device.json"));
          } catch(Exception except) {
#if DEBUG
            Debug.WriteLine($"w {except.GetType().Name} {except.Message}");
#endif
          }
        }

        try {
          pipeline.Start(cfg);
          tokenSource = new CancellationTokenSource();

          pipelineTask = Task.Factory.StartNew(
              OnPipelineTaskAsync,
              pipeline,
              tokenSource.Token);

          ViewModel.IsRecording = true;
          } catch {
          tokenSource?.Dispose();
          pipeline.Dispose();
          context.Dispose();
        }
      } else { 
        ViewModel.IsRecording = false;
        RecordButton.IsEnabled = false;

        // 停止
        await Task.Factory.StartNew(() => pipeline?.Stop());

        try {
          tokenSource?.Cancel(true);
          pipelineTask?.Wait();
        } catch { }

        context?.Dispose();
        targetId = Guid.Empty;

        RecordButton.IsEnabled = true;
      }
    }

    /// <summary></summary>
    private async Task OnPipelineTaskAsync(object? state) {
      if(state == null || ! (state is Pipeline)) {
        return;
      }


      var _outputDirectory = Dispatcher.Invoke(
          (Func<string?>)(() => ViewModel.IsSaveEnabled
              ? System.IO.Path.Join(ViewModel.OutputDirectory, $"{targetId.ToString()}")
              : null ));

      if(_outputDirectory != null && ! System.IO.Directory.Exists(_outputDirectory)) {
        System.IO.Directory.CreateDirectory(_outputDirectory);
      }

      var pipeline = (Pipeline)state;

      while(! (tokenSource?.Token.IsCancellationRequested ?? true)) {
        using(var frames = pipeline.WaitForFrames(1500)) {
          if(tokenSource?.Token.IsCancellationRequested ?? true) {
            break;
          }

          var _frames = frames.Select(f => f.DisposeWith(frames));

          using(var depth = _frames.FirstOrDefault(f => f.Is(Extension.VideoFrame) && f.Profile.Stream == Stream.Depth)?
              .As<VideoFrame>()
              .DisposeWith(frames)) {
            if(depth != null) {
              if(_outputDirectory != null) {
                await depth.SaveAsync(_outputDirectory, serialNumber);
              }

              using(var colorized = colorizer.Process<VideoFrame>(depth).DisposeWith(frames)) {
                await DepthImage.Dispatcher.BeginInvoke(
                    (Action<Image, VideoFrame>)InvokeApplyVideoFrame,
                    new object[] { DepthImage, colorized });
              }
            }
          }

          using(var color = _frames.FirstOrDefault(f => f.Is(Extension.VideoFrame) && f.Profile.Stream == Stream.Color)?
              .As<VideoFrame>()
              .DisposeWith(frames)) {
            if(color != null) {
              if(_outputDirectory != null) {
                await color.SaveAsync(_outputDirectory, serialNumber);
              }

              await DepthImage.Dispatcher.BeginInvoke(
                  (Action<Image, VideoFrame>)InvokeApplyVideoFrame,
                  new object[] { ColorImage, color });
            }
          }

          using(var motion = _frames.FirstOrDefault(f => f.Is(Extension.MotionFrame))?.As<MotionFrame>().DisposeWith(frames)) {
            if(motion != null) {
              await RealSenseAccelControl.Dispatcher.BeginInvoke(
                  (Action<MotionFrame>)InvokeMotionData,
                  new object[] { motion });
            }
          }
        }

        ++serialNumber;
      }
    }

    /// <summary>
    /// Image 反映
    /// </summary>
    private void InvokeApplyVideoFrame(
        Image      image,
        VideoFrame frame) {
      if(image.Source != null && image.Source is WriteableBitmap bmp) {
        var rect = new Int32Rect(0, 0, frame.Width, frame.Height);
        bmp.WritePixels(rect, frame.Data, frame.DataSize, frame.Stride);
      }

      if(image == DepthImage) {
        ViewModel.DepthFrameText = $"{frame.Number} {frame.Timestamp}";
      } else if(image == ColorImage) {
        ViewModel.ColorFrameText = $"{frame.Number} {frame.Timestamp}";
      }
    }

    /// <summary></summary>
    private void InvokeMotionData(MotionFrame frame) {
      var motion = new MotionData(
          frame.MotionData.x,
          frame.MotionData.y,
          frame.MotionData.z);

      ViewModel.RealSenseAccel = motion;
    }
  }
}
