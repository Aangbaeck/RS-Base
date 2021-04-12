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
using Autofac;
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
            var builder = new ContainerBuilder();

            builder.RegisterType<DataService>().SingleInstance();
            builder.RegisterType<WindowManager>().SingleInstance();
            builder.RegisterType<SettingsService>().SingleInstance();

            builder.RegisterType<MainVM>().SingleInstance();
            builder.RegisterType<SecondVM>().SingleInstance();

            //Build the container
            Container = builder.Build();

            //Setting language for whole application
            CultureManager.UICulture = new CultureInfo(Container.Resolve<SettingsService>().Settings.Language);
        }
        public MainVM MainVM => Container.Resolve<MainVM>();
        public SecondVM SecondVM => Container.Resolve<SecondVM>();
        public WindowManager WindowManager => Container.Resolve<WindowManager>();

        private static IContainer Container { get; }

        //This can be used to clean up when windows closes (if necessary). Preferably from the WindowManager.
        public static void Cleanup()
        {
            Container.Resolve<MainVM>().Cleanup();
        }
    }
}