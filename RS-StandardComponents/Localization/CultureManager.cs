using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace RS_StandardComponents
{
    public static class CultureManager
    {
        /// <summary>
        /// Sets the UICulture for the WPF application and raises the <see cref="UICultureChanged"/>
        /// event causing any XAML elements using the <see cref="ResxExtension"/> to automatically
        /// update
        /// </summary>
        public static CultureInfo UICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value != UICulture)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            Thread.CurrentThread.CurrentUICulture = value;
                            Thread.CurrentThread.CurrentCulture = value.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(value.Name) : value;

                            ResxExtension.MarkupManager.UpdateAllTargets();
                        }));
                }
            }
        }
        

    }
}