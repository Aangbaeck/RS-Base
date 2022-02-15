using System.Globalization;
using Autofac;
using RS_Base.Net.Model;
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
            builder.RegisterType<WindowManager>().SingleInstance();
            builder.RegisterType<MainVM>().SingleInstance();
            Container = builder.Build();

            //Setting language for whole application
            CultureManager.UICulture = new CultureInfo(SettingsService.Instance.Settings.Language);
        }
        public MainVM MainVM => Container.Resolve<MainVM>();
        public WindowManager WindowManager => Container.Resolve<WindowManager>();

        public static IContainer Container { get; }

        public static void Cleanup()
        {
            Container.Resolve<MainVM>();
        }
    }
}