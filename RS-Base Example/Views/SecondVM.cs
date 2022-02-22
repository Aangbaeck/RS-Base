using System.ComponentModel;
using System.Reflection;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using RS_Base.Net.Model;

namespace RS_Base.Net.Views
{
    public class SecondVM : ObservableObject
    {
        private string _propertyName;

        public SecondVM()
        {
            Thread.Sleep(5000);

        }
        
    }
}