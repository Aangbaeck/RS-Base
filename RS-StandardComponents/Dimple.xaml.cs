using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RS_StandardComponents
{
    /// <summary>
    /// Interaction logic for Simple.xaml
    /// </summary>
    /// <summary>
    /// Implements a <see cref="Snackbar"/> inspired by the Material Design specs (https://material.google.com/components/snackbars-toasts.html).
    /// </summary>

    public partial class SimpleSnackbar : UserControl
    {
        private const string ActivateStoryboardName = "ActivateStoryboard";
        private const string DeactivateStoryboardName = "DeactivateStoryboard";



        public SimpleSnackbar()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleSnackbar), new FrameworkPropertyMetadata(typeof(SimpleSnackbar)));
            InitializeComponent();
            //OnApplyTemplate();
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(FrameworkElement), typeof(SimpleSnackbar), new PropertyMetadata(default(FrameworkElement), ApplyContent));

        private static void ApplyContent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //NetworkNode nn = (NetworkNode)d;
            //Ellipse el = nn.GetTemplateChild("PART_inner") as Ellipse;
            //if (el.PART_inner.Visibility == ...) < --exception el is null
                //if (!(e.NewValue is PackIconKind icon)) return;
            //    ((SimpleSnackbar)d).ContentPresenter.Content = new ContentPresenter();
            //((SimpleSnackbar)d).ContentPresenter.Content = e.NewValue;
        }

        
        public object Message
        {
            get => (FrameworkElement)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }


        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(SimpleSnackbar), new PropertyMetadata(default(bool), IsActivePropertyChangedCallback));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<bool> IsActiveChanged
        {
            add => AddHandler(IsActiveChangedEvent, value);
            remove => RemoveHandler(IsActiveChangedEvent, value);
        }

        public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(IsActiveChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(SimpleSnackbar));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SimpleSnackbar;
            var args = new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue)
            {
                RoutedEvent = IsActiveChangedEvent
            };
            instance?.RaiseEvent(args);
        }

        public static readonly RoutedEvent DeactivateStoryboardCompletedEvent = EventManager.RegisterRoutedEvent(
            nameof(DeactivateStoryboardCompleted), RoutingStrategy.Bubble, typeof(object), typeof(SimpleSnackbar));

        public event RoutedPropertyChangedEventHandler<object> DeactivateStoryboardCompleted
        {
            add => AddHandler(DeactivateStoryboardCompletedEvent, value);
            remove => RemoveHandler(DeactivateStoryboardCompletedEvent, value);
        }

        private static void OnDeactivateStoryboardCompleted(IInputElement snackbar, object message)
        {
            var args = new RoutedEventArgs(DeactivateStoryboardCompletedEvent, message);
            snackbar.RaiseEvent(args);
        }

        public TimeSpan ActivateStoryboardDuration { get; private set; }

        public TimeSpan DeactivateStoryboardDuration { get; private set; }



        public override void OnApplyTemplate()
        {
            //we regards to notification of deactivate storyboard finishing,
            //we either build a storyboard in code and subscribe to completed event, 
            //or take the not 100% proof of the storyboard duration from the storyboard itself
            //...HOWEVER...we can both methods result can work under the same public API so 
            //we can flip the implementation if this version does not pan out

            //(currently we have no even on the activate animation; don't 
            // need it just now, but it would mirror the deactivate)

            ActivateStoryboardDuration = GetStoryboardResourceDuration(ActivateStoryboardName);
            DeactivateStoryboardDuration = GetStoryboardResourceDuration(DeactivateStoryboardName);

            base.OnApplyTemplate();
        }

        private TimeSpan GetStoryboardResourceDuration(string resourceName)
        {
            var storyboard = Template.Resources.Contains(resourceName)
                ? (Storyboard)Template.Resources[resourceName]
                : null;

            return storyboard != null && storyboard.Duration.HasTimeSpan
                ? storyboard.Duration.TimeSpan
                : new Func<TimeSpan>(() =>
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Warning, no Duration was specified at root of storyboard '{resourceName}'.");
                    return TimeSpan.Zero;
                })();
        }

        private static void IsActivePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            OnIsActiveChanged(dependencyObject, dependencyPropertyChangedEventArgs);

            if ((bool)dependencyPropertyChangedEventArgs.NewValue) return;

            var snackbar = (SimpleSnackbar)dependencyObject;
            if (snackbar.Message is null) return;

            var dispatcherTimer = new DispatcherTimer
            {
                Tag = new Tuple<SimpleSnackbar, object>(snackbar, snackbar.Message),
                Interval = snackbar.DeactivateStoryboardDuration
            };
            dispatcherTimer.Tick += DeactivateStoryboardDispatcherTimerOnTick;
            dispatcherTimer.Start();
        }

        private static void DeactivateStoryboardDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (sender is DispatcherTimer dispatcherTimer)
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tick -= DeactivateStoryboardDispatcherTimerOnTick;
                var source = (Tuple<SimpleSnackbar, object>)dispatcherTimer.Tag;
                OnDeactivateStoryboardCompleted(source.Item1, source.Item2);
            }
        }

    }
}
