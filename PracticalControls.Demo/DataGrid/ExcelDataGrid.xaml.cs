using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

        private ObservableCollection<List<string>> _lstData;

        public ObservableCollection<List<string>> LstData
        {
            get { return _lstData; }
            set { Set(ref _lstData, value); }
        }


        public ExcelDataGridViewModel()
        {
            //Default Data
            ObservableCollection<List<string>> lstDefaultData = new ObservableCollection<List<string>>();
            for (int i = 0; i < 10; i++)
            {
                lstDefaultData.Add(new List<string>());
                for (int j = 0; j < 10; j++)
                {
                    lstDefaultData[i].Add(string.Empty);
                }
            }
            LstData = lstDefaultData;
        }

        private RelayCommand _addRowCommand;

        public RelayCommand AddRowCommand =>
            _addRowCommand ?? (_addRowCommand = new RelayCommand(ExcuteAddRowCommand));

        private void ExcuteAddRowCommand()
        {
            LstData.Insert(1, new List<string>() { "2", "3" });
        }

        private RelayCommand<object> _addColCommand;

        public RelayCommand<object> AddColCommand =>
            _addColCommand ?? (_addColCommand = new RelayCommand<object>(ExcuteAddColCommand));


        private void ExcuteAddColCommand(object obj)
        {

        }
    }
}
