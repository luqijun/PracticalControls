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

namespace PracticalControls.Demo.Views
{
    /// <summary>
    /// DragTabControlDemo.xaml 的交互逻辑
    /// </summary>
    public partial class DragTabControlDemo : UserControl
    {
        public DragTabControlDemo()
        {
            InitializeComponent();
            this.DataContext = new DragTabControlDemoViewModel();
        }
    }

    public class DragTabControlDemoViewModel : ViewModelBase
    {
        private ObservableCollection<Data> _lstObject;

        public ObservableCollection<Data> LstObject
        {
            get { return _lstObject; }
            set { Set(ref _lstObject, value); }
        }

        public DragTabControlDemoViewModel()
        {
            _lstObject = new ObservableCollection<Data>();
            for (int i = 0; i < 5; i++)
            {
                string name = i.ToString();
                for (int j = 0; j < i; j++)
                {
                    name += i.ToString();
                }
                _lstObject.Add(new Data() { Name = $"Name{name}" });
            }
        }
    }

    public class Data : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

    }
}
