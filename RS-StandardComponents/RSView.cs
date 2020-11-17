using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
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
        public void SetIcon(PackIconKind icon)
        {
            Titlebar.Icon = icon;
            this.Icon = VisualToImageSourceConverter.ConvertInCode(new PackIcon() { Kind = icon, Width = 1000, Height = 1000, });
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
            WindowChrome.SetWindowChrome(this, new WindowChrome() { CaptionHeight = 1, CornerRadius = new CornerRadius(0, 0, 0, 0), GlassFrameThickness = new Thickness(0, 0, 0, 0), ResizeBorderThickness = new Thickness(6, 6, 6, 6) });
            
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            StateChanged += RSWindow_StateChanged;
            Titlebar = new TitlebarUserCtrl()
            {
                Title = "Untitled",

                CheckBeforeClose = false,
                EnableClosable = true,
                EnableMaximize = true,
                EnableMinimize = true,
                EnablePinMode = false,
                Icon = PackIconKind.Fire
            };
            base.Content = Titlebar;
            Titlebar.BoundWindow = this;
            RSWindow_StateChanged(null, null);
        }


        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(RSView), new PropertyMetadata(null, SetContentPropertyChanged));

        private static void SetContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RSView)d).Titlebar.Content = e.NewValue;
            //((RSView)d).Titlebar.BoundWindow = ((RSView)d);
            //((RSView)d).Titlebar.UpdateLayout();
        }

        private void RSWindow_StateChanged(object sender, EventArgs e)
        {
            //Checking the taskbar height
            APPBARDATA data = new APPBARDATA();
            data.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(data);
            SHAppBarMessage(ABM_GETTASKBARPOS, ref data);
            TaskBarHeigt = (data.rc.bottom - data.rc.top);

            if (WindowState == WindowState.Maximized)
            {
                MaxHeight = SystemParameters.WorkArea.Size.Height + 12 + 2;  //This makes the window no go underneath the bottom taskbar 12 is 6 + 6 with borderthickness. 2 is one pixel up and one pixel down to go underneath edge.
                BorderThickness = new Thickness(6, 6, 6, 6); //I don't understand but it's always half the taskbar height.
            }
            else
            {
                MaxHeight = SystemParameters.WorkArea.Size.Height;  //This makes the window no go underneath the bottom taskbar 12 is 6 + 6 with borderthickness. 2 is one pixel up and one pixel down to go underneath edge.
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

        public void SetContent(object userctrl)
        {
            Titlebar.Content = userctrl;
            Titlebar.BoundWindow = this;
        }

        public void SetTitle(string title)
        {
            Titlebar.Title = title;
            Title = title;
        }

        public void SetDataContext(object context)
        {
            Titlebar.DataContext = context;
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
