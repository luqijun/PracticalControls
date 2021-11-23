using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PracticalControls.Common.Helpers;
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
        private byte _selNumType;

        public byte SelNumType
        {
            get { return _selNumType; }
            set { Set(ref _selNumType, value); }
        }


        private Dictionary<byte, string> _dicNumType;

        public Dictionary<byte, string> DicNumType
        {
            get { return _dicNumType; }
            set { Set(ref _dicNumType, value); }
        }


        private int _selColsCount;

        public int SelColsCount
        {
            get { return _selColsCount; }
            set { Set(ref _selColsCount, value); }
        }


        private List<int> _lstColsCount;

        public List<int> LstColsCount
        {
            get { return _lstColsCount; }
            set { Set(ref _lstColsCount, value); }
        }


        private ObservableCollection<TextBlockRow> _lstTextRow;

        public ObservableCollection<TextBlockRow> LstTextRow
        {
            get { return _lstTextRow; }
            set { Set(ref _lstTextRow, value); }
        }

        public List<byte> LstBytes { get; set; }


        public MultiRowTextBlockViewModel()
        {

            this.DicNumType = new Dictionary<byte, string>();
            this.DicNumType.Add(2, "二进制");
            this.DicNumType.Add(8, "八进制");
            this.DicNumType.Add(10, "十进制");
            this.DicNumType.Add(16, "十六进制");
            this.SelNumType = 10;

            this.LstColsCount = new List<int>();
            this.LstColsCount.Add(8);
            this.LstColsCount.Add(16);
            this.LstColsCount.Add(32);
            this.LstColsCount.Add(64);
            this.SelColsCount = 16;


            Random rd = new Random();
            this.LstBytes = new List<byte>();
            for (int i = 0; i < 1000; i++)
            {
                this.LstBytes.Add((byte)rd.Next(0, 255));
            }

            ExcuteReloadContentCommand();
        }

        private RelayCommand _reloadContentCommand;

        public RelayCommand ReloadContentCommand =>
            _reloadContentCommand ?? (_reloadContentCommand = new RelayCommand(ExcuteReloadContentCommand));

        private void ExcuteReloadContentCommand()
        {
            LoadContent(this.LstBytes.ToArray(), this.SelNumType, this.SelColsCount);
        }

        public void LoadContent(byte[] bytes, byte numType, int columnsCount)
        {
            List<TextBlockRow> lstRows = new List<TextBlockRow>();
            int singleNumLength = (numType == 8 || numType == 10) ? 3 : (numType == 2 ? 8 : 2);

            List<string> lstStr = new List<string>();
            for (int i = 0; i < bytes.Length; i++)
            {
                string res = CommonHelper.Instance.ConvertGenericBinary(bytes[i].ToString(), 10, numType);
                lstStr.Add(CommonHelper.Instance.LeftFill(res, '0', singleNumLength));

                if (lstStr.Count % 8 == 0 && this.SelColsCount >= 16 && lstStr.Count != columnsCount)
                    lstStr.Add("    ");

                if (lstStr.Count == columnsCount)
                {
                    string content = string.Join("  ", lstStr);
                    lstRows.Add(new TextBlockRow($"行号：{(i / columnsCount).ToString("D8")}", content));
                    lstStr.Clear();
                }
            }

            this.LstTextRow = new ObservableCollection<TextBlockRow>(lstRows);
        }

    }
}
