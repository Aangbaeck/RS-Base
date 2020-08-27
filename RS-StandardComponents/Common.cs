using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Media.Imaging;

namespace RS_StandardComponents
{
    public static class Common
    {
        //Used as:
        //DirectoryInfo d = new DirectoryInfo(dir); //Assuming Test is your Folder
        //var files = d.GetFilesByExtensions(".bin", ".json", ".zip").ToList(); //Getting Text files
        //files = files.OrderBy(f => f, new Common.NaturalFileInfoNameComparer()).ToList();
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern int StrCmpLogicalW(string psz1, string psz2);
        }
        /// <summary>
        /// Use this as a comparer to get natural file name sorting (aka windows style)
        /// </summary>
        public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo a, FileInfo b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a.Name, b.Name);
            }
        }
    }
}
