using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using RS_Base;
using RS_Base.Net.Model;
using RS_Base.Views;
using RS_StandardComponents;
using Serilog;

namespace RS_Base.Views
{
    public partial class MainV
    {
        public MainV()
        {
            Application.Current.DispatcherUnhandledException += ThreadStuffUI;
            SimpleIoc.Default.Register<SettingsService>();
            SettingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            //MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;  //This makes the window no go underneath the bottom taskbar
            //MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            Log.Information("STARTING APPLICATION...");
            InitializeComponent();
            Messenger.Default.Register<Type>(this, MessengerID.MainWindowV, OpenAnotherWindow);
            Closing += (s, e) =>
            {
                Log.Information("CLOSING APPLICATION...");

                var listOfWindowsToOpenNextTime = new List<Type>();
                var windows = Application.Current.Windows;  //Close every window individually to save their position
                foreach (Window window in windows)
                {
                    var t = window.GetType();
                    if (window == this || t.Name == "AdornerLayerWindow") continue;  //We will close this window below
                    listOfWindowsToOpenNextTime.Add(window.GetType());
                    window.Close();
                }

                var startWindows = JsonConvert.SerializeObject(listOfWindowsToOpenNextTime);
                SettingsService.Settings.WindowsToOpenAtStart = startWindows;
                SettingsService.SaveSettings();
                ViewModelLocator.Cleanup();
            };
        }

        
        public SettingsService SettingsService { get; set; }

        

        private void OpenAnotherWindow(Type window)
        {
            //if (typeof(SecondV) == window)  
            //{
            //    if (IsWindowOpen<SecondV>())  //If window is already open, why open another?
            //        Application.Current.Windows.OfType<SecondV>().First().Activate(); //Attempts to bring the current window to the foreground
            //    else
            //        new SecondV() { Owner = this }.Show();
            //}
            //else if (typeof(AnotherV) == window)
            //{
            //    if (IsWindowOpen<AnotherV>())
            //        Application.Current.Windows.OfType<AnotherV>().First().Activate(); //Attempts to bring the current window to the foreground
            //    else
            //        new AnotherV() { Owner = this }.Show();
            //}
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