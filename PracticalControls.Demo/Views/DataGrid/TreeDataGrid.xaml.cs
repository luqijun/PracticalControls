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

namespace PracticalControls.Demo.Views
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

        public ObservableCollection<DataGridItem> LstDataGridItem { get; set; }

        private ICollectionView _lstDataGridItemView;

        public ICollectionView LstDataGridItemView
        {
            get { return _lstDataGridItemView; }
            set { _lstDataGridItemView = value; }
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
                lstResult.Add(new DataGridItem("Grade" + i, "Type1", "v1", 0, true) { GroupName = "Group" + i, IsExpanded = true });
                for (int j = 0; j < 4; j++)
                {
                    lstResult.Add(new DataGridItem("Grade" + j, "Type1", "v1", 1, true) { GroupName = "Group" + i });
                    for (int k = 0; k < 1000; k++)
                    {
                        lstResult.Add(new DataGridItem("Grade" + k, "Type1", "v1", 2, false) { GroupName = "Group" + i });
                    }
                }
            }

            this.LstDataGridItem = new ObservableCollection<DataGridItem>(lstResult);
            TreeDataGridHelper.ResetRelationShip(this.LstDataGridItem);

            this.LstDataGridItemView = CollectionViewSource.GetDefaultView(this.LstDataGridItem);
            this.LstDataGridItemView.Filter = o => (o as TreeDataGridItemBase).IsVisible;

            //分组
            this.LstDataGridItemView.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
        }

        #region Event
        public void LstDataGridItem_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (this.LstDataGridItemView is ListCollectionView cv)
            {
                object currentAddItem = cv.CurrentAddItem;

                //延迟刷新
                DelayActionHelper.DoActionWhenMeetCondition("LstDataGridItem_CellEditEnding", () =>
                {
                    if (currentAddItem != null)
                    {
                        //不符合条件则移除
                        var newItem = cv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd ? this.LstDataGridItem.LastOrDefault() : this.LstDataGridItem.FirstOrDefault();
                        if (string.IsNullOrEmpty(this.LstDataGridItem.LastOrDefault().Name))
                            this.LstDataGridItem.Remove(newItem);

                        this.LstDataGridItemView.Refresh();
                    }

                }, () => !cv.IsAddingNew && !cv.IsEditingItem);
            }
        }
        #endregion
    }
}
