using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace RS_Base.Views
{
    public class TabControlWindowVM: ViewModelBase
    {
        private int _selectedIndex;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set { _selectedIndex = value; RaisePropertyChanged();}
        }
    }
}
