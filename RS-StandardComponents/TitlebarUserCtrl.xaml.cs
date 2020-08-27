﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Infralution.Localization.Wpf;
using MaterialDesignThemes.Wpf;
using Serilog;

namespace RS_StandardComponents
{
    /// <summary>
    /// Interaction logic for TitlebarUserCtrl.xaml
    /// </summary>
    public partial class TitlebarUserCtrl
    {
        public static readonly DependencyProperty BoundCurrentWindowProperty =
            DependencyProperty.Register("BoundWindow", typeof(Window), typeof(TitlebarUserCtrl),
                new PropertyMetadata(PropertyChanged));

        public static readonly DependencyProperty MinimizableProperty = DependencyProperty.Register("EnableMinimize",
            typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(true, MinPropertyChanged));

        public static readonly DependencyProperty MaximizableProperty = DependencyProperty.Register("EnableMaximize",
            typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(true, MaxPropertyChanged));

        public static readonly DependencyProperty ClosableProperty = DependencyProperty.Register("EnableClosable",
            typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(true, ClosePropertyChanged));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string),
            typeof(TitlebarUserCtrl), new PropertyMetadata(TitlePropertyChanged));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
            typeof(PackIconKind), typeof(TitlebarUserCtrl),
            new PropertyMetadata(PackIconKind.Cake, IconPropertyChanged));

        public static readonly DependencyProperty CheckBeforeCloseProperty =
            DependencyProperty.Register("CheckBeforeClose", typeof(bool), typeof(TitlebarUserCtrl),
                new PropertyMetadata(false));

        private Window _localWindow;
        private bool _mRestoreForDragMove;

        public TitlebarUserCtrl()
        {
            InitializeComponent();
            MaximizeIcon.Kind = PackIconKind.WindowMaximize;
        }

        public Window LocalWindow
        {
            get => _localWindow;
            set
            {
                _localWindow = value;
                BoundWindow?.LoadPlacement();
                MaximizeIcon.Kind = BoundWindow?.WindowState == WindowState.Maximized ? PackIconKind.WindowRestore : PackIconKind.WindowMaximize;
            }
        }

        //private WINDOWPLACEMENT 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Window BoundWindow
        {
            get => (Window)GetValue(BoundCurrentWindowProperty);
            set => SetValue(BoundCurrentWindowProperty, value);
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

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public PackIconKind Icon
        {
            get => (PackIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public bool CheckBeforeClose
        {
            get => (bool)GetValue(CheckBeforeCloseProperty);
            set => SetValue(CheckBeforeCloseProperty, value);
        }


        private void StateChanged(object sender, EventArgs e)
        {
            if (sender is Window win)
            {
                if (win.WindowState == WindowState.Maximized)
                {
                    BoundWindow.SizeToContent = SizeToContent.Manual;
                    BoundWindow.WindowState = WindowState.Maximized;
                }
                MaximizeIcon.Kind = win.WindowState == WindowState.Maximized
                    ? PackIconKind.WindowRestore
                    : PackIconKind.WindowMaximize;
            }
        }

        private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is PackIconKind icon)) return;
            ((TitlebarUserCtrl)d).TitleIcon.Kind = icon;
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is Window win)) return;
            var bar = ((TitlebarUserCtrl)d);
            win.Deactivated += bar.WindowDeactivated;
            win.Activated += bar.WindowActivated;
            win.StateChanged += bar.StateChanged;
            bar.LocalWindow = win;
            win.Closing += WindowClosing;
        }

        //This method is save the actual position of the window to file "WindowName.pos"
        private static void WindowClosing(object window, CancelEventArgs ee)
        {
            try
            {
                ((Window)window).SavePlacement();
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not save window position.");
            }
        }

        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is string s)) return;
            ((TitlebarUserCtrl)d).TitleText.Text = s;
        }

        private static void ClosePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) return;
            ((TitlebarUserCtrl)d).CloseButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private static void MinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) return;
            ((TitlebarUserCtrl)d).MinimizeButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private static void MaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) return;
            ((TitlebarUserCtrl)d).MaximizeButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            BoundWindow.WindowState = WindowState.Minimized;
        }

        //These mouse methods is used for normal window behaviour and still it's a borderless stylable window
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (BoundWindow == null)
            {
                Log.Error("No window is bound to the TitleBar usercontrol. Add this to the XAML: 'BoundWindow=\"{ Binding RelativeSource = { RelativeSource FindAncestor, AncestorType ={ x:Type Window}}}\"'");
                return;
            }

            if (e.ClickCount == 2)
            {
                if (BoundWindow.ResizeMode != ResizeMode.CanResize &&
                    BoundWindow.ResizeMode != ResizeMode.CanResizeWithGrip)
                {
                    return;
                }
                MaximizeRestore();
            }
            else
            {
                _mRestoreForDragMove = BoundWindow.WindowState == WindowState.Maximized;
                BoundWindow.DragMove();
            }
        }

        private void MaximizeRestore()
        {
            if (BoundWindow.WindowState == WindowState.Maximized)
            {
                BoundWindow.SizeToContent = SizeToContent.WidthAndHeight;
                BoundWindow.WindowState = WindowState.Normal;
                MaximizeButton.ToolTip = ResxExtension.GetResourceValue("MaximizeTT", "RS_StandardComponents.Localization.TitlebarUserCtrl");
            }
            else if (BoundWindow.WindowState == WindowState.Normal)
            {
                BoundWindow.SizeToContent = SizeToContent.Manual;
                BoundWindow.WindowState = WindowState.Maximized;
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

                    BoundWindow.Left = point.X - (BoundWindow.RestoreBounds.Width * 0.5);
                    BoundWindow.Top = point.Y;
                    BoundWindow.WindowState = WindowState.Normal;

                    BoundWindow.DragMove();
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
                    BoundWindow.Close(); //Close this window (Main)
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
                BoundWindow.Close(); //Close this window (Main)
            }
            catch
            {
                Log.Error("Could not close window properly");
            }
        }
    }
}