using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
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
    /// DynamicContent.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelDataGrid : UserControl
    {
        ExcelDataGridViewModel _vm;
        public ExcelDataGrid()
        {
            InitializeComponent();

            _vm = new ExcelDataGridViewModel();
            this.DataContext = _vm;
        }
    }

    public class ExcelDataGridViewModel : ViewModelBase
    {

        private ExcelGridRowCollection _lstData;

        public ExcelGridRowCollection LstData
        {
            get { return _lstData; }
            set { Set(ref _lstData, value); }
        }

        private CellValueType _selCellVauleType;

        public CellValueType SelCellValueType
        {
            get { return _selCellVauleType; }
            set
            {
                if (Set(ref _selCellVauleType, value))
                {
                    this.LstData.SetCellValueType(value);
                }
            }
        }

        private List<CellValueType> _lstCellValueType;

        public List<CellValueType> LstCellValueType
        {
            get { return _lstCellValueType; }
            set { Set(ref _lstCellValueType, value); }
        }


        public ExcelDataGridViewModel()
        {
            //Default Data
            List<ExcelGridRow> lstDefaultData = new List<ExcelGridRow>();
            for (int i = 0; i < 100000; i++)
            {
                ExcelGridRow row = new ExcelGridRow(8);
                lstDefaultData.Add(row);
            }
            LstData = new ExcelGridRowCollection(lstDefaultData);

            //Cell Types
            this.SelCellValueType = CellValueType.Double;
            this.LstCellValueType = new List<CellValueType>(System.Enum.GetValues<CellValueType>());
        }

        #region 增删改

        private RelayCommand<string> _addRemoveRowCommand;

        public RelayCommand<string> AddRemoveRowCommand =>
            _addRemoveRowCommand ?? (_addRemoveRowCommand = new RelayCommand<string>(ExcuteAddRemoveRowCommand));

        private void ExcuteAddRemoveRowCommand(string tag)
        {
            switch (tag)
            {
                case "0":
                    if (this.LstData.Count > 0)
                        this.LstData.RemoveAt(this.LstData.Count - 1);
                    break;
                case "1":
                    ExcelGridRow row = new ExcelGridRow(this.LstData.ExcelGrid.ColumnsCount, this.LstData.CellValueType);
                    this.LstData.Add(row);
                    break;
                default:
                    break;
            }
        }

        private RelayCommand<string> _addRemoveColCommand;

        public RelayCommand<string> AddRemoveColCommand =>
            _addRemoveColCommand ?? (_addRemoveColCommand = new RelayCommand<string>(ExcuteAddRemoveColCommand));


        private void ExcuteAddRemoveColCommand(string tag)
        {
            int offset = tag == "0" ? -1 : 1;
            this.LstData.AddRemoveColumns(offset);
        }

        private RelayCommand _modifyValueCommand;

        public RelayCommand ModifyValueCommand =>
            _modifyValueCommand ?? (_modifyValueCommand = new RelayCommand(ExcuteModifyValueCommand));

        private void ExcuteModifyValueCommand()
        {
            this.LstData[3][2].Value = "32";
            this.LstData[0][0].Value = "00";

            this.LstData[this.LstData.Count - 1][this.LstData[0].Cells.Count - 1].Value = "NaN";
        }

        #endregion

        #region 导入导出

        private RelayCommand _importDataCommand;

        public RelayCommand ImportDataCommand =>
            _importDataCommand ?? (_importDataCommand = new RelayCommand(ExcuteImportDataCommand));

        private void ExcuteImportDataCommand()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFile.Multiselect = false;
            openFile.ShowDialog();
            string filePath = openFile.FileName;
            bool fileIsUsing = false;
            FileStream fs = null;
            IWorkbook workbook = null;
            if (string.IsNullOrEmpty(filePath))
                return;
            try
            {
                fs = File.OpenRead(filePath);
                if (filePath.IndexOf(".xlsx") > 0)
                    workbook = new XSSFWorkbook(fs);
                else if (filePath.IndexOf(".xls") > 0)
                    workbook = new HSSFWorkbook(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("出错了,可能文件正被使用...:" + e.Message);
                fileIsUsing = true;
            }
            finally
            {

            }

            if (fileIsUsing)
                return;

            int sc = workbook.NumberOfSheets;
            if (sc == 1)
            {
                ISheet sheet = workbook.GetSheetAt(0);
                int maxRowCount = sheet.LastRowNum;
                int maxColumnCount = 0;
                for (int i = 0; i <= maxRowCount; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }

                    int cellNum = row.LastCellNum;
                    if (cellNum > maxColumnCount)
                    {
                        maxColumnCount = cellNum;
                    }
                }

                List<ExcelGridRow> lstNewRows = new List<ExcelGridRow>();
                for (int i = 0; i <= maxRowCount; i++)
                {
                    ExcelGridRow newRow = new ExcelGridRow(maxColumnCount);

                    IRow excelRow = sheet.GetRow(i);
                    if (excelRow != null)
                    {
                        for (int k = 0; k < excelRow.LastCellNum; k++)
                        {
                            ICell cell = sheet.GetRow(i).GetCell(k);
                            if (cell != null)
                            {
                                newRow[k].Value = cell.ToString();
                            }
                        }
                    }
                    lstNewRows.Add(newRow);
                }
                this.LstData = new ExcelGridRowCollection(lstNewRows);
            }
        }

        private RelayCommand _exportDataCommand;

        public RelayCommand ExportDataCommand =>
            _exportDataCommand ?? (_exportDataCommand = new RelayCommand(ExcuteExportDataCommand));

        private void ExcuteExportDataCommand()
        {
            string exportFileName = DateTime.Now.ToFileTime().ToString() + ".csv";

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出数据";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sfd.Filter = "csv文件| *.csv";
            sfd.FileName = exportFileName;
            sfd.ShowDialog();

            if (string.IsNullOrEmpty(sfd.FileName))
                return;


            StringBuilder fileData = new StringBuilder();
            for (int i = 0; i < this.LstData.Count; i++)
            {
                foreach (var cell in this.LstData[i].Cells)
                {
                    fileData.Append(cell.Value.ToString());
                    fileData.Append(",");
                }
                fileData = fileData.Remove(fileData.Length - 1, 1);
                fileData.Append("\n");
            }

            try
            {
                File.WriteAllText(sfd.FileName, fileData.ToString());
                MessageBox.Show("导出成功");
            }
            catch (Exception e)
            {
                MessageBox.Show("导出失败:" + e.Message);
            }
        }

        #endregion
    }
}
