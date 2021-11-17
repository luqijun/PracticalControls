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
    public class ExcelGridCollection<T> : ObservableCollection<T>
    {
        public ExcelDataGrid ExcelGrid { get; set; }

        #region 构造函数

        public ExcelGridCollection()
        {

        }

        public ExcelGridCollection(ExcelDataGrid excelgrid) : this(null, excelgrid)
        {

        }

        public ExcelGridCollection(IEnumerable<T> collection) : this(collection, null)
        {

        }

        public ExcelGridCollection(IEnumerable<T> collection, ExcelDataGrid excelgrid) : base(collection)
        {
            this.ExcelGrid = excelgrid;
        }

        #endregion

        #region 公共方法

        public void RefreshValue()
        {
            this.ExcelGrid.RefreshDataGridValue();
        }

        public void RefreshValue(int row, int col)
        {
            this.ExcelGrid.RefreshDataGridValue(row, col);
        }

        public void RefreshValue(int starRow, int starCol, int endRow, int endCol)
        {
            this.ExcelGrid.RefreshDataGridValue(starRow, starCol, endRow, endCol);
        }

        public void ChangeColumnsCount(int offset)
        {
            this.ExcelGrid.ChangeColumnsCount(offset);
        }
        #endregion
    }
}
