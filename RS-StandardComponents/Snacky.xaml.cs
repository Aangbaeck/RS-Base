using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS_StandardComponents
{
    /// <summary>
    /// Heavily inspired by material designs Snackbar but much simpler and without wierd positioning, padding and messagequeue. Can be easily extended too.
    /// </summary>
    public partial class Snacky
    {
        public Snacky()
        {
            InitializeComponent();
            RecalculateSize();
            NotifyActivePropertyChanged(this, new DependencyPropertyChangedEventArgs());
            //NotifyActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)


            }

        //Make the framework (re)calculate the size of the element. In the beginning the actual size is 0,0. This method remedies this.
        private void RecalculateSize()
        {
            Root.Measure(new Size(double.MaxValue, double.MaxValue));
            Size visualSize = Root.DesiredSize;
            Root.Arrange(new Rect(new Point(0, 0), visualSize));
            Root.UpdateLayout();
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(object), typeof(Snacky), new PropertyMetadata(default(object), NotifyMessagePropertyChanged));
        private static void NotifyMessagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d != null && e != null)
                {
                    ((Snacky)d).MessageControl.Content = e.NewValue;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NotifyMessagePropertyChanged error");
            }
        }
        public object Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(Snacky), new PropertyMetadata(false, NotifyActivePropertyChanged));
        private static void NotifyActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                Snacky snacky = d as Snacky;
                if (snacky == null) return;
                if (!(e.NewValue is bool s)) { snacky.Visibility = Visibility.Collapsed; return; }
                if (d != null)
                {
                    snacky.Visibility = Visibility.Visible;
                    var height = ((Snacky)d).Root.ActualHeight;

                    if (s)
                    {
                        DoubleAnimation animation = new DoubleAnimation(0, height, TimeSpan.FromSeconds(0.3));
                        SineEase easingFunction = new SineEase();
                        easingFunction.EasingMode = EasingMode.EaseOut;
                        animation.EasingFunction = easingFunction;
                        ((Snacky)d).Root.BeginAnimation(StackPanel.HeightProperty, animation);

                        DoubleAnimation animationContentPresenter = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.225));
                        animationContentPresenter.EasingFunction = easingFunction;
                        ((Snacky)d).MessageControl.BeginAnimation(StackPanel.OpacityProperty, animationContentPresenter);
                    }
                    else
                    {
                        DoubleAnimation animation = new DoubleAnimation(height, 0, TimeSpan.FromSeconds(0.3));
                        SineEase easingFunction = new SineEase();
                        easingFunction.EasingMode = EasingMode.EaseOut;
                        animation.EasingFunction = easingFunction;
                        ((Snacky)d).Root.BeginAnimation(StackPanel.HeightProperty, animation);
                    }
                }
            ((Snacky)d).UpdateLayout();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NotifyActivePropertyChanged error");
            }
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

    }
}
