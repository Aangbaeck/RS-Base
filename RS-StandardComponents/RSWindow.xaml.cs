using Infralution.Localization.Wpf;
using MaterialDesignThemes.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
