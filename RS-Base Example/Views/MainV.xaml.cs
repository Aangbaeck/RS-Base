using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using RS_Base.Net.Model;
using RS_Base.Net.Views;
using RS_Base.Services;
using RS_StandardComponents;
using Serilog;

namespace RS_Base.Views
{
    public partial class MainV
    {
        public MainV()
        {

            Application.Current.DispatcherUnhandledException += ThreadStuffUI;

            //MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;  //This makes the window not go underneath the bottom taskbar
            //MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            
            Log.Information("STARTING APPLICATION...");
            InitializeComponent();
        }
        
        private readonly string ResXPath = "Client.Views.Main";
        

        private static ViewModelLocator VMLocator => (Application.Current.TryFindResource("Locator") as ViewModelLocator);
        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }
        private void OpenSecondWindow(object sender, RoutedEventArgs e)
        {
            var w = new SecondWindow();
            w.Show();
            //VMLocator.WindowManager.OpenWindowButLoadOldSettings("Second window", typeof(SecondV));
        }

        private void wnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// This often finds weird threading errors in the UI.
        /// </summary>
        private void ThreadStuffUI(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Some UI Error!");
        }
    }
}