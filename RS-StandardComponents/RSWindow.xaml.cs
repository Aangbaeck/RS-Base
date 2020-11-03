using System.Windows;

namespace RS_StandardComponents
{
    /// <summary>
    /// Interaction logic for EmptyWindow.xaml
    /// </summary>
    public partial class RSWindow : Window
    {
        public RSWindow()
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;  //This makes the window no go underneath the bottom taskbar
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            InitializeComponent();
        }
        public void SetContent(object userctrl)
        {
            Titlebar.Content = userctrl;
            Titlebar.BoundWindow = this;
        }

        public void SetTitle(string title)
        {
            Titlebar.Title = title;
            Title = title;
        }

        public void SetDataContext(object context)
        {
            Titlebar.DataContext = context;
        }
        public object GetDataContext()
        {
            return Titlebar.DataContext;
        }
        public override string ToString()
        {
            return Title;
        }
        
    }
}
