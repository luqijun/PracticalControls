using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PracticalControls.Models
{
    public class ExcelGridCell : ObservableObject, IDataErrorInfo
    {
        private object _value = string.Empty;

        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public CellValueType ValueType { get; set; }

        #region IDataErrorInfo接口实现

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(this.Value))
                {
                    //值为空时不验证
                    if (string.IsNullOrEmpty(this.Value?.ToString()))
                        return null;

                    if (!this.ValidateValueType(this.Value, this.ValueType))
                        return $"值必须为{this.ValueType.ToString()}类型!";
                }

                return "";
            }
        }

        #endregion


        public ExcelGridCell()
        {

        }

        public ExcelGridCell(object value, CellValueType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }


        public override string ToString()
        {
            return this.Value.ToString();
        }

        public void SetCellValueType(CellValueType valueType)
        {
            CellValueType oldValueType = this.ValueType;
            this.ValueType = valueType;
            this.Value = ConvertType(this.Value, oldValueType, valueType);
        }

        public object ConvertType(object value, CellValueType fromType, CellValueType toType)
        {
            object result = string.Empty;
            switch (toType)
            {
                case CellValueType.String:
                    result = value?.ToString();
                    break;
                case CellValueType.Int:
                    if (int.TryParse(value?.ToString(), out int i))
                        result = i;
                    break;
                case CellValueType.Float:
                    if (float.TryParse(value?.ToString(), out float f))
                        result = f;
                    break;
                case CellValueType.Double:
                    if (double.TryParse(value?.ToString(), out double d))
                        result = d;
                    break;
                default:
                    break;
            }
            return result;
        }


        public bool ValidateValueType(object value, CellValueType valueType)
        {
            switch (valueType)
            {
                case CellValueType.String:
                    return value is string;
                case CellValueType.Int:
                    return int.TryParse(value?.ToString(), out int i);
                case CellValueType.Float:
                    return float.TryParse(value?.ToString(), out float f);
                case CellValueType.Double:
                    return double.TryParse(value?.ToString(), out double d);
                default:
                    break;
            }

            return false;
        }
    }

    public enum CellValueType
    {
        String,
        Int,
        Float,
        Double
    }
}
