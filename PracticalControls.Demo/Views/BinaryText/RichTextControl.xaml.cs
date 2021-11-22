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
    /// ListTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class RichTextControl : UserControl
    {
        public RichTextControl()
        {
            InitializeComponent();

            this.DataContext = new RichTextControlViewModel();
        }
    }


    public class RichTextControlViewModel : ViewModelBase
    {
        private ObservableCollection<TextModel> _lstText;

        public ObservableCollection<TextModel> LstText
        {
            get { return _lstText; }
            set { Set(ref _lstText, value); }
        }

        public RichTextControlViewModel()
        {
            this.LstText = new ObservableCollection<TextModel>();
            this.LstText.Add(new TextModel("Both RichTextBox and TextBox allow users to edit text"));
            this.LstText.Add(new TextModel("Both RichTextBox and TextBox allow users to edit text"));
            this.LstText.Add(new TextModel("Both RichTextBox and TextBox allow users to edit text"));
            this.LstText.Add(new TextModel("Both RichTextBox and TextBox allow users to edit text"));
            this.LstText.Add(new TextModel("Both RichTextBox and TextBox allow users to edit text"));
        }
    }

    public class TextModel
    {
        public string Text { get; set; }

        public TextModel(string text)
        {
            this.Text = text;
        }
    }
}
