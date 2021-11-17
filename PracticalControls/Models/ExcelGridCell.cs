using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public class ExcelGridCell : ObservableObject
    {
        private object _value = string.Empty;

        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public ExcelGridCell()
        {

        }

        public ExcelGridCell(object value)
        {
            this.Value = value;
        }


        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
