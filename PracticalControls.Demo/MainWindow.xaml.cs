using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PracticalControls.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();

            IList<B> lstB = new List<B>();
            var lstA = lstB as System.Collections.IList;
        }

        class A
        {

        }

        class B : A
        {

        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<object> _lstObject;

        public ObservableCollection<object> LstObject
        {
            get { return _lstObject; }
            set { Set(ref _lstObject, value); }
        }

        public MainWindowViewModel()
        {
            _lstObject = new ObservableCollection<object>();
            for (int i = 0; i < 5; i++)
            {
                _lstObject.Add(new Data() { Name = $"Name{i}" });
            }
        }
    }

    public class Data
    {
        public string Name { get; set; }
    }
}
