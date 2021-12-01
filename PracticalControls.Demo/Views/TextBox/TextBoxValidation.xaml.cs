using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PracticalControls.Demo.Views
{
    /// <summary>
    /// TextBoxValidation.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxValidation : UserControl
    {
        public TextBoxValidation()
        {
            InitializeComponent();
            //DataGridRow row = new DataGridRow();
            //row.validat
            this.DataContext = new TextBoxValidationViewModel();
        }
    }


    public class TextBoxValidationViewModel : ViewModelBase, IDataErrorInfo
    {
        private int? _integerGreater10Property = 2;
        public int? IntegerGreater10Property
        {
            get => this._integerGreater10Property;
            set
            {
                if (this.IntegerGreater10Property == null)
                    _error = "值不能为空！";
                this.Set(ref this._integerGreater10Property, value);
            }
        }

        #region IDataErrorInfo接口实现

        private string _error = string.Empty;

        public string Error => _error;

        public string this[string columnName]
        {
            get
            {

                if (columnName == nameof(this.IntegerGreater10Property) && this.IntegerGreater10Property == null)
                    return "不能为空!";
                if (columnName == nameof(this.IntegerGreater10Property) && this.IntegerGreater10Property < 10)
                {
                    _error = "121212";
                    return "不能小于10!";
                }

                return null;
            }
        }
        #endregion
    }
}
