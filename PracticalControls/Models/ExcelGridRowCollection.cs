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


        #region 构造函数

        public ExcelGridRowCollection()
        {

        }

        public ExcelGridRowCollection(ExcelDataGrid excelgrid) : this(null, excelgrid)
        {

        }

        public ExcelGridRowCollection(IEnumerable<ExcelGridRow> collection) : this(collection, null)
        {

        }

        public ExcelGridRowCollection(IEnumerable<ExcelGridRow> collection, ExcelDataGrid excelgrid) : base(collection)
        {
            this.ExcelGrid = excelgrid;
        }

        #endregion

        #region 公共方法

        public void AddRemoveColumns(int offset)
        {
            this.ExcelGrid.AddRemoveColumns(offset);
        }
        #endregion
    }
}
