using GalaSoft.MvvmLight;
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
            //win.WindowState = WindowState.Maximized;
            win.Show();
            win.UpdateLayout();
            WindowList.Add(win.Title, win);
        }

        private Dictionary<string, RSView> WindowList { get; } = new Dictionary<string, RSView>();
        private string WPath => Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//" + "WindowPositions//").FullName;

        public double ZoomFactor
        {
            get
            {

                return zoomFactor;
            }
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