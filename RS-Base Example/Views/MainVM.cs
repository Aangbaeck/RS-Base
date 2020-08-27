﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infralution.Localization.Wpf;
using MaterialDesignThemes.Wpf;
using RS_Base.Models;
using RS_Base.Net.Helper;
using RS_Base.Net.Model;
using RS_Base.Services;
using Serilog;

namespace RS_Base.Views
{
    public class MainVM : ViewModelBase
    {
        public readonly DataService D;
        public RelayCommand ChangeTitleLocalCmd => new RelayCommand(ChangeTitleLocal);
        public RelayCommand ChangeTitleDataServiceCmd => new RelayCommand(ChangeTitleDataService);
        public RelayCommand OpenNewWindowCmd => new RelayCommand(() => { Messenger.Default.Send(typeof(SecondV), MessengerID.MainWindowV); });
        public RelayCommand OpenWindowWithTabControl => new RelayCommand(() => { Messenger.Default.Send(typeof(TabControlWindowV), MessengerID.MainWindowV); });
        public RelayCommand<string> ChangeLanguageCmd => new RelayCommand<string>(lang =>
        {
            CultureManager.UICulture = new CultureInfo(lang);
            S.Settings.Language = lang;
            S.SaveSettings();
        });
        
        public RelayCommand OpenLogFile => new RelayCommand(() =>
        {
            try
            {
                var directory = Path.GetDirectoryName(Common.LogfilesPath);
                var files = Directory.GetFiles(directory, "*.log");
                if (files.Length > 0)
                {
                    var filePath = files[files.Length - 1];
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not open log file...");
            }
        });
        private void ChangeTitleDataService()
        {
            D.Title = "Changed for every window: " + DateTime.Now.ToLongTimeString();
            WelcomeTitle = D.Title;
        }

        private void ChangeTitleLocal()
        {
            WelcomeTitle = "Hallabaloo!";
        }

        private string _welcomeTitle;
        private bool _isLightTheme;


        public string WelcomeTitle
        {
            get { return _welcomeTitle; }
            set
            {
                //Set(ref _welcomeTitle, value);  //Set is the same as RaisePropertyChanged. Can send messages as well. 

                // ^ samma sak som
                _welcomeTitle = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ToggleBaseCommand { get; } = new RelayCommand<bool>(o => ApplyBase((bool)!o));

        public bool IsLightTheme
        {
            get { return _isLightTheme; }
            set { _isLightTheme = value; RaisePropertyChanged(); }
        }

        private static void ApplyBase(bool isDark)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
        }
        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. IOC
        /// </summary>
        public MainVM(DataService d, SettingsService s)
        {
            S = s;
            D = d;  //Here you can control that the DataService is correct
            WelcomeTitle = D.Title;  //Setting the initial values from DataService
            Messenger.Default.Send("Hej");
            Messenger.Default.Register<string>(this, TaEmotHej);
        }

        public SettingsService S { get; set; }

        private void TaEmotHej(object str)
        {
            Log.Debug("Vi fick ett Hej!");
        }

        public override void Cleanup()
        {
            // Clean up if needed
            // Save data!
            base.Cleanup();
        }
    }
}