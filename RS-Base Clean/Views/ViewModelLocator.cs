using System.Globalization;
using GalaSoft.MvvmLight.Ioc;
using RS_Base.Net.Model;
using RS_StandardComponents;

namespace RS_Base.Views
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<MainVM>();

            //Setting language for whole application
            CultureManager.UICulture = new CultureInfo(SimpleIoc.Default.GetInstance<SettingsService>().Settings.Language);
        }
        public MainVM MainVM => SimpleIoc.Default.GetInstance<MainVM>();
        
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            SimpleIoc.Default.GetInstance<MainVM>().Cleanup();
        }
    }
}