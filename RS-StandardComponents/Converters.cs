using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// This class implements like this 
// 
//  <RSConvertersWPF:BoolToBrushConverter
//  x:Key="HeartColorConverter"
//  FalseValue="Red"
//  TrueValue="DarkRed" />
//
//either in ResourceDictionary for the window or resource dictionary for the App.xaml for global access


namespace RS_StandardComponents
{

    public class VisualToImageSourceConverter : IValueConverter
    {
        public static ImageSource ConvertInCode(object icon)
        {
            if (icon is FrameworkElement visual)
            {
                visual.Measure(new Size(visual.ActualWidth, visual.ActualHeight));
                visual.Arrange(new Rect(0, 0, visual.ActualWidth, visual.ActualHeight));
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);
                return rtb;
            }
            return null;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FrameworkElement visual)
            {
                visual.Measure(new Size(visual.Width, visual.Height));
                visual.Arrange(new Rect(0, 0, visual.Width, visual.Height));
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)visual.Width, (int)visual.Height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);
                return rtb;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }
        public T NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                if (NullValue != null)
                    return NullValue;
                else
                    return FalseValue;
            }
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility>
    {
        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        } 
    }
    
    public class InvertBoolConverter : BoolToValueConverter<bool>
    {
        public InvertBoolConverter()
        {
            TrueValue = false;
            FalseValue = true;
        }
    }



    public class DecimalCommaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Replace(",", ".");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Replace(",", ".");
        }

    }

    public class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((DateTime)value == DateTime.MinValue)
                return string.Empty;
            else
                return ((DateTime)value).ToString((string)parameter);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
    public sealed class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }



        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Enum)value)?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// This converter does nothing except breaking the
    /// debugger into the convert method
    /// </summary>
    public class DatabindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }


    /// <summary>
    /// Use this class as a markup extension in wpf to write strings instead of bool values
    /// <TextBlock Text="{my:SwitchBinding MyBoolValue, Yes, No}" />
    /// Will print Yes or No. Works on all wpf-elements that accepts strings.
    /// Much more convenient and looks more nice than to write all string converters i app.xaml instead.
    /// </summary>
    public class SwitchBindingExtension : Binding
    {
        public SwitchBindingExtension()
        {
            Initialize();
        }

        public SwitchBindingExtension(string path) : base(path)
        {
            Initialize();
        }
        public SwitchBindingExtension(string path, object valueIfTrue, object valueIfFalse) : base(path)
        {
            Initialize();
            ValueIfTrue = valueIfTrue;
            ValueIfFalse = valueIfFalse;
        }

        private void Initialize()
        {
            ValueIfTrue = Binding.DoNothing;
            ValueIfFalse = Binding.DoNothing;
            Converter = new SwitchConverter(this);
        }

        [ConstructorArgument("valueIfTrue")]
        public object ValueIfTrue { get; set; }

        [ConstructorArgument("valueIfFalse")]
        public object ValueIfFalse { get; set; }

        private class SwitchConverter : IValueConverter
        {
            public SwitchConverter(SwitchBindingExtension switchExtension)
            {
                _switch = switchExtension;
            }

            private readonly SwitchBindingExtension _switch;

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    bool b = System.Convert.ToBoolean(value);
                    var trueValue = _switch.ValueIfTrue;
                    var falseValue = _switch.ValueIfFalse;
                    var resXTrue = trueValue as ResxExtension;
                    if (resXTrue != null) { trueValue = ResxExtension.GetValueManual(resXTrue.Key.ToString(), resXTrue.ResxName.ToString()); }
                    var resXFalse = falseValue as ResxExtension;
                    if (resXFalse != null) { falseValue = ResxExtension.GetValueManual(resXFalse.Key.ToString(), resXFalse.ResxName.ToString()); }
                    return b ? trueValue : falseValue;
                }
                catch
                {
                    return Binding.DoNothing;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return Binding.DoNothing;
            }
        }


    }
















}