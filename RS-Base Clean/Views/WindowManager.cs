using GalaSoft.MvvmLight;
using RS_StandardComponents;
using System.Collections.Generic;

namespace RS_Base.Views
{
    public partial class WindowManager : ObservableObject
    {
        public RSView MainWindow { get; private set; }
        public void CreateMainWindow()
        {
            var win = new MainV();
            win.Show();
            win.UpdateLayout();
            MainWindow = win;
            WindowList.Add(win.Title, win);
        }

        private Dictionary<string, RSView> WindowList { get; } = new Dictionary<string, RSView>();
        

    }
}