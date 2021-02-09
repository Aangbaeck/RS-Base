/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:RS_Base.Net_2018041.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System.Globalization;
using GalaSoft.MvvmLight.Ioc;
using RS_Base.Net.Model;
using RS_Base.Net.Views;
using RS_Base.Services;
using RS_StandardComponents;

namespace RS_Base.Views
{
    public class ViewModelLocator
    {
        private static ViewModelLocator instance = null;
        private static readonly object padlock = new object();

        public static ViewModelLocator Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ViewModelLocator();
                    }
                    return instance;
                }
            }
        }
        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<DataService>();
            SimpleIoc.Default.Register<WindowManager>();
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<MainVM>();
            SimpleIoc.Default.Register<SecondVM>();

            //Setting language for whole application
            CultureManager.UICulture = new CultureInfo(SimpleIoc.Default.GetInstance<SettingsService>().Settings.Language);
        }
        public MainVM MainVM => SimpleIoc.Default.GetInstance<MainVM>();
        public SecondVM SecondVM => SimpleIoc.Default.GetInstance<SecondVM>();
        public WindowManager WindowManager => SimpleIoc.Default.GetInstance<WindowManager>();

        //This can be used to clean up when windows closes (if necessary). Preferably from the WindowManager.
        public static void Cleanup()
        {
            SimpleIoc.Default.GetInstance<MainVM>().Cleanup();
        }
    }
}