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
using Infralution.Localization.Wpf;
using RS_Base.Net.Model;
using RS_Base.Net.Views;
using RS_Base.Services;

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
            SimpleIoc.Default.Register<DataService>();
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<MainVM>();
            SimpleIoc.Default.Register<SecondVM>();
            SimpleIoc.Default.Register<TabControlWindowVM>();
            

            //Setting language for whole application
            CultureManager.UICulture = new CultureInfo(SimpleIoc.Default.GetInstance<SettingsService>().Settings.Language);
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainVM MainVM => SimpleIoc.Default.GetInstance<MainVM>();
        public SecondVM SecondVM => SimpleIoc.Default.GetInstance<SecondVM>();
        public TabControlWindowVM TabControlWindowVM => SimpleIoc.Default.GetInstance<TabControlWindowVM>();
        

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            SimpleIoc.Default.GetInstance<MainVM>().Cleanup();
        }
    }
}