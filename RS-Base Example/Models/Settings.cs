using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace RS_Base.Models
{
    public class Settings : ViewModelBase
    {
        private bool _isLightTheme = false;
        public string WindowsToOpenAtStart { get; set; }
        public string Language { get; set; } = "en-US";
        public bool IsLightTheme { get { return _isLightTheme; } set { _isLightTheme = value; RaisePropertyChanged(); } }
    }
}
