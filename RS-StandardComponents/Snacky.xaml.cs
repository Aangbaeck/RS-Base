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
    /// Interaction logic for Snacky.xaml
    /// </summary>
    public partial class Snacky
    {
        public Snacky()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(object), typeof(Snacky), new PropertyMetadata("Testing"/*default(object)*/, NotifyContentPropertyChanged));

        private static void NotifyContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && e != null)
            {
                ((Snacky)d).TheContentControl.Content = e.NewValue;
            }
        }

        public object Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(Snacky), new PropertyMetadata(default(bool), NotifyActivePropertyChanged));
        private static void NotifyActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool s)) { Console.WriteLine($"wrong datatype in {MethodBase.GetCurrentMethod()}"); return; }
            if (d != null)
            {
                if(s)
                {
                    DoubleAnimation animation = new DoubleAnimation(0,1, new TimeSpan(0,0,0,0,300));
                    ((Snacky)d).Root.BeginAnimation(StackPanel.TagProperty, animation);
                }
         //           < Storyboard x: Key = "ActivateStoryboard" Duration = "0:0:0.3" >
    
         //       < DoubleAnimation Storyboard.TargetName = "Root" Storyboard.TargetProperty = "Tag" From = "0" To = "1" Duration = "0:0:0.3" >
             
         //                    < DoubleAnimation.EasingFunction >
             
         //                        < SineEase EasingMode = "EaseOut" />
              
         //                     </ DoubleAnimation.EasingFunction >
              
         //                 </ DoubleAnimation >
              
         //                 < DoubleAnimation Storyboard.TargetName = "ContentPresenter" Storyboard.TargetProperty = "Opacity" To = "0" BeginTime = "0" Duration = "0" />
                       
         //                          < DoubleAnimation Storyboard.TargetName = "ContentPresenter" Storyboard.TargetProperty = "Opacity" From = "0" To = "1" BeginTime = "0:0:0.075"
         //                                    Duration = "0:0:0.225" >
         //       < DoubleAnimation.EasingFunction >
         //           < SineEase EasingMode = "EaseOut" />
 
         //        </ DoubleAnimation.EasingFunction >
 
         //    </ DoubleAnimation >
 
         //</ Storyboard >
 
                    else
                        { 
                        }
         //< Storyboard x: Key = "DeactivateStoryboard" Duration = "0:0:0.3" >
     
         //        < DoubleAnimation Storyboard.TargetName = "Root" Storyboard.TargetProperty = "Tag" From = "1" To = "0" Duration = "0:0:0.3" >
              
         //                     < DoubleAnimation.EasingFunction >
              
         //                         < SineEase EasingMode = "EaseOut" />
               
         //                      </ DoubleAnimation.EasingFunction >
               
         //                  </ DoubleAnimation >
               
         //              </ Storyboard >
            }
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        
    }
}
