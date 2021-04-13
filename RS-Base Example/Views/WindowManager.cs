using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using RS_StandardComponents;
using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace RS_Base.Views
{
    public partial class WindowManager : ObservableObject
    {
        private double zoomFactor = 1.0;

        public bool AllWindowsAreEditable { get; private set; }
        public RSView MainWindow { get; private set; }

        public void CreateMainWindow()
        {
            var win = new MainV();
            win.Topmost = true;
            MainWindow = win;
            win.Show();
                        //win.UpdateLayout();
            //WindowList.Add(win.Title, win);
        }

        public RelayCommand OpenSecondWindow => new RelayCommand(() =>
        {
            var win = new SecondWindow();
            win.EnablePinMode = true;
            win.Owner = MainWindow;
            win.Show();
            win.Closing += (e, o) => { WindowList.Remove(win.Title); };
            
            WindowList.Add(win.Title, win);
        });
        private Dictionary<string, RSView> WindowList { get; } = new Dictionary<string, RSView>();
        private string WPath => Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//" + "WindowPositions//").FullName;

        public double ZoomFactor
        {
            get => zoomFactor;
            set
            {
                zoomFactor = value;
                foreach (var win in WindowList)
                    win.Value.ZoomFactor = zoomFactor;
            }
        }

        private string FilePath(string title) => WPath + title + ".state";


    }
}