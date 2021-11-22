using GalaSoft.MvvmLight;
using PracticalControls.Models;
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
    /// ListTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class MultiRowTextBlock : UserControl
    {
        public MultiRowTextBlock()
        {
            InitializeComponent();
            this.DataContext = new MultiRowTextBlockViewModel();
        }
    }


    public class MultiRowTextBlockViewModel : ViewModelBase
    {
        private ObservableCollection<TextBlockRow> _lstTextRow;

        public ObservableCollection<TextBlockRow> LstTextRow
        {
            get { return _lstTextRow; }
            set { Set(ref _lstTextRow, value); }
        }

        public MultiRowTextBlockViewModel()
        {
            this.LstTextRow = new ObservableCollection<TextBlockRow>();
            this.LstTextRow.Add(new TextBlockRow("Header1","Both RichTextBox and TextBox allow users to edit text"));
            this.LstTextRow.Add(new TextBlockRow("Header2","Both RichTextBox and TextBox allow users to edit text"));
            this.LstTextRow.Add(new TextBlockRow("Header3","Both RichTextBox and TextBox allow users to edit text"));
            this.LstTextRow.Add(new TextBlockRow("Header4","Both RichTextBox and TextBox allow users to edit text"));
            this.LstTextRow.Add(new TextBlockRow("Header5", "Both RichTextBox and TextBox allow users to edit text"));
        }
    }
}
