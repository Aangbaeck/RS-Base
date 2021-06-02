using System;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using RS_Base.Net.Helper;
using RS_Base.Net.Model;

using RS_Base.Services;
using RS_StandardComponents;
using Serilog;
using System.Drawing;
using System.Diagnostics;

namespace RS_Base.Views
{
    public class MainVM : ViewModelBase
    {
        public readonly DataService D;
        public RelayCommand<string> ChangeTitleLocalCmd => new RelayCommand<string>(ChangeTitleLocal);
        public RelayCommand ChangeTitleDataServiceCmd => new RelayCommand(ChangeTitleDataService);

        public RelayCommand<string> ChangeLanguageCmd => new RelayCommand<string>(lang =>
        {
            CultureManager.UICulture = new CultureInfo(lang);
            S.Settings.Language = lang;
            S.SaveSettings();
        });
        public RelayCommand<bool> ToggleBaseCommand => new RelayCommand<bool>(o =>
        {
            ApplyBase((bool)o);
            S.Settings.IsLightTheme = o;
            S.SaveSettings();
        });
        public RelayCommand FasterCmd => new RelayCommand(() =>
        {
            ViewModelLocator.Instance.WindowManager.OpenSecondWin();
            //ApplyBase((bool)o);
            //S.Settings.IsLightTheme = o;
            //S.SaveSettings();
        });
        public double ZoomFactor
        {
            get
            {
                return WM.ZoomFactor;
            }
            set
            {
                WM.ZoomFactor = value;
            }
        }
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
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(filePath)
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                    }
                }
            }
            catch (Exception e)
            {

                Log.Error(e, "Could not open log file...");
            }
        });
        public BitmapImage ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                RaisePropertyChanged();
            }
        }
        private void ChangeTitleDataService()
        {
            D.Title = "Changed for every window: " + DateTime.Now.ToLongTimeString();
            WelcomeTitle = D.Title;
            ImageSource = ConvertToImageSource(new PackIcon() { Kind = PackIconKind.AccessTime, Width = 1024, Height = 1024, Foreground = new SolidColorBrush(Colors.White) }, System.Drawing.Color.AliceBlue);

        }
        public BitmapImage ConvertToImageSource(FrameworkElement visual, System.Drawing.Color color)
        {
            int width = (int)visual.Width;
            int height = (int)visual.Height;

            // Render to a bitmap
            var bitmapSource = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmapSource.Render(visual);

            // Convert to System.Drawing.Bitmap
            var pixels = new int[width * height];
            bitmapSource.CopyPixels(pixels, 4096, 0);
            var bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    bitmap.SetPixel(x, y, color);

            // Save to .ico format
            var stream = new MemoryStream();
            System.Drawing.Icon.FromHandle(bitmap.GetHicon()).Save(stream);

            //// Convert saved file into .cur format
            //stream.Seek(2, SeekOrigin.Begin);
            //stream.WriteByte(2);
            //stream.Seek(10, SeekOrigin.Begin);
            //stream.WriteByte((byte)(int)(hotSpot.X * width));
            //stream.WriteByte((byte)(int)(hotSpot.Y * height));
            //stream.Seek(0, SeekOrigin.Begin);

            //// Construct Cursor
            //return new Cursor(stream);
            return Convert(bitmap);
        }
        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }


        private void ChangeTitleLocal(string s)
        {

            //To change strings with localized text in back end we could use
            ResxExtension.ShowBrokenManualResX = true;
            WelcomeTitle = ResxExtension.GetValueManual("ChangeLanguageInBackEndString", "RS_Base.Views.Localization.Main");
            WelcomeTitle = "Hallabaloo!";
        }

        private string _welcomeTitle;
        private BitmapImage imageSource;

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
        private static void ApplyBase(bool isLightTheme)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isLightTheme ? Theme.Light : Theme.Dark));
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
        public MainVM(DataService d, SettingsService s, WindowManager wm)
        {
            WM = wm;
            S = s;
            D = d;  //Here you can control that the DataService is correct
            WelcomeTitle = D.Title;  //Setting the initial values from DataService
            ApplyBase(S.Settings.IsLightTheme);
            Messenger.Default.Send("Hej");
            Messenger.Default.Register<string>(this, TaEmotHej);
        }

        private WindowManager WM { get; set; }

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