using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RS_Base.Net.Helper;

namespace RS_Base.Views
{
    public class MainVM : ObservableRecipient
    {
        public RelayCommand OpenLogFile => new RelayCommand(() =>
        {
            var directory = Path.GetDirectoryName(Common.LogfilesPath);
            var files = Directory.GetFiles(directory, "*.log");
            if (files.Length > 0)
            {
                var filePath = files[files.Length-1];  //^1 is the same as files.Length-1
                if (File.Exists(filePath))
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(filePath)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
            }

        });
    }
}