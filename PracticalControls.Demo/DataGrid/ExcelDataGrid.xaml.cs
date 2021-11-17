using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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

namespace PracticalControls.Demo.DataGrid
{
    /// <summary>
    /// DynamicContent.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelDataGrid : UserControl
    {
        ExcelDataGridViewModel _vm;
        public ExcelDataGrid()
        {
            InitializeComponent();

            _vm = new ExcelDataGridViewModel();
            this.DataContext = _vm;
        }
    }

    public class ExcelDataGridViewModel : ViewModelBase
    {

        private ExcelGridRowCollection _lstData;

        public ExcelGridRowCollection LstData
        {
            get { return _lstData; }
            set { Set(ref _lstData, value); }
        }


        public ExcelDataGridViewModel()
        {
            //Default Data
            List<ExcelGridRow> lstDefaultData = new List<ExcelGridRow>();
            for (int i = 0; i < 10; i++)
            {
                ExcelGridRow row = new ExcelGridRow(8);
                lstDefaultData.Add(row);
            }
            LstData = new ExcelGridRowCollection(lstDefaultData);
        }

        private RelayCommand<string> _addRemoveRowCommand;

        public RelayCommand<string> AddRemoveRowCommand =>
            _addRemoveRowCommand ?? (_addRemoveRowCommand = new RelayCommand<string>(ExcuteAddRemoveRowCommand));

        private void ExcuteAddRemoveRowCommand(string tag)
        {
            switch (tag)
            {
                case "0":
                    if (this.LstData.Count > 0)
                        this.LstData.RemoveAt(this.LstData.Count - 1);
                    break;
                case "1":
                    ExcelGridRow row = new ExcelGridRow(this.LstData.ExcelGrid.ColumnsCount);
                    this.LstData.Add(row);
                    break;
                default:
                    break;
            }
        }

        private RelayCommand<string> _addRemoveColCommand;

        public RelayCommand<string> AddRemoveColCommand =>
            _addRemoveColCommand ?? (_addRemoveColCommand = new RelayCommand<string>(ExcuteAddRemoveColCommand));


        private void ExcuteAddRemoveColCommand(string tag)
        {
            int offset = tag == "0" ? -1 : 1;
            this.LstData.AddRemoveColumns(offset);
        }

        private RelayCommand _modifyValueCommand;

        public RelayCommand ModifyValueCommand =>
            _modifyValueCommand ?? (_modifyValueCommand = new RelayCommand(ExcuteModifyValueCommand));

        private void ExcuteModifyValueCommand()
        {
            this.LstData[3][2].Value = "32";
            this.LstData[0][0].Value = "00";

            this.LstData[this.LstData.Count - 1][this.LstData[0].Cells.Count - 1].Value = "NaN";
        }
    }
}
