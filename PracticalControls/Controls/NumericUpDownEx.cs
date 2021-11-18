using PracticalControls.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PracticalControls.Controls
{
    public class NumericUpDownEx : MahApps.Metro.Controls.NumericUpDown, ICommandSource
    {
        private const string PART_NumericDown = "PART_NumericDown";
        private const string PART_NumericUp = "PART_NumericUp";

        private RepeatButton _repeatDown;
        private RepeatButton _repeatUp;

        public object UpParameter { get; set; }
        public object DownParameter { get; set; }

        #region ICommandSource

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(NumericUpDownEx), new PropertyMetadata(null));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(NumericUpDownEx), new PropertyMetadata(null));

        public IInputElement CommandTarget => throw new NotImplementedException();

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _repeatUp = this.GetTemplateChild(PART_NumericUp) as RepeatButton;
            _repeatDown = this.GetTemplateChild(PART_NumericDown) as RepeatButton;
            _repeatUp.Click += (sender, e) =>
            {
                this.CommandParameter = UpParameter;
                CommandHelper.ExecuteCommandSource(this);
            };

            _repeatDown.Click += (sender, e) =>
            {
                this.CommandParameter = DownParameter;
                CommandHelper.ExecuteCommandSource(this);
            };
        }

    }
}
