using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PracticalControls.Controls;
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
    /// Interaction logic for Ruler.xaml
    /// </summary>
    public partial class MeasureLine : UserControl
    {
        LineMeasurerViewModel _vm;

        public MeasureLine()
        {
            InitializeComponent();

            _vm = new LineMeasurerViewModel();
            this.DataContext = _vm;
            this.SizeChanged += Ruler_SizeChanged;
        }

        private void Ruler_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int imgWidth = 400;
            int imgHeight = 200;
            this.lineMeasurer.ScaleX = this.lineMeasurer.ActualWidth / imgWidth;
            this.lineMeasurer.ScaleY = this.lineMeasurer.ActualHeight / imgHeight;
        }
    }

    public class LineMeasurerViewModel : ViewModelBase
    {
        private ObservableCollection<MeasureLineInfo> _lineInfos;

        public ObservableCollection<MeasureLineInfo> LineInfos
        {
            get { return _lineInfos; }
            set { Set(ref _lineInfos, value); }
        }

        public LineMeasurerViewModel()
        {
            this.LineInfos = new ObservableCollection<MeasureLineInfo>();

            MeasureLineInfo line;
            line = CreateLineInfo(10, 100, 100, 10);
            this.LineInfos.Add(line);

            line = CreateLineInfo(10, 10, 100, 100);
            this.LineInfos.Add(line);

            line = CreateLineInfo(10, 20, 100, 90);
            this.LineInfos.Add(line);
        }

        private MeasureLineInfo CreateLineInfo(double x1, double y1, double x2, double y2)
        {
            MeasureLineInfo line = new MeasureLineInfo();
            line.OriginStartPoint = new Point(x1, y1);
            line.OriginEndPoint = new Point(x2, y2);
            return line;
        }




        private RelayCommand _addCommand;
        public RelayCommand AddCommand =>
            (_addCommand ?? (_addCommand = new RelayCommand(ExecuteAddCommand)));

        private void ExecuteAddCommand()
        {
            MeasureLineInfo line;
            line = CreateLineInfo(20, 100, 100, 20);
            this.LineInfos.Add(line);

        }
    }
}
