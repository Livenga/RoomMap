namespace RoomMap.Wpf {
  using System.Reflection;
  using System.Runtime.InteropServices;

  /// <summary></summary>
  public static class Win32 {
    /// <summary></summary>
    [DllImport("kernel32.dll", EntryPoint = "SetDllDirectoryW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);


    /// <summary></summary>
    [DllImport("kernel32.dll", EntryPoint = "GetLastError", CharSet = CharSet.Unicode)]
    public static extern int GetLastError();
  }
}
