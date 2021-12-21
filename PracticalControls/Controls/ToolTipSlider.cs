using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PracticalControls.Controls
{
    public class ToolTipSlider : Slider
    {
        private ToolTip _autoToolTip;

        public ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField(nameof(_autoToolTip), BindingFlags.NonPublic | BindingFlags.Instance);
                    _autoToolTip = field.GetValue(this) as ToolTip;
                }
                return _autoToolTip;
            }
        }

        /// <summary>
        /// AutoToolTip显示内容
        /// </summary>
        public string AutoToolTipContent
        {
            get { return (string)GetValue(AutoToolTipContentProperty); }
            set { SetValue(AutoToolTipContentProperty, value); }
        }

        public static readonly DependencyProperty AutoToolTipContentProperty =
            DependencyProperty.Register("AutoToolTipContent", typeof(string), typeof(ToolTipSlider), new PropertyMetadata(string.Empty));


        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            SetAutoToolTipContent();
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            SetAutoToolTipContent();
        }

        private void SetAutoToolTipContent()
        {
            this.AutoToolTip?.SetCurrentValue(ContentControl.ContentProperty, this.AutoToolTipContent);
        }
    }
}
