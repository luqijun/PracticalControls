using PracticalControls.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PracticalControls.Models
{
    public class ExcelGridRowCollection : ObservableCollection<ExcelGridRow>
    {
        public ExcelDataGrid ExcelGrid { get; set; }

        public ExcelGridCell this[int row, int col]
        {
            get
            {
                return this[row][col];
            }
            set
            {
                this[row][col] = value;
            }
        }

        private CellValueType _cellValueType;

        public CellValueType CellValueType
        {
            get
            {
                return _cellValueType;
            }
        }


        #region 构造函数

        public ExcelGridRowCollection()
        {

        }

        public ExcelGridRowCollection(ExcelDataGrid excelgrid, CellValueType valueType) : this(null, excelgrid, valueType)
        {

        }

        public ExcelGridRowCollection(IEnumerable<ExcelGridRow> collection, CellValueType valueType) : this(collection, null, valueType)
        {

        }

        public ExcelGridRowCollection(IEnumerable<ExcelGridRow> collection, ExcelDataGrid excelgrid, CellValueType valueType = CellValueType.String) : base(collection)
        {
            this.ExcelGrid = excelgrid;
            this._cellValueType = valueType;
        }

        #endregion

        #region 公共方法

        public void AddRemoveColumns(int offset)
        {
            this.ExcelGrid.AddRemoveColumns(offset);
        }

        public void SetCellValueType(CellValueType valueType)
        {
            _cellValueType = valueType;
            foreach (var row in this)
            {
                foreach (var cell in row.Cells)
                {
                    cell.SetCellValueType(valueType);
                }
            }
        }
        #endregion
    }
}
