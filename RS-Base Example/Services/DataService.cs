using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RS_Base.Net.Model
{
    public class DataService : ObservableObject
    {
        public DataService()
        {
            var RSStandardComponentsVersion = Assembly.GetAssembly(typeof(RS_StandardComponents.GetResourceHandler)).GetName().Version.ToString();
            _title = $"RS-Base Version {RSStandardComponentsVersion} .Net 6.0";
        }

        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }
}