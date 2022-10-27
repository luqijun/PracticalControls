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
    /// Interaction logic for SplintRing.xaml
    /// </summary>
    public partial class SplintRing : UserControl
    {
        public SplintRing()
        {
            InitializeComponent();
            this.DataContext = new SplintRingViewModel();
        }
    }

    public class SplintRingViewModel : ViewModelBase
    {
        private List<string> _textNums;

        public List<string> TextNums
        {
            get { return _textNums; }
            set { Set(ref _textNums, value); }
        }

        public SplintRingViewModel()
        {
            this.TextNums = new List<string>() { "1.23", "你好", "1.23", "你好", "1.23", "你好" };
        }
    }
}
