using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public class ExcelGridRow
    {
        private ObservableCollection<ExcelGridCell> _cells = new ObservableCollection<ExcelGridCell>();

        public ObservableCollection<ExcelGridCell> Cells
        {
            get { return _cells; }
            set { _cells = value; }
        }

        public ExcelGridCell this[int index]
        {
            get
            {
                return _cells[index];
            }
            set
            {
                _cells[index] = value;
            }
        }

        public ExcelGridRow(int cellsCount, CellValueType valueType = CellValueType.String)
        {
            for (int i = 0; i < cellsCount; i++)
            {
                _cells.Add(new ExcelGridCell(string.Empty, valueType));
            }
        }
    }
}
