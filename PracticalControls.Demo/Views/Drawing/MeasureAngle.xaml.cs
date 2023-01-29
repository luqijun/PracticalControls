using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MeasureAngle.xaml
    /// </summary>
    public partial class MeasureAngle : UserControl
    {
        MeasureAngleViewModel _vm;
        public MeasureAngle()
        {
            InitializeComponent();

            _vm = new MeasureAngleViewModel();
            this.DataContext = _vm;
            this.angleMeasurer.SizeChanged += MeasureAngle_SizeChanged;
        }

        private void MeasureAngle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int imgWidth = 400;
            int imgHeight = 200;
            this.angleMeasurer.ScaleX = this.angleMeasurer.ActualWidth / imgWidth;
            this.angleMeasurer.ScaleY = this.angleMeasurer.ActualHeight / imgHeight;
        }

        private void MiReset_Click(object sender, RoutedEventArgs e)
        {
            _vm.InitData();
        }
    }

    public class MeasureAngleViewModel : ViewModelBase
    {
        private PointCollection _points;

        public PointCollection Points
        {
            get { return _points; }
            set { Set(ref _points, value); }
        }

        public MeasureAngleViewModel()
        {
            InitData();
        }

        public void InitData()
        {
            this.Points = new PointCollection();
            this.Points.Add(new Point(80, 100));
            this.Points.Add(new Point(200, 100));
            this.Points.Add(new Point(85, 40));
        }

    }
}
