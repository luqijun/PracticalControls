using GalaSoft.MvvmLight;
using PracticalControls.Common.Helpers;
using PracticalControls.Demo.Models;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// TreeDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class TreeDataGrid : UserControl
    {
        public TreeDataGrid()
        {
            InitializeComponent();
            this.DataContext = new TreeDataGridViewModel();
        }
    }

    public class TreeDataGridViewModel : ViewModelBase
    {
        public ICollectionView LstDataGridItemView { get; set; }

        private ObservableCollection<DataGridItem> _lstDataGridItem;

        public ObservableCollection<DataGridItem> LstDataGridItem
        {
            get { return _lstDataGridItem; }
            set { Set(ref _lstDataGridItem, value); }
        }

        private DataGridItem _selDataGridItem;
        public DataGridItem SelDataGridItem
        {
            get { return _selDataGridItem; }
            set { Set(ref _selDataGridItem, value); }
        }

        public TreeDataGridViewModel()
        {
            Test();
        }

        public void Test()
        {
            List<DataGridItem> lstResult = new List<DataGridItem>();

            for (int i = 0; i < 3; i++)
            {
                lstResult.Add(new DataGridItem("Grade" + i, "Type1", "v1", 0, true) { GroupName = "Group" + i });
                for (int j = 0; j < 4; j++)
                {
                    lstResult.Add(new DataGridItem("Grade" + j, "Type1", "v1", 1, true));
                    for (int k = 0; k < 1000; k++)
                    {
                        lstResult.Add(new DataGridItem("Grade" + k, "Type1", "v1", 2, true));
                    }
                }
            }

            this.LstDataGridItem = new ObservableCollection<DataGridItem>(lstResult);
            TreeDataGridHelper.ResetRelationShip(this.LstDataGridItem);

            this.LstDataGridItemView = CollectionViewSource.GetDefaultView(this.LstDataGridItem);
            this.LstDataGridItemView.Filter = o => (o as TreeDataGridItemBase).IsVisible;
            this.LstDataGridItemView.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
        }
    }
}
