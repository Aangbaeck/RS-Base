using MaterialDesignThemes.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "RS_StandardComponents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "RS_StandardComponents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "RS_StandardComponents")]
namespace RS_StandardComponents
{
    /// <summary>
    /// Interaction logic for EmptyWindow.xaml
    /// </summary>
    public partial class RSView : Window
    {
        private void SetTitlebarIcon(PackIconKind icon)
        {
            Titlebar.Icon = icon;
        }

        private void SetTaskbarIcon(PackIconKind icon)
        {
            var usrCtrl = new Grid();
            var border = new Border() { Background = new SolidColorBrush(WindowIconBackgroundColor), CornerRadius = new CornerRadius(50) };
            var packIcon = new PackIcon() { Kind = icon, Width = 256, Height = 256, Foreground = new SolidColorBrush(WindowIconForegroundColor), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            usrCtrl.Children.Add(border);
            usrCtrl.Children.Add(packIcon);
            usrCtrl.Measure(new Size(256, 256));
            usrCtrl.Arrange(new Rect(new Size(256, 256)));
            usrCtrl.UpdateLayout();
            ImageSource image = VisualToImageSourceConverter.ConvertInCode(usrCtrl);
            base.Icon = image;
        }
        //This works on Window.
        //    <Window.Icon>
        //    <Binding Converter = "{StaticResource ConvertMaterialDesignIconToIcon}" >
        //        < Binding.Source >
        //            < materialDesign:PackIcon
        //                 Width = "256"
        //                Height="256"
        //                Foreground="WhiteSmoke"
        //                Kind="TestTube" />
        //        </Binding.Source>
        //    </Binding>
        //</Window.Icon>

        public Color WindowIconForegroundColor
        {
            get { return (Color)GetValue(WindowIconForegroundColorProperty); }
            set { SetValue(WindowIconForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty WindowIconForegroundColorProperty =
            DependencyProperty.Register("WindowIconForegroundColor", typeof(Color), typeof(RSView), new PropertyMetadata(Colors.White, ChangedColorCallback));

        private static void ChangedColorCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is Color color)) return;
            ((RSView)d).SetTitlebarIcon(((RSView)d).TitlebarIcon);

        }

        public Color WindowIconBackgroundColor
        {
            get { return (Color)GetValue(WindowIconBackgroundColorProperty); }
            set { SetValue(WindowIconBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty WindowIconBackgroundColorProperty = DependencyProperty.Register("WindowIconBackgroundColor", typeof(Color), typeof(RSView), new PropertyMetadata(Colors.Transparent, ChangedColorCallback));



        public PackIconKind TaskbarIcon
        {
            get { return (PackIconKind)GetValue(TaskbarIconProperty); }
            set { SetValue(TaskbarIconProperty, value); }
        }

        public static readonly DependencyProperty TaskbarIconProperty = DependencyProperty.Register("TaskbarIcon", typeof(PackIconKind), typeof(RSView), new PropertyMetadata(PackIconKind.TestTube, SetTaskbarIconCallBack));
        private static void SetTaskbarIconCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is PackIconKind icon)) return;
            if (d != null)
            {
                ((RSView)d).SetTaskbarIcon(((RSView)d).TaskbarIcon);
            }
        }


        public PackIconKind TitlebarIcon
        {
            get { return (PackIconKind)GetValue(TitlebarIconProperty); }
            set { SetValue(TitlebarIconProperty, value); }
        }

        public static readonly DependencyProperty TitlebarIconProperty = DependencyProperty.Register("TitlebarIcon", typeof(PackIconKind), typeof(RSView), new PropertyMetadata(PackIconKind.TestTube, SetTitlebarIconCallBack));
        private static void SetTitlebarIconCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is PackIconKind icon)) return;
            if (d != null)
            {
                ((RSView)d).SetTitlebarIcon(((RSView)d).TitlebarIcon);
            }
        }



        public RSView()
        {
            this.SetResourceReference(Control.BackgroundProperty, "MaterialDesignPaper");
            this.SetResourceReference(FontFamilyProperty, "materialDesign:MaterialDesignFont");
            this.SetResourceReference(TextElement.ForegroundProperty, "MaterialDesignBody");
            //Background = (Brush)FindResource("MaterialDesignPaper");
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            MinWidth = 10;

            WindowChrome.SetWindowChrome(this, new WindowChrome() { CaptionHeight = 1, CornerRadius = new CornerRadius(0, 0, 0, 0), GlassFrameThickness = new Thickness(6, 6, 6, 6), ResizeBorderThickness = new Thickness(6, 6, 6, 6) });
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            StateChanged += RSWindow_StateChanged;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 2/* SystemParameters.WorkArea.Size.Height*//* + 12*//* + 2*/;  //This makes the window no go underneath the bottom taskbar 12 is 6 + 6 with borderthickness. 2 is one pixel up and one pixel down to go underneath edge.
            Titlebar = new TitlebarUserCtrl();
            base.Content = Titlebar;
            Titlebar.BoundWindow = this;
            RSWindow_StateChanged(null, null);
            UpdateLayout();

            JotService.Tracker.Track(this);
        }

        public new void Show()
        {
            var theStateThatWasActuallySet = WindowState;
            WindowState = WindowState.Normal;  //We should always start with normal, otherwise the border when maximised become f***ed up
            base.Show();
            WindowState = theStateThatWasActuallySet;
        }

        public bool CheckBeforeClose
        {
            get { return (bool)GetValue(CheckBeforeCloseProperty); }
            set { SetValue(CheckBeforeCloseProperty, value); }
        }
        public static readonly DependencyProperty CheckBeforeCloseProperty = DependencyProperty.Register("CheckBeforeClose", typeof(bool), typeof(RSView), new PropertyMetadata(false, (d, e) => { if (e.NewValue is bool b) ((RSView)d).Titlebar.CheckBeforeClose = b; }));
        public bool EnableClosable
        {
            get { return (bool)GetValue(EnableClosableProperty); }
            set { SetValue(EnableClosableProperty, value); }
        }
        public static readonly DependencyProperty EnableClosableProperty = DependencyProperty.Register("EnableClosable", typeof(bool), typeof(RSView), new PropertyMetadata(true, (d, e) => { if (e.NewValue is bool b) ((RSView)d).Titlebar.EnableClosable = b; }));

        public bool EnableMaximize
        {
            get { return (bool)GetValue(EnableMaximizeProperty); }
            set { SetValue(EnableMaximizeProperty, value); }
        }
        public static readonly DependencyProperty EnableMaximizeProperty = DependencyProperty.Register("EnableMaximize", typeof(bool), typeof(RSView), new PropertyMetadata(true, (d, e) => { if (e.NewValue is bool b) ((RSView)d).Titlebar.EnableMaximize = b; }));
        public bool EnableMinimize
        {
            get { return (bool)GetValue(EnableMinimizeProperty); }
            set { SetValue(EnableMinimizeProperty, value); }
        }
        public static readonly DependencyProperty EnableMinimizeProperty = DependencyProperty.Register("EnableMinimize", typeof(bool), typeof(RSView), new PropertyMetadata(true, (d, e) => { if (e.NewValue is bool b) ((RSView)d).Titlebar.EnableMinimize = b; }));

        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(RSView), new PropertyMetadata(null, (d, e) => { ((RSView)d).Titlebar.Content = e.NewValue; }));



        private void RSWindow_StateChanged(object sender, EventArgs e)
        {
            //Checking the taskbar height
            APPBARDATA data = new APPBARDATA();
            data.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(data);
            SHAppBarMessage(ABM_GETTASKBARPOS, ref data);
            TaskBarHeigt = (data.rc.bottom - data.rc.top);
            var max = this.MaxHeight;
            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new Thickness(6.7, 6, 6.7, 6); //I don't understand but it's always half the taskbar height.
            }
            else
            {
                BorderThickness = new Thickness(0, 0, 0, 0);
            }
        }

        private const int ABM_GETTASKBARPOS = 5;

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(int msg, ref APPBARDATA data);

        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        private struct RECT
        {
            public int left, top, right, bottom;
        }

        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor",
    typeof(double), typeof(RSView),
    new PropertyMetadata(1.0, ZoomFactorPropertyChanged));

        public double ZoomFactor
        {
            get => (double)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }
        public double TaskBarHeigt { get; private set; }
        public TitlebarUserCtrl Titlebar { get; }

        private static void ZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is double zoomFactor)) return;
            ((RSView)d).Titlebar.LayoutTransform = new ScaleTransform(zoomFactor, zoomFactor);
            ((RSView)d).Titlebar.UpdateLayout();
        }

        public new string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static new readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(RSView), new PropertyMetadata("", SetTitle));
        private static void SetTitle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is string title)) return;
            ((RSView)d).Titlebar.Title = title;
            ((RSView)d).Title = title;
        }

        public new object DataContext
        {
            get { return (object)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataContext.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(RSView), new PropertyMetadata(default(object), DataContextChanged));

        private static new void DataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RSView)d).Titlebar.DataContext = e.NewValue;
        }

        public object GetDataContext()
        {
            return Titlebar.DataContext;
        }
        public override string ToString()
        {
            return Title;
        }

    }
}
