using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace RoomMap.Wpf {
  using System.IO;
  using System.Reflection;

    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : Application {
      /// <summary></summary>
      public string LocalPath => Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Assembly.GetExecutingAssembly().GetName().Name ?? "RoomMap.Wpf");


      /// <summary></summary>
      private void OnAppStartup(object source, StartupEventArgs e) {
        bool isSuccessful = false;
        if(IntPtr.Size == 8) {
          isSuccessful = Win32
            .SetDllDirectory(@"C:\Program Files (x86)\Intel RealSense SDK 2.0\bin\x64");
        } else {
          isSuccessful = Win32
            .SetDllDirectory(@"C:\Program Files (x86)\Intel RealSense SDK 2.0\bin\x86");
        }
        if(! isSuccessful) {
          throw new Win32Exception(Win32.GetLastError());
        }

        // アプリケーションディレクトリの作成
        try {
          if(! Directory.Exists(LocalPath)) {
            Directory.CreateDirectory(LocalPath);
          }
        } catch { }
      }
    }
}
