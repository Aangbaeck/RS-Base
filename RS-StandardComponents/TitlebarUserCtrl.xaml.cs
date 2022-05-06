using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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
                new PropertyMetadata(NewWindowAdded));

        public static readonly DependencyProperty MinimizableProperty = DependencyProperty.Register("EnableMinimize",
            typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(true, MinPropertyChanged));

        public bool EnablePinMode
        {
            get { return (bool)GetValue(EnablePinModeProperty); }
            set { SetValue(EnablePinModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnablePinMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnablePinModeProperty = DependencyProperty.Register("EnablePinMode", typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(false, PinModeChanged));
        public static readonly DependencyProperty MaximizableProperty = DependencyProperty.Register("EnableMaximize", typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(true, MaxPropertyChanged));
        public static readonly DependencyProperty ClosableProperty = DependencyProperty.Register("EnableClosable", typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(true, ClosePropertyChanged));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitlebarUserCtrl), new PropertyMetadata(TitlePropertyChanged));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(PackIconKind), typeof(TitlebarUserCtrl), new PropertyMetadata(PackIconKind.Cake, IconPropertyChanged));
        public static readonly DependencyProperty CheckBeforeCloseProperty = DependencyProperty.Register("CheckBeforeClose", typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(false));
        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(TitlebarUserCtrl), new UIPropertyMetadata(null));

        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private Window _localWindow;
        private bool _mRestoreForDragMove;

        public TitlebarUserCtrl()
        {
            InitializeComponent();
            MaximizeButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Visible;
        }

        public Window LocalWindow
        {
            get => _localWindow;
            set
            {
                _localWindow = value;
                SetMaximizeRestoreIcons(BoundWindow);
            }
        }

        private void SetMaximizeRestoreIcons(Window boundWindow)
        {
            if (boundWindow?.WindowState == WindowState.Maximized)
            {
                MaximizeButton.Visibility = Visibility.Collapsed;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                MaximizeButton.Visibility = Visibility.Visible;
                RestoreButton.Visibility = Visibility.Collapsed;
            }
        }

        //private WINDOWPLACEMENT 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Window BoundWindow
        {
            get => (Window)GetValue(BoundCurrentWindowProperty);
            set => SetValue(BoundCurrentWindowProperty, value);
        }


        /// <summary>
        /// Hide or show minimize button titlebar
        /// </summary>
        public bool EnableMinimize
        {
            get => (bool)GetValue(MinimizableProperty);
            set => SetValue(MinimizableProperty, value);
        }


        /// <summary>
        /// Hide or show maximize button titlebar
        /// </summary>
        public bool EnableMaximize
        {
            get => (bool)GetValue(MaximizableProperty);
            set => SetValue(MaximizableProperty, value);
        }
        /// <summary>
        /// Hide or show close button titlebar
        /// </summary>
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
        /// <summary>
        /// show popup question before closing
        /// </summary>
        public bool CheckBeforeClose
        {
            get => (bool)GetValue(CheckBeforeCloseProperty);
            set => SetValue(CheckBeforeCloseProperty, value);
        }



        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPinned.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.Register("IsPinned", typeof(bool), typeof(TitlebarUserCtrl), new PropertyMetadata(false));



        private void StateChanged(object sender, EventArgs e)
        {
            if (sender is Window win)
            {
                if (win.WindowState == WindowState.Maximized)
                {
                    BoundWindow.SizeToContent = SizeToContent.Manual;
                    BoundWindow.WindowState = WindowState.Maximized;
                }
                SetMaximizeRestoreIcons(win);
            }
        }

        private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is PackIconKind icon)) return;
            ((TitlebarUserCtrl)d).TitleIcon.Kind = icon;
        }
        public static event EventHandler Closing;

        private static void NewWindowAdded(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is Window win)) return;
            var bar = ((TitlebarUserCtrl)d);
            win.Deactivated += bar.WindowDeactivated;
            win.Activated += bar.WindowActivated;
            win.StateChanged += bar.StateChanged;
            bar.LocalWindow = win;
            win.Closing += (a, o) =>
            {
                try
                {
                    

                    Closing?.Invoke(bar, EventArgs.Empty);
                    win.Deactivated -= bar.WindowDeactivated;
                    win.Activated -= bar.WindowActivated;
                    win.StateChanged -= bar.StateChanged;
                    
                }
                catch (Exception ee)
                {
                    Log.Error(ee, "Could not save window position or unsubscribe from state changes.");
                }
            };
            win.Closed += (a, o) =>
            {

                win.Dispatcher.InvokeAsync(() => {
                    // added these
                    if (win?.Owner is RSView parent)
                        try
                        {
                            parent?.Activate();
                            parent?.Focus();
                        }
                        catch (Exception ee)
                        {
                            Log.Error("Could not activate window... Please check why", ee);

                        }
                }, DispatcherPriority.Send);
                
                    
            };

        }



        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is string s)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((TitlebarUserCtrl)d).TitleText.Text = s;
        }

        private static void ClosePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((TitlebarUserCtrl)d).CloseButton.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void MinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((TitlebarUserCtrl)d).MinimizeButton.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void MaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((TitlebarUserCtrl)d).MaxRestoreGrid.Visibility = b ? Visibility.Visible : Visibility.Collapsed;

        }
        private static void PinModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool b)) { Log.Error($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            ((TitlebarUserCtrl)d).PinButton.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
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
                //BoundWindow.SizeToContent = SizeToContent.WidthAndHeight;
                BoundWindow.WindowState = WindowState.Normal;
            }
            else if (BoundWindow.WindowState == WindowState.Normal)
            {
                BoundWindow.SizeToContent = SizeToContent.Manual;
                BoundWindow.WindowState = WindowState.Maximized;
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


        
        private void WindowDeactivated(object sender, EventArgs e)
        {
            TitleIcon.Opacity = 0.4;  //This is to make the title text a bit faded out when window is not active (e.g. how visual studio looks)
            TitleText.Opacity = 0.4;
            Border.Opacity = 0.4;
            if (!IsPinned && EnablePinMode)
            {
                if (BoundWindow?.Owner is RSView parent)
                {
                    BoundWindow?.Close();
                    parent?.Activate();
                    parent?.Focus();
                }
            }
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
                    BoundWindow?.Close(); //Close this window (Main)
                }
                catch
                {
                    Log.Error("Could not close window properly");
                }
            }
        }

        private void OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter == null || eventArgs.Parameter.ToString() != "True") return;

            try
            {
                BoundWindow.Close(); //Close this window (Main)
            }
            catch
            {
                Log.Error("Could not close window properly");
            }
        }

        private void PinWindow(object sender, RoutedEventArgs e)
        {
            IsPinned = !IsPinned;
            if (IsPinned)
            {
                PinButtonIcon.Kind = PackIconKind.Pin;
                PinButton.ToolTip = "Pinned";
            }
            else
            {
                PinButtonIcon.Kind = PackIconKind.PinOff;
                PinButton.ToolTip = "Not pinned";
            }
        }


    }
}
