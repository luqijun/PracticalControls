using GalaSoft.MvvmLight;
using PracticalControls.Demo.Models;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// AddRows.xaml 的交互逻辑
    /// </summary>
    public partial class AddRows : UserControl
    {
        public AddRows()
        {
            InitializeComponent();

            this.DataContext = new AddRowsViewModel();
        }
    }

    public class AddRowsViewModel : ViewModelBase
    {


        private RangeObservableCollection<DataGridItem> _lstData;

        public RangeObservableCollection<DataGridItem> LstData
        {
            get { return _lstData; }
            set { Set(ref _lstData, value); }
        }

        public AddRowsViewModel()
        {
            this.LstData = new RangeObservableCollection<DataGridItem>();
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);

                    //List<DataGridItem> lst = new List<DataGridItem>();
                    //for (int i = 0; i < 5000; i++)
                    //{
                    //    lst.Add(new DataGridItem() { Name = "Name" + i });
                    //}
                    //this.LstData.AddRange(lst);
                }
            });

        }
    }
}
