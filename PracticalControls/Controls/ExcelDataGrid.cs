using GalaSoft.MvvmLight.Command;
using PracticalControls.Common.Helpers;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PracticalControls.Controls
{
    [TemplatePart(Name = nameof(PART_DG), Type = typeof(DataGrid))]
    public class ExcelDataGrid : Control, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 接口实现

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        //const int DefaultRowsCount = 5;
        //const int DefaultColumnsCount = 8;
        const double DefaulgColumnWidth = 80.0;
        const string ColumnBindingPrefix = "Cells";

        #region 控件

        string PART_DG = "PART_DG";

        DataGrid _datagrid;

        #endregion

        public bool IsSaving { get; set; }

        public int RowsCount => this.ItemsSource == null ? 0 : this.ItemsSource.Count;

        public int ColumnsCount
        {
            get
            {
                if (this.ItemsSource is IList<ExcelGridRow> lstData && lstData.Count > 0)
                    return lstData.Max(o => o.Cells.Count);
                return 0;
            }
        }

        #region Constructor

        static ExcelDataGrid()
        {
            // Default Clipboard handling
            CommandManager.RegisterClassCommandBinding(typeof(DataGrid),
                                                       new CommandBinding(ApplicationCommands.Paste,
                                                       new ExecutedRoutedEventHandler(OnExecutedPaste),
                                                       new CanExecuteRoutedEventHandler(OnCanExecutedPaste)));
        }

        #endregion

        #region ClipBoard

        private static void OnCanExecutedPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            string dataText = Clipboard.GetText();
            e.CanExecute = !string.IsNullOrEmpty(dataText);
        }

        private static void OnExecutedPaste(object sender, ExecutedRoutedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            ExcelDataGrid excelGrid = dataGrid.TemplatedParent as ExcelDataGrid;

            int startRow = dataGrid.Items.IndexOf(dataGrid.SelectedCells[0].Item);
            int startCol = dataGrid.Columns.IndexOf(dataGrid.SelectedCells[0].Column);

            string dataText = Clipboard.GetText();
            if (dataText.EndsWith("\r\n"))
                dataText = dataText.Substring(0, dataText.LastIndexOf("\r\n"));
            string[] dataSplits = dataText.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            int maxCopyColumn = dataSplits.Max(o => o.Split("\t").Length);

            int oldColumnsCount = excelGrid.ColumnsCount;
            IList<ExcelGridRow> lstData = (excelGrid.ItemsSource as IList<ExcelGridRow>);

            //行列对齐
            int maxRowsCount = Math.Max(lstData.Count, startRow + dataSplits.Length);
            int maxColsCount = Math.Max(excelGrid.ColumnsCount, startCol + maxCopyColumn);
            excelGrid.AlignRowsColsCount(lstData, maxRowsCount, maxColsCount);

            //新增DataGrid列
            int changedCount = lstData.Max(o => o.Cells.Count) - oldColumnsCount;
            if (changedCount != 0)
                excelGrid.AddRemoveColumns(changedCount);

            //赋值
            for (int i = 0; i < dataSplits.Length; i++)
            {
                int row = startRow + i;
                string[] columnsData = dataSplits[i].Split("\t");
                for (int j = 0; j < columnsData.Length; j++)
                {
                    int col = startCol + j;
                    lstData[row][col].Value = columnsData[j];
                }
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _datagrid = this.GetTemplateChild(this.PART_DG) as DataGrid;
            if (_datagrid != null)
            {
                _datagrid.ContextMenu.DataContext = this;
                _datagrid.LoadingRow += Datagrid_LoadingRow;
            }

        }

        #region Events

        private void Datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        #endregion

        #region DependencyProperty

        public ExcelGridRowCollection ItemsSource
        {
            get { return (ExcelGridRowCollection)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                                        typeof(ExcelGridRowCollection), typeof(ExcelDataGrid),
                                        new FrameworkPropertyMetadata(new ExcelGridRowCollection(),
                                                                      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                      ItemsSourcePropertyChanged,
                                                                      null,
                                                                      false,
                                                                      UpdateSourceTrigger.PropertyChanged));

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExcelDataGrid excelgrid = d as ExcelDataGrid;
            IList<ExcelGridRow> lstData = e.NewValue as IList<ExcelGridRow>;
            if (lstData != null && !excelgrid.IsSaving)
            {
                if (lstData is ExcelGridRowCollection collection)
                {
                    collection.ExcelGrid = excelgrid;
                    collection.CollectionChanged += (sender, e) => excelgrid.OnPropertyChanged("RowsCount");
                    excelgrid.BindDataGrid(collection);
                }
            }
        }

        #endregion

        #region ContextMenu

        #region 添加

        private RelayCommand _addRowCommand;

        public RelayCommand AddRowCommand =>
            _addRowCommand ?? (_addRowCommand = new RelayCommand(ExcuteAddRowCommand));

        private void ExcuteAddRowCommand()
        {
            InsertRow(this.RowsCount);
        }

        private RelayCommand<object> _addColCommand;

        public RelayCommand<object> AddColCommand =>
            _addColCommand ?? (_addColCommand = new RelayCommand<object>(ExcuteAddColCommand));


        private void ExcuteAddColCommand(object obj)
        {
            InsertCol(this.ColumnsCount);
        }

        #endregion

        #region 插入

        private RelayCommand _insertRowCommand;

        public RelayCommand InsertRowCommand =>
            _insertRowCommand ?? (_insertRowCommand = new RelayCommand(ExcuteInsertRowCommand));

        private void ExcuteInsertRowCommand()
        {
            if (_datagrid.SelectedCells.Count == 0)
                return;

            ExcelGridRow row = _datagrid.SelectedCells.FirstOrDefault().Item as ExcelGridRow;
            InsertRow(this.ItemsSource.IndexOf(row));
        }

        private RelayCommand _insertColCommand;

        public RelayCommand InsertColCommand =>
            _insertColCommand ?? (_insertColCommand = new RelayCommand(ExcuteInsertColCommand));

        private void ExcuteInsertColCommand()
        {
            if (_datagrid.SelectedCells.Count == 0)
                return;

            DataGridColumn column = _datagrid.SelectedCells.FirstOrDefault().Column;
            InsertCol(_datagrid.Columns.IndexOf(column));
        }
        #endregion

        #region 删除

        private RelayCommand _deleteRowCommand;

        public RelayCommand DeleteRowCommand =>
            _deleteRowCommand ?? (_deleteRowCommand = new RelayCommand(ExcuteDeleteRowCommand));

        private void ExcuteDeleteRowCommand()
        {
            var lstRow = _datagrid.SelectedCells.Select(o => o.Item).Cast<ExcelGridRow>().Distinct().ToList();
            foreach (var row in lstRow)
            {
                DeleteRow(this.ItemsSource.IndexOf(row), false);
            }
            RefreshRowsHeader();
        }

        private RelayCommand _deleteColCommand;

        public RelayCommand DeleteColCommand =>
            _deleteColCommand ?? (_deleteColCommand = new RelayCommand(ExcuteDeleteColCommand));

        private void ExcuteDeleteColCommand()
        {
            var lstColumns = _datagrid.SelectedCells.Select(o => o.Column).Distinct().ToList();
            int minIndex = lstColumns.Min(o => _datagrid.Columns.IndexOf(o));
            foreach (var column in lstColumns)
            {
                DeleteCol(_datagrid.Columns.IndexOf(column), false);
            }
            RefreshColumnsHeader(minIndex);
        }

        #endregion

        #region Methods

        public void InsertRow(int insertIndex, bool needRefresh = true)
        {
            ExcelGridRow row = new ExcelGridRow(this.ColumnsCount);
            this.ItemsSource.Insert(insertIndex, row);
            if (needRefresh)
                RefreshRowsHeader();

            OnPropertyChanged("RowsCount");
        }

        public void InsertCol(int insertIndex, bool needRefresh = true)
        {
            DataGridTextColumn newCol = new DataGridTextColumn();
            newCol.Width = DefaulgColumnWidth;
            _datagrid.Columns.Insert(insertIndex, newCol);

            AlignRowsColsCount(this.ItemsSource, this.ItemsSource.Count, _datagrid.Columns.Count);

            //右移
            foreach (var row in this.ItemsSource)
            {
                for (int i = _datagrid.Columns.Count - 1; i > insertIndex; i--)
                {
                    row[i].Value = row[i - 1].Value;
                }

                row[insertIndex].Value = "";
            }

            if (needRefresh)
                RefreshColumnsHeader(insertIndex);

            OnPropertyChanged("ColumnsCount");
        }

        public void DeleteRow(int deleteIndex, bool needRefresh = true)
        {
            this.ItemsSource.RemoveAt(deleteIndex);
            if (needRefresh)
                RefreshRowsHeader();

            OnPropertyChanged("RowsCount");
        }

        public void DeleteCol(int deleteIndex, bool needRefresh = true)
        {
            _datagrid.Columns.RemoveAt(deleteIndex);

            //左移
            foreach (var row in this.ItemsSource)
            {
                for (int i = deleteIndex; i < _datagrid.Columns.Count; i++)
                {
                    row[i].Value = row[i + 1].Value;
                }
            }

            AlignRowsColsCount(this.ItemsSource, this.ItemsSource.Count, _datagrid.Columns.Count);

            if (needRefresh)
                RefreshColumnsHeader(deleteIndex);

            OnPropertyChanged("ColumnsCount");
        }

        public void Clear()
        {
            this.ItemsSource.Clear();
        }

        public void RefreshRowsHeader()
        {
            _datagrid.Items.Refresh();
        }

        public void RefreshColumnsHeader(int startIndex = 0)
        {
            int columnsCount = _datagrid.Columns.Count;
            for (int i = startIndex; i < columnsCount; i++)
            {
                _datagrid.Columns[i].Header = CommonHelper.Instance.NumberToSystem26(i);

                //重置binding
                Binding binding = new Binding(GetColumnBindingPath(i));
                binding.Mode = BindingMode.TwoWay;
                (_datagrid.Columns[i] as DataGridTextColumn).Binding = binding;
            }
        }

        private string GetColumnBindingPath(int colIndex)
        {
            return $"{ColumnBindingPrefix}[{colIndex}].Value";
        }

        #endregion

        #endregion

        #region Common Methods

        public void BindDataGrid(ExcelGridRowCollection itemsSource)
        {
            _datagrid.Columns.Clear();
            int columns = itemsSource.Max(row => row.Cells.Count);
            for (int i = 0; i < columns; i++)
            {
                DataGridTextColumn newCol = new DataGridTextColumn();

                Binding binding = new Binding(GetColumnBindingPath(i));
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                newCol.Binding = binding;
                newCol.Width = DefaulgColumnWidth;

                newCol.Header = CommonHelper.Instance.NumberToSystem26(i);
                _datagrid.Columns.Add(newCol);
            }

            _datagrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding { Source = this.ItemsSource });

            OnPropertyChanged("RowsCount");
            OnPropertyChanged("ColumnsCount");
        }

        public void AlignRowsColsCount(IList<ExcelGridRow> itemsSource, int maxRowsCount, int maxColsCount)
        {
            //行对齐
            if (itemsSource.Count < maxRowsCount)
            {
                while (itemsSource.Count < maxRowsCount)
                    itemsSource.Add(new ExcelGridRow(this.ColumnsCount));
            }
            else if (itemsSource.Count > maxRowsCount)
            {
                while (itemsSource.Count > maxRowsCount)
                    itemsSource.RemoveAt(itemsSource.Count - 1);
            }


            //列对齐
            foreach (var data in itemsSource)
            {
                if (data.Cells.Count < maxColsCount)
                {
                    while (data.Cells.Count < maxColsCount)
                        data.Cells.Add(new ExcelGridCell());
                }
                else if (data.Cells.Count > maxColsCount)
                {
                    while (data.Cells.Count > maxColsCount)
                        data.Cells.RemoveAt(data.Cells.Count - 1);
                }
            }
        }

        /// <summary>
        /// 列的数量修改时通知刷新界面 正数：增加列  负数：删除列
        /// </summary>
        /// <param name="offset"></param>
        public void AddRemoveColumns(int offset)
        {
            if (offset != 0)
            {
                if (offset > 0)
                {
                    int insertIndex = _datagrid.Columns.Count;
                    for (int i = 0; i < offset; i++)
                    {
                        this.InsertCol(insertIndex + i);
                    }
                }
                else if (offset < 0)
                {
                    int deleteIndex = _datagrid.Columns.Count - 1;
                    for (int i = 0; i < -offset; i++)
                    {
                        this.DeleteCol(deleteIndex - i);
                    }
                }
                this.RefreshColumnsHeader();
            }
        }
        #endregion
    }


    #region TestData

    public class Objj
    {
        public string Column0 { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }

        public List<Objj> GetTestData()
        {
            List<Objj> lst = new List<Objj>();
            for (int i = 0; i < 1000000; i++)
            {
                lst.Add(new Objj() { Column0 = "00", Column1 = "01", Column2 = "02", Column3 = "03", Column4 = "04", Column5 = "05", Column6 = "06" });
            }
            return lst;
        }

    }
    #endregion
}
