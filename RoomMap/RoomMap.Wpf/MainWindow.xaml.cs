using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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


namespace RoomMap.Wpf {
  using Microsoft.WindowsAPICodePack.Dialogs;
  using Intel.RealSense;
  using RoomMap.Wpf.Data;


    /// <summary>Interaction logic for MainWindow.xaml</summary>
  public partial class MainWindow : Window {
    /// <summary></summary>
    public MainWindowViewModel ViewModel =>
      (MainWindowViewModel)DataContext;

    private readonly Colorizer colorizer = new();


    /// <summary></summary>
    public MainWindow() {
      InitializeComponent();
      DataContext = new MainWindowViewModel();
    }


    /// <summary></summary>
    private void OnWindowLoaded(
        object source,
        RoutedEventArgs e) {
      if(Application.Current is App _app) {
        ViewModel.OutputDirectory = _app.LocalPath;
      }

      using(var ctx = new Context()) {
        var devices = ctx.QueryDevices();
        if(devices.Count == 0) {
          MessageBox.Show(
              caption: "警告",
              messageBoxText: "Intel RealSense デバイスが検出されませんでした.",
              icon: MessageBoxImage.Warning,
              button: MessageBoxButton.OK);

          return;
        }

        foreach(var dev in devices) {
          ViewModel.Devices.Add(dev);
        }
        ViewModel.SelectedDevice = ViewModel.Devices.FirstOrDefault();
      }
    }

    /// <summary></summary>
    private  void OnDeviceListSelectionChanged(
        object source,
        SelectionChangedEventArgs e) {
      ViewModel.DepthStreamProfiles.Clear();
      ViewModel.ColorStreamProfiles.Clear();
      ViewModel.MotionStreamProfiles.Clear();

      if(ViewModel.SelectedDevice == null) {
      } else {
        var dev = ViewModel.SelectedDevice;

        var depthSensor = dev.QuerySensors()
          .FirstOrDefault(s => s.Is(Extension.DepthStereoSensor));
        var colorSensor = dev.QuerySensors()
          .FirstOrDefault(s => s.Is(Extension.ColorSensor));
        var motionSensor = dev.QuerySensors()
          .FirstOrDefault(s => s.Is(Extension.MotionSensor));

        if(depthSensor != null) {
          foreach(var prof in depthSensor.StreamProfiles) {
            ViewModel.DepthStreamProfiles.Add(prof);
          }
          ViewModel.SelectedDepthStreamProfile = ViewModel.DepthStreamProfiles.FirstOrDefault();
        }
        if(colorSensor != null) {
          foreach(var prof in colorSensor.StreamProfiles) {
            ViewModel.ColorStreamProfiles.Add(prof);
          }
          ViewModel.SelectedColorStreamProfile = ViewModel.ColorStreamProfiles.FirstOrDefault();
        }
        if(motionSensor != null) {
          foreach(var prof in motionSensor.StreamProfiles) {
            ViewModel.MotionStreamProfiles.Add(prof);
          }
          ViewModel.SelectedMotionStreamProfile = ViewModel.MotionStreamProfiles.FirstOrDefault();
        }
      }
    }


    /// <summary>出力先選択押下</summary>
    private void OnChooseOutputDirectoryClick(object source, RoutedEventArgs e) {
      using(var dialog = new CommonOpenFileDialog()) {
        dialog.Title = "出力先ディレクトリを指定してください.";
        dialog.RestoreDirectory = true;
        dialog.IsFolderPicker = true;
        dialog.Multiselect = false;
        dialog.InitialDirectory = ViewModel.OutputDirectory;

        var result = dialog.ShowDialog(this);
        if(result == CommonFileDialogResult.Ok) {
          ViewModel.OutputDirectory = dialog.FileName;
        }
      }
    }

    /// <summary>保存状態の切り替え</summary>
    private void OnSwitchIsSaveEnable(object source, RoutedEventArgs e) {
      isSaveEnabled = ViewModel.IsSaveEnabled;
#if DEBUG
      Debug.WriteLine($"d IsSaveEnabled: {isSaveEnabled}");
#endif
    }


    private Pipeline? pipeline = null;
    private Context? context = null;

    private CancellationTokenSource? tokenSource = null;
    private Task? pipelineTask = null;
    private long serialNumber = 0;
    private bool isSaveEnabled = false;

    private WriteableBitmap? depthBitmap = null;
    private WriteableBitmap? colorBitmap = null;


    /// <summary></summary>
    private void OnRecordingClick(object source, RoutedEventArgs e) {
      if(! ViewModel.IsRecording) {
        // 開始
        serialNumber = 0;

        context  = new Context();
        pipeline = new Pipeline(context);

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

          cfg.EnableStream(
              stream_type: prof.Stream,
              stream_index: prof.Index,
              framerate:    prof.Framerate,
              format:       prof.Format);
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
        // 停止
        pipeline?.Stop();

        try {
          tokenSource?.Cancel(true);
          pipelineTask?.Wait();
        } catch { }

        context?.Dispose();
      }
    }

    /// <summary></summary>
    private async Task OnPipelineTaskAsync(object? state) {
      if(state == null || ! (state is Pipeline)) {
        return;
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
              Debug.WriteLine($"d {depth.Width}x{depth.Height} {depth.Profile.Stream} {depth.Profile.Format}");

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
              Debug.WriteLine($"d {color.Width}x{color.Height} {color.Profile.Stream} {color.Profile.Format}");

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

    /// <summary></summary>
    private void InvokeApplyVideoFrame(Image image, VideoFrame frame) {
      //Debug.WriteLine($"d {frame.Width}x{frame.Height} {frame.DataSize}");
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
