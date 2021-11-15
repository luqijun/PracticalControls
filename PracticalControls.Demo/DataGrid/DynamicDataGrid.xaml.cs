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
    public partial class DynamicDataGrid : UserControl
    {
        DynamicContentViewModel _vm;
        public DynamicDataGrid()
        {
            InitializeComponent();

            _vm = new DynamicContentViewModel();
            this.DataContext = _vm;
        }
    }

    public class DynamicContentViewModel : ViewModelBase
    {

        System.Windows.Controls.DataGrid _dataGrid;

        int _rowsCount = 5;
        int _columnsCount = 8;

        private ObservableCollection<ExpandoObject> _lstExpandedObject;

        public ObservableCollection<ExpandoObject> LstExpandedObject
        {
            get { return _lstExpandedObject; }
            set { Set(ref _lstExpandedObject, value); }
        }

        public DynamicContentViewModel()
        {
            _lstExpandedObject = new ObservableCollection<ExpandoObject>();
        }

        public void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _dataGrid = sender as System.Windows.Controls.DataGrid;

            InitDataGrid(_rowsCount, _columnsCount);
        }

        private void InitDataGrid(int rows, int cols)
        {
            for (int i = 0; i < rows; i++)
            {
                dynamic newObj = new ExpandoObject();
                this.LstExpandedObject.Add(newObj);
            }

            for (int i = 0; i < cols; i++)
            {
                DataGridTextColumn newCol = new DataGridTextColumn();

                Binding binding = new Binding("Column" + i);
                binding.Mode = BindingMode.TwoWay;
                newCol.Binding = binding;
                newCol.Width = 100;

                newCol.Header = NumberToSystem26(i);
                _dataGrid.Columns.Add(newCol);
            }
        }

        private RelayCommand _addRowCommand;

        public RelayCommand AddRowCommand =>
            _addRowCommand ?? (_addRowCommand = new RelayCommand(ExcuteAddRowCommand));

        private void ExcuteAddRowCommand()
        {
            dynamic newObj = new ExpandoObject();
            newObj.Column0 = "122";
            this.LstExpandedObject.Add(newObj);
            _rowsCount++;
        }

        private RelayCommand<object> _addColCommand;

        public RelayCommand<object> AddColCommand =>
            _addColCommand ?? (_addColCommand = new RelayCommand<object>(ExcuteAddColCommand));


        private void ExcuteAddColCommand(object obj)
        {
            DataGridTextColumn newCol = new DataGridTextColumn();

            Binding binding = new Binding("Column" + _columnsCount);
            binding.Mode = BindingMode.TwoWay;
            newCol.Binding = binding;

            newCol.Header = NumberToSystem26(_columnsCount);

            _dataGrid.Columns.Add(newCol);
            _columnsCount++;
        }

        private RelayCommand<object> _modifyCommand;

        public RelayCommand<object> ModifyCommand =>
            _modifyCommand ?? (_modifyCommand = new RelayCommand<object>(ExcuteModifyCommand));

        private void ExcuteModifyCommand(object obj)
        {
            (this.LstExpandedObject[0] as IDictionary<String, Object>)["Column0"] = "4567";
        }

        string NumberToSystem26(int n)
        {
            string s = string.Empty;
            n++;
            while (n > 0)
            {
                int m = n % 26;
                if (m == 0) m = 26;
                s = (char)(m + 64) + s;
                n = (n - m) / 26;
            }

            return s;
        }

        int System26ToNumber(string s)
        {
            int r = 0;
            for (int i = 0; i < s.Length; i++)
            {
                r = r * 26 + s[i] - 'A' + 1;
            }
            return r;
        }
    }
}
