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
            SimpleIoc.Default.Register<SettingsService>();
            SettingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;  //This makes the window no go underneath the bottom taskbar
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            Log.Information("STARTING APPLICATION...");
            InitializeComponent();
            Messenger.Default.Register<Type>(this, MessengerID.MainWindowV, OpenAnotherWindow);
            Closing += (s, e) =>
            {
                Log.Information("CLOSING APPLICATION...");
                this.SavePlacement();  //Saves this windows position
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
        public RelayCommand OpenNewWindowCmd => new RelayCommand(() => { OpenAnotherWindow(typeof(SecondV)); });
        public RelayCommand OpenWindowWithTabControl => new RelayCommand(() => { OpenAnotherWindow(typeof(TabControlWindowV)); });
        public RelayCommand OpenEmptyWindowCmd => new RelayCommand(() => { OpenAnotherWindow(typeof(EmptyWindowV)); });

        private void wnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.LoadPlacement();  //Sets the last position of the window
        }

        private void OpenAnotherWindow(Type window)
        {
            if (typeof(SecondV) == window)  
            {
                if (IsWindowOpen<SecondV>())  //If window is already open, why open another?
                    Application.Current.Windows.OfType<SecondV>().First().Activate(); //Attempts to bring the current window to the foreground
                else
                    new SecondV() { Owner = this }.Show();
            }
            if (typeof(TabControlWindowV) == window)
            {
                if (IsWindowOpen<TabControlWindowV>())  //If window is already open, why open another?
                    Application.Current.Windows.OfType<TabControlWindowV>().First().Activate(); //Attempts to bring the current window to the foreground
                else
                    new TabControlWindowV() { Owner = this }.Show();
            }
            else if (typeof(EmptyWindowV) == window)
            {
                if (IsWindowOpen<EmptyWindowV>())
                    Application.Current.Windows.OfType<EmptyWindowV>().First().Activate(); //Attempts to bring the current window to the foreground
                else
                    new EmptyWindowV() { Owner = this }.Show();
            }
        }

        
        /// <summary>
        /// This method will check for custom windows as well by specifying T to window type
        /// </summary>
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var startWindows = JsonConvert.DeserializeObject<List<Type>>(SettingsService.Settings.WindowsToOpenAtStart);
                foreach (var w in startWindows)
                {
                    try
                    {
                        OpenAnotherWindow(w);
                    }
                    catch 
                    {
                        //ignore
                    }
                    
                }
            }
            catch
            {
                Log.Error("Could not read window positions setting.");
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