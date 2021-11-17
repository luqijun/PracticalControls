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

        private RelayCommand _addRowCommand;

        public RelayCommand AddRowCommand =>
            _addRowCommand ?? (_addRowCommand = new RelayCommand(ExcuteAddRowCommand));

        private void ExcuteAddRowCommand()
        {
            ExcelGridRow row = new ExcelGridRow(8);
            this.LstData.Insert(1, row);
            row[0].Value = "2";
            row[1].Value = "3";
        }

        private RelayCommand<object> _addColCommand;

        public RelayCommand<object> AddColCommand =>
            _addColCommand ?? (_addColCommand = new RelayCommand<object>(ExcuteAddColCommand));


        private void ExcuteAddColCommand(object obj)
        {
            this.LstData.AddRemoveColumns(1);

            this.LstData[0].Cells.LastOrDefault().Value = "123";
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
