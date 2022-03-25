using PracticalControls.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PracticalControls.Common.Helpers
{
    public class ScrollViewerAttach
    {
        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.RegisterAttached(
            "AutoHide", typeof(bool), typeof(ScrollViewerAttach), new FrameworkPropertyMetadata(ValueBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetAutoHide(DependencyObject element, bool value)
            => element.SetValue(AutoHideProperty, ValueBoxes.BooleanBox(value));

        public static bool GetAutoHide(DependencyObject element)
            => (bool)element.GetValue(AutoHideProperty);

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached(
            "Orientation", typeof(Orientation), typeof(ScrollViewerAttach), new FrameworkPropertyMetadata((Orientation)ValueBoxes.VerticalBox, FrameworkPropertyMetadataOptions.Inherits, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Controls.ScrollViewerEx)
            {
                return;
            }

            if (d is System.Windows.Controls.ScrollViewer scrollViewer)
            {
                if ((Orientation)e.NewValue == Orientation.Horizontal)
                {
                    scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
                }
                else
                {
                    scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
                }
            }
        }

        private static void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (System.Windows.Controls.ScrollViewer)sender;
            scrollViewer.ScrollToHorizontalOffset(Math.Min(Math.Max(0, scrollViewer.HorizontalOffset - e.Delta), scrollViewer.ScrollableWidth));

            e.Handled = true;
        }

        public static void SetOrientation(DependencyObject element, Orientation value)
            => element.SetValue(OrientationProperty, ValueBoxes.OrientationBox(value));

        public static Orientation GetOrientation(DependencyObject element)
            => (Orientation)element.GetValue(OrientationProperty);
    }
}
