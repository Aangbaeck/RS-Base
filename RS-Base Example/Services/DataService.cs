﻿using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RS_Base.Net.Model
{
    public class DataService : ObservableObject
    {
        public DataService()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            _title = $"RS-Base Version {version} .Net Core 3.1";
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