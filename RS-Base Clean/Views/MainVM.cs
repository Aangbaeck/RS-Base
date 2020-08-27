using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RS_Base.Net.Helper;

namespace RS_Base.Views
{
    public class MainVM : ViewModelBase
    {
        public RelayCommand OpenLogFile => new RelayCommand(() =>
        {
            var directory = Path.GetDirectoryName(Common.LogfilesPath);
            var files = Directory.GetFiles(directory, "*.log");
            if (files.Length > 0)
            {
                var filePath = files[^1];  //^1 is the same as files.Length-1
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }

        });
    }
}