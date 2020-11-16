using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using RS_StandardComponents;
using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RS_Base.Views
{
    public partial class WindowManager : ObservableObject
    {
        private double zoomFactor = 1.0;

        public bool AllWindowsAreEditable { get; private set; }
        public RSWindow MainWindow { get; private set; }

        public void CreateMainWindow()
        {

            var win = new RSWindow();
            win.SetContent(new MainV());
            MainWindow = win;
            win.Topmost = true;
            win.Show();
            WindowList.Add(win.Title, win);
        }
        
        private Dictionary<string, RSWindow> WindowList { get; } = new Dictionary<string, RSWindow>();
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