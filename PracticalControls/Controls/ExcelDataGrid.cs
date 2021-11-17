using GalaSoft.MvvmLight.Command;
using PracticalControls.Common.Helpers;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PracticalControls.Controls
{
    [TemplatePart(Name = nameof(PART_DG), Type = typeof(DataGrid))]
    public class ExcelDataGrid : Control
    {
        //const int DefaultRowsCount = 5;
        //const int DefaultColumnsCount = 8;
        const double DefaulgColumnWidth = 80.0;
        const string ColumnPrefix = "Column";

        #region 控件

        string PART_DG = "PART_DG";

        DataGrid _datagrid;

        #endregion

        ObservableCollection<ExpandoObject> _expandoObjects;
        public bool IsSaving { get; set; }

        public int RowsCount => _expandoObjects == null ? 0 : _expandoObjects.Count;

        public int ColumnsCount
        {
            get
            {
                if (ItemsSource is IList<List<string>> lstData)
                    return lstData.Max(o => o.Count);
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
            IList<List<string>> lstData = (excelGrid.ItemsSource as IList<List<string>>);

            //行列对齐
            int maxRowsCount = Math.Max(lstData.Count, startRow + dataSplits.Length);
            int maxColsCount = Math.Max(excelGrid.ColumnsCount, startCol + maxCopyColumn);
            excelGrid.AlignData(lstData, maxRowsCount, maxColsCount);

            //赋值
            for (int i = 0; i < dataSplits.Length; i++)
            {
                int row = startRow + i;
                string[] columnsData = dataSplits[i].Split("\t");
                for (int j = 0; j < columnsData.Length; j++)
                {
                    int col = startCol + j;
                    lstData[row][col] = columnsData[j];
                }
            }

            //通知刷新列
            int changedCount = lstData.Max(o => o.Count) - oldColumnsCount;
            if (changedCount != 0)
                excelGrid.ChangeColumnsCount(changedCount);
            else
                excelGrid.RefreshDataGridValue();
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _datagrid = this.GetTemplateChild(this.PART_DG) as DataGrid;
            _datagrid.ContextMenu.DataContext = this;
            _datagrid.LoadingRow += Datagrid_LoadingRow;
            _datagrid.CellEditEnding += Datagrid_CellEditEnding;
        }

        #region Events

        private void Datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void Datagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SaveDataGridValue();
        }

        #endregion

        #region DependencyProperty

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                                        typeof(object), typeof(ExcelDataGrid),
                                        new FrameworkPropertyMetadata(new ExcelGridCollection<List<string>>(),
                                                                      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                      ItemsSourcePropertyChanged,
                                                                      null,
                                                                      false,
                                                                      UpdateSourceTrigger.PropertyChanged));

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExcelDataGrid excelgrid = d as ExcelDataGrid;
            IList<List<string>> lstData = e.NewValue as IList<List<string>>;
            if (lstData != null && !excelgrid.IsSaving)
            {
                excelgrid.LoadDataGridValue(lstData);

                if (lstData is ExcelGridCollection<List<string>> collection)
                {
                    //Refresh
                    collection.ExcelGrid = excelgrid;
                    collection.CollectionChanged += (sender, e) =>
                    {
                        if (!excelgrid.IsSaving)
                        {
                            switch (e.Action)
                            {
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                    for (int i = 0; i < e.NewItems.Count; i++)
                                    {
                                        excelgrid.InsertRow(excelgrid.RowsCount, false);
                                    }
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                    for (int i = 0; i < e.NewItems.Count; i++)
                                    {
                                        excelgrid.DeleteRow(excelgrid.RowsCount, false);
                                    }
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                    excelgrid.Clear();
                                    break;
                                default:
                                    break;
                            }

                            excelgrid.RefreshRowsHeader();
                            excelgrid.LoadDataGridValue(sender as ExcelGridCollection<List<string>>, false);
                        }
                    };
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
            SaveDataGridValue();
        }

        private RelayCommand<object> _addColCommand;

        public RelayCommand<object> AddColCommand =>
            _addColCommand ?? (_addColCommand = new RelayCommand<object>(ExcuteAddColCommand));


        private void ExcuteAddColCommand(object obj)
        {
            InsertCol(this.ColumnsCount);
            SaveDataGridValue();
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

            ExpandoObject obj = _datagrid.SelectedCells.FirstOrDefault().Item as ExpandoObject;
            InsertRow(_expandoObjects.IndexOf(obj));
            SaveDataGridValue();
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
            SaveDataGridValue();
        }
        #endregion

        #region 删除

        private RelayCommand _deleteRowCommand;

        public RelayCommand DeleteRowCommand =>
            _deleteRowCommand ?? (_deleteRowCommand = new RelayCommand(ExcuteDeleteRowCommand));

        private void ExcuteDeleteRowCommand()
        {
            var lstObjects = _datagrid.SelectedCells.Select(o => o.Item).Cast<ExpandoObject>().Distinct().ToList();
            foreach (var obj in lstObjects)
            {
                DeleteRow(_expandoObjects.IndexOf(obj), false);
            }
            SaveDataGridValue();
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
            SaveDataGridValue();
            RefreshColumnsHeader(minIndex);
        }

        #endregion

        #region Methods

        private void InsertRow(int insertIndex, bool needRefresh = true)
        {
            dynamic newObj = new ExpandoObject();
            _expandoObjects.Insert(insertIndex, newObj);
            if (needRefresh)
                RefreshRowsHeader();
        }

        private void InsertCol(int insertIndex, bool needRefresh = true)
        {
            DataGridTextColumn newCol = new DataGridTextColumn();
            newCol.Width = DefaulgColumnWidth;
            _datagrid.Columns.Insert(insertIndex, newCol);

            //右移
            foreach (var obj in _expandoObjects)
            {
                IDictionary<string, object> dicObj = obj as IDictionary<string, object>;
                for (int i = _datagrid.Columns.Count - 1; i > insertIndex; i--)
                {
                    dicObj.TryGetValue($"{ColumnPrefix}{i - 1}", out object value);
                    dicObj[$"{ColumnPrefix}{i}"] = value ?? "";
                }

                dicObj[$"{ColumnPrefix}{insertIndex}"] = "";
            }

            if (needRefresh)
                RefreshColumnsHeader(insertIndex);
        }

        private void DeleteRow(int deleteIndex, bool needRefresh = true)
        {
            _expandoObjects.RemoveAt(deleteIndex);
            if (needRefresh)
                RefreshRowsHeader();
        }

        private void DeleteCol(int deleteIndex, bool needRefresh = true)
        {
            _datagrid.Columns.RemoveAt(deleteIndex);

            //左移
            foreach (var obj in _expandoObjects)
            {
                IDictionary<string, object> dicObj = obj as IDictionary<string, object>;
                for (int i = deleteIndex; i < _datagrid.Columns.Count; i++)
                {
                    dicObj.TryGetValue($"{ColumnPrefix}{i + 1}", out object value);
                    dicObj[$"{ColumnPrefix}{i}"] = value ?? "";
                }
                dicObj.Remove($"{ColumnPrefix}{_datagrid.Columns.Count}");
            }

            if (needRefresh)
                RefreshColumnsHeader(deleteIndex);
        }

        private void Clear()
        {
            _expandoObjects.Clear();
        }

        private void RefreshRowsHeader()
        {
            _datagrid.Items.Refresh();
        }

        private void RefreshColumnsHeader(int startIndex = 0)
        {
            int columnsCount = _datagrid.Columns.Count;
            for (int i = startIndex; i < columnsCount; i++)
            {
                _datagrid.Columns[i].Header = CommonHelper.Instance.NumberToSystem26(i);

                //重置binding
                Binding binding = new Binding(ColumnPrefix + i);
                binding.Mode = BindingMode.TwoWay;
                (_datagrid.Columns[i] as DataGridTextColumn).Binding = binding;
            }
        }

        #endregion

        #endregion

        #region Common Methods

        public void RefreshDataGridValue()
        {
            LoadDataGridValue(this.ItemsSource as IList<List<string>>, false);
        }

        public void RefreshDataGridValue(int row, int col)
        {
            LoadDataGridValue(this.ItemsSource as IList<List<string>>, row, col, row, col, false);
        }

        public void RefreshDataGridValue(int starRow, int starCol, int endRow, int endCol)
        {
            LoadDataGridValue(this.ItemsSource as IList<List<string>>, starRow, starCol, endRow, endCol, false);
        }

        public void LoadDataGridValue(IList<List<string>> lstData, bool needRecreate = true)
        {
            LoadDataGridValue(lstData, 0, 0, this.RowsCount - 1, this.ColumnsCount - 1, needRecreate);
        }

        public void LoadDataGridValue(IList<List<string>> lstData, int startRow, int startCol, int endRow, int endCol, bool needRecreate = true)
        {
            if (needRecreate)
                CreateDataGrid(lstData.Count, lstData.Max(o => o.Count));

            //行列对齐
            int maxRowsCount = Math.Max(lstData.Count, endRow + 1);
            int maxColsCount = Math.Max(this.ColumnsCount, endCol + 1);
            AlignData(lstData, maxRowsCount, maxColsCount);

            for (int i = startRow; i <= endRow; i++)
            {
                IDictionary<string, object> dicObj = _expandoObjects[i] as IDictionary<string, object>;
                for (int j = startCol; j <= endCol; j++)
                {
                    dicObj[$"{ColumnPrefix}{j}"] = lstData[i][j];
                }
            }
        }

        public void SaveDataGridValue()
        {
            this.IsSaving = true;

            IList<List<string>> lstData = this.ItemsSource as IList<List<string>>;

            //行列对齐
            AlignData(lstData, _expandoObjects.Count, _datagrid.Columns.Count);

            for (int i = 0; i < this.RowsCount; i++)
            {
                IDictionary<string, object> dic = _expandoObjects[i] as IDictionary<string, object>;
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    dic.TryGetValue($"{ColumnPrefix}{j}", out object cellValue);

                    string value = cellValue == null ? "" : cellValue.ToString();
                    lstData[i][j] = value;
                }
            }

            this.IsSaving = false;
        }

        public void CreateDataGrid(int rows, int cols)
        {
            List<ExpandoObject> lstObjects = new List<ExpandoObject>();
            for (int i = 0; i < rows; i++)
            {
                dynamic newObj = new ExpandoObject();
                lstObjects.Add(newObj);
            }

            _datagrid.Columns.Clear();
            for (int i = 0; i < cols; i++)
            {
                DataGridTextColumn newCol = new DataGridTextColumn();

                Binding binding = new Binding(ColumnPrefix + i);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                newCol.Binding = binding;
                newCol.Width = DefaulgColumnWidth;

                newCol.Header = CommonHelper.Instance.NumberToSystem26(i);
                _datagrid.Columns.Add(newCol);
            }

            _expandoObjects = new ObservableCollection<ExpandoObject>(lstObjects);
            _datagrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = _expandoObjects });
        }

        public void AlignData(IList<List<string>> lstData, int maxRowsCount, int maxColsCount)
        {
            //行对齐
            if (lstData.Count < maxRowsCount)
            {
                while (lstData.Count < maxRowsCount)
                    lstData.Add(new List<string>());
            }
            else if (lstData.Count > maxRowsCount)
            {
                while (lstData.Count > maxRowsCount)
                    lstData.RemoveAt(lstData.Count - 1);
            }


            //列对齐
            foreach (var data in lstData)
            {
                if (data.Count < maxColsCount)
                {
                    while (data.Count < maxColsCount)
                        data.Add("");
                }
                else
                {
                    while (data.Count > maxColsCount)
                        data.RemoveAt(data.Count - 1);
                }
            }
        }

        /// <summary>
        /// 列的数量修改时通知刷新界面 正数：增加列  负数：删除列
        /// </summary>
        /// <param name="offset"></param>
        public void ChangeColumnsCount(int offset)
        {
            if (offset != 0)
            {
                if (offset > 0)
                {
                    int insertIndex = this.ColumnsCount - offset;
                    for (int i = 0; i < offset; i++)
                    {
                        this.InsertCol(insertIndex + i);
                    }
                }
                else if (offset < 0)
                {
                    int deleteIndex = this.ColumnsCount + offset;
                    for (int i = 0; i < -offset; i++)
                    {
                        this.DeleteCol(deleteIndex - i);
                    }
                }
                this.RefreshColumnsHeader();
                this.RefreshDataGridValue();
            }
        }
        #endregion
    }
}
