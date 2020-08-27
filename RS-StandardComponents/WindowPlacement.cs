using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Newtonsoft.Json;
using Serilog;

namespace RS_StandardComponents
{
    // RECT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

    public static class WindowPlacement
    {
        public static string WindowPositionsPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//" + "WindowPositions/";

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        public static void LoadPlacement(this Window window)
        {
            var className = window.GetType().Name;
            try
            {
                var dir = Path.GetDirectoryName(WindowPositionsPath) ?? "";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var path = WindowPositionsPath + className + ".pos";
                if (File.Exists(path))
                {
                    var pos = File.ReadAllText(path);

                    WINDOWPLACEMENT placement = JsonConvert.DeserializeObject<WINDOWPLACEMENT>(pos);
                    placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                    placement.flags = 0;
                    placement.showCmd = (placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd);
                    SetWindowPlacement(new WindowInteropHelper(window).Handle, ref placement);
                }
            }
            catch (Exception e)
            {
                Log.Error("Couldn't read position for " + className + e.Message + e.StackTrace);
            }

        }

        public static void SavePlacement(this Window window)
        {
            var className = window.GetType().Name;
            try
            {
                var dir = Path.GetDirectoryName(WindowPositionsPath) ?? "";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                GetWindowPlacement(new WindowInteropHelper(window).Handle, out var placement);
                var pos = JsonConvert.SerializeObject(placement);
                File.WriteAllText(WindowPositionsPath +  className + ".pos", pos);
            }
            catch (Exception e)
            {
                Log.Error("Couldn't write position for " + className + e.Message + e.StackTrace);
            }
        }


    }
}


