using GalaSoft.MvvmLight;
using PracticalControls.Demo.Models;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        //public ICollectionView LstDataView { get; set; }

        private ObservableCollection<DataGridItem> _lstData;

        public ObservableCollection<DataGridItem> LstData
        {
            get { return _lstData; }
            set { Set(ref _lstData, value); }
        }

        static int index = 0;


        public AddRowsViewModel()
        {
            this.LstData = new ObservableCollection<DataGridItem>();

#if false

            //this.LstDataView = CollectionViewSource.GetDefaultView(this.LstData);
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    System.Diagnostics.Debug.WriteLine(LstData.Count);

                    List<DataGridItem> lst = new List<DataGridItem>();
                    for (int i = 0; i < 5000; i++)
                    {
                        index++;
                        var item = new DataGridItem() { Name = "Name" + index, Type = "Type" + index };
                        lst.Add(item);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.LstData.Add(item);
                        });
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //lst.ForEach(o=> this.LstData.Add(o));
                        //(this.LstData as RangeObservableCollection<DataGridItem>).AddRange(lst);
                    });
                }
            });

#endif

        }
    }
}
