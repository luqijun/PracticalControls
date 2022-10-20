using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZdMed.SharedUI.Helpers
{
    public static class StringFormatHelper
    {
        #region Value

        public static DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value", typeof(object), typeof(StringFormatHelper), new System.Windows.PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RefreshFormattedValue(obj);
        }

        public static object GetValue(DependencyObject obj)
        {
            return obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, object newValue)
        {
            obj.SetValue(ValueProperty, newValue);
        }

        #endregion

        #region Format

        public static DependencyProperty FormatProperty = DependencyProperty.RegisterAttached(
            "Format", typeof(string), typeof(StringFormatHelper), new System.Windows.PropertyMetadata(null, OnFormatChanged));

        private static void OnFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RefreshFormattedValue(obj);
        }

        public static string GetFormat(DependencyObject obj)
        {
            return (string)obj.GetValue(FormatProperty);
        }

        public static void SetFormat(DependencyObject obj, string newFormat)
        {
            obj.SetValue(FormatProperty, newFormat);
        }

        #endregion

        #region FormattedValue

        public static DependencyProperty FormattedValueProperty = DependencyProperty.RegisterAttached(
            "FormattedValue", typeof(string), typeof(StringFormatHelper), new System.Windows.PropertyMetadata(null));

        public static string GetFormattedValue(DependencyObject obj)
        {
            return (string)obj.GetValue(FormattedValueProperty);
        }

        public static void SetFormattedValue(DependencyObject obj, string newFormattedValue)
        {
            obj.SetValue(FormattedValueProperty, newFormattedValue);

            //TODO 临时处理 
            if (obj is TextBlock tb)
                tb.Text = newFormattedValue;
        }

        #endregion

        private static void RefreshFormattedValue(DependencyObject obj)
        {
            var value = GetValue(obj);
            var format = GetFormat(obj);

            if (format != null)
            {
                if (!format.StartsWith("{0:"))
                {
                    format = String.Format("{{0:{0}}}", format);
                }

                SetFormattedValue(obj, String.Format(format, value));
            }
            else
            {
                SetFormattedValue(obj, value == null ? String.Empty : value.ToString());
            }
        }
    }
}
