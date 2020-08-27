using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Timers;
using System.Windows.Media.Imaging;

namespace RS_Base.Net.Helper
{
    public static class Common
    {
        public static string LogfilesPath { get; set; } = Directory + "Logfiles/";
        public static string SettingsPath { get; set; } = Directory + "Settings/Settings.json";
        public static string Directory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//";
    }

}
