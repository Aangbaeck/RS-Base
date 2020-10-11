using Infralution.Localization.Wpf;
using MaterialDesignThemes.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RS_StandardComponents
{
    /// <summary>
    /// Interaction logic for EmptyWindow.xaml
    /// </summary>
    public partial class RSWindow : Window
    {
        public RSWindow()
        {
            
            InitializeComponent();
            MaximizeButton.Visibility = Visibility.Hidden;
            RestoreButton.Visibility = Visibility.Visible;
            PinButton.Visibility = Visibility.Collapsed;
            UnpinButton.Visibility = Visibility.Collapsed;
            SetMaximizeRestoreIcons(this);
        }

        public static readonly DependencyProperty MinimizableProperty = DependencyProperty.Register("EnableMinimize",
            typeof(bool), typeof(RSWindow), new PropertyMetadata(true, MinPropertyChanged));

        public static readonly DependencyProperty MaximizableProperty = DependencyProperty.Register("EnableMaximize",
            typeof(bool), typeof(RSWindow), new PropertyMetadata(true, MaxPropertyChanged));

        public static readonly DependencyProperty ClosableProperty = DependencyProperty.Register("EnableClosable",
            typeof(bool), typeof(RSWindow), new PropertyMetadata(true, ClosePropertyChanged));

        public static new readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string),
            typeof(RSWindow), new PropertyMetadata(TitlePropertyChanged));

        public static new readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
            typeof(PackIconKind), typeof(RSWindow),
            new PropertyMetadata(PackIconKind.Cake, IconPropertyChanged));

        public static readonly DependencyProperty CheckBeforeCloseProperty =
            DependencyProperty.Register("CheckBeforeClose", typeof(bool), typeof(RSWindow),
                new PropertyMetadata(false));
        public static readonly DependencyProperty EnableFreezeModeProperty =
            DependencyProperty.Register("EnableFreezeMode", typeof(bool), typeof(RSWindow),
                new PropertyMetadata(false, SetFreezeMode));



        public static new readonly DependencyProperty ContentProperty =
   DependencyProperty.Register("Content", typeof(object),
    typeof(RSWindow), new UIPropertyMetadata(null));

        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private bool _mRestoreForDragMove;

        private void SetMaximizeRestoreIcons(Window boundWindow)
        {
            if (boundWindow?.WindowState == WindowState.Maximized)
            {
                MaximizeButton.Visibility = Visibility.Hidden;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                MaximizeButton.Visibility = Visibility.Visible;
                RestoreButton.Visibility = Visibility.Hidden;
            }
        }


        public bool EnableMinimize
        {
            get => (bool)GetValue(MinimizableProperty);
            set => SetValue(MinimizableProperty, value);
        }

        public bool EnableMaximize
        {
            get => (bool)GetValue(MaximizableProperty);
            set => SetValue(MaximizableProperty, value);
        }

        public bool EnableClosable
        {
            get => (bool)GetValue(ClosableProperty);
            set => SetValue(ClosableProperty, value);
        }

        public new string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public new PackIconKind Icon
        {
            get => (PackIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public bool CheckBeforeClose
        {
            get => (bool)GetValue(CheckBeforeCloseProperty);
            set => SetValue(CheckBeforeCloseProperty, value);
        }
        public bool EnableFreezeMode
        {
            get => (bool)GetValue(EnableFreezeModeProperty);
            set => SetValue(EnableFreezeModeProperty, value);
        }




        private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is PackIconKind icon)) return;
            ((RSWindow)d).TitleIcon.Kind = icon;
        }












        private static void SetFreezeMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool enableFreezeMode)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((RSWindow)d).PinButton.Visibility = enableFreezeMode ? Visibility.Visible : Visibility.Collapsed;

        }
        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is string s)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((RSWindow)d).TitleText.Text = s;
        }

        private static void ClosePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((RSWindow)d).CloseButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private static void MinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((RSWindow)d).MinimizeButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private static void MaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((RSWindow)d).MaximizeButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void PinWindow(object sender, RoutedEventArgs e)
        {
            TitleBar.Visibility = Visibility.Collapsed;
            this.ResizeMode = ResizeMode.NoResize;
            UnpinButton.Visibility = Visibility.Visible;
        }
        private void UnpinWindow(object sender, RoutedEventArgs e)
        {
            TitleBar.Visibility = Visibility.Visible;
            this.ResizeMode = ResizeMode.CanResize;
            UnpinButton.Visibility = Visibility.Collapsed;
        }

        //These mouse methods is used for normal window behaviour and still it's a borderless stylable window
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.ResizeMode != ResizeMode.CanResize &&
                    this.ResizeMode != ResizeMode.CanResizeWithGrip)
                {
                    return;
                }
                MaximizeRestore();
            }
            else
            {
                _mRestoreForDragMove = this.WindowState == WindowState.Maximized;
                this.DragMove();
            }
        }

        private void MaximizeRestore()
        {
            if (this.WindowState == WindowState.Maximized)
            {

                this.WindowState = WindowState.Normal;
                MaximizeButton.ToolTip = ResxExtension.GetResourceValue("MaximizeTT", "RS_StandardComponents.Localization.TitlebarUserCtrl");
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.SizeToContent = SizeToContent.Manual;
                this.WindowState = WindowState.Maximized;
                MaximizeButton.ToolTip = ResxExtension.GetResourceValue("RestoreTT", "RS_StandardComponents.Localization.TitlebarUserCtrl");
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_mRestoreForDragMove)
                {
                    _mRestoreForDragMove = false;

                    var point = PointToScreen(e.MouseDevice.GetPosition(this));

                    this.Left = point.X - (this.RestoreBounds.Width * 0.5);
                    this.Top = point.Y;
                    this.WindowState = WindowState.Normal;

                    this.DragMove();
                }
            }
            catch (Exception ee)
            {
                Log.Error(ee, "Could not move window.");
            }
        }

        private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
        {
            MaximizeRestore();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mRestoreForDragMove = false;
        }


        //These methods is to make the title text a bit faded out when window is not active (e.g. how visual studio looks)
        private void WindowDeactivated(object sender, EventArgs e)
        {
            TitleIcon.Opacity = 0.4;
            TitleText.Opacity = 0.4;
            Border.Opacity = 0.4;
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            TitleIcon.Opacity = 1;
            TitleText.Opacity = 0.9;
            Border.Opacity = 1;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (CheckBeforeClose)
                DialogHost.IsOpen = true;
            else
            {
                try
                {
                    this.Close(); //Close this window (Main)
                }
                catch
                {
                    Log.Error("Could not close window properly");
                }
            }
        }

        private void OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter != null && (string)eventArgs.Parameter != "True") return;
            try
            {
                this.Close(); //Close this window (Main)
            }
            catch
            {
                Log.Error("Could not close window properly");
            }
        }

        
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (sender is Window win)
            {
                //if (win.WindowState == WindowState.Maximized)
                //{
                //    this.SizeToContent = SizeToContent.Manual;
                //    this.WindowState = WindowState.Maximized;
                //}
                SetMaximizeRestoreIcons(win);
            }
        }

        private void wnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Services.Tracker.Track(this);
        }
    }
}
