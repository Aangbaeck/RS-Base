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

        }
        public static readonly DependencyProperty BoundCurrentTitlebarProperty =
            DependencyProperty.Register("BoundTitlebar", typeof(TitlebarUserCtrl), typeof(Window),
                new PropertyMetadata(PropertyChanged));
        public TitlebarUserCtrl BoundTitlebar
        {
            get => (TitlebarUserCtrl)GetValue(BoundCurrentTitlebarProperty);
            set => SetValue(BoundCurrentTitlebarProperty, value);
        }
        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is TitlebarUserCtrl bar)) return;
            var win = ((RSWindow)d);
            win.BoundTitlebar = bar;            
        }
    }
}
