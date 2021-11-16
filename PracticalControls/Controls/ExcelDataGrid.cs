using GalaSoft.MvvmLight.Command;
using PracticalControls.Common.Helpers;
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
        const int DefaultRowsCount = 5;
        const int DefaultColumnsCount = 8;
        const double DefaulgColumnWidth = 80.0;
        const string ColumnPrefix = "Column";

        #region 控件

        string PART_DG = "PART_DG";

        DataGrid _datagrid;

        #endregion

        ObservableCollection<ExpandoObject> _expandoObjects;
        public bool IsSaving { get; set; }

        public int RowsCount => _expandoObjects == null ? DefaultColumnsCount : _expandoObjects.Count;

        public int ColumnsCount
        {
            get
            {
                if (ItemsSource is IList<List<string>> lstData)
                    return lstData.Max(o => o.Count);
                return DefaultColumnsCount;
            }
        }

        #region 依赖属性

        /// <summary>
        /// 列的数量修改时通知刷新界面 正数：增加列  负数：删除列
        /// </summary>
        public int NotifyColumnsCount
        {
            get { return (int)GetValue(NotifyColumnsCountProperty); }
            set { SetValue(NotifyColumnsCountProperty, value); }
        }

        public static readonly DependencyProperty NotifyColumnsCountProperty =
            DependencyProperty.Register("NotifyColumnsCount", typeof(int), typeof(ExcelDataGrid), new PropertyMetadata(0, NotifyColumnsCountPropertyChanged));

        private static void NotifyColumnsCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExcelDataGrid excelGrid = d as ExcelDataGrid;
            int changedColumnsCount = (int)e.NewValue;
            if (changedColumnsCount != 0)
            {
                excelGrid.ChangeColumnsCount(changedColumnsCount);
                excelGrid.SetCurrentValue(NotifyColumnsCountProperty, 0);
            }
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                                        typeof(object), typeof(ExcelDataGrid),
                                        new FrameworkPropertyMetadata(new ObservableCollection<List<string>>(),
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
                excelgrid.SetDataGridValue(lstData);

                if (lstData is ObservableCollection<List<string>> collection)
                {
                    //Refresh
                    collection.CollectionChanged += (sender, e) =>
                    {
                        if (!excelgrid.IsSaving)
                        {
                            switch (e.Action)
                            {
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                    for (int i = 0; i < e.NewItems.Count; i++)
                                    {
                                        excelgrid.InsertRow(excelgrid.RowsCount);
                                    }
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                    for (int i = 0; i < e.NewItems.Count; i++)
                                    {
                                        excelgrid.DeleteRow(excelgrid.RowsCount);
                                    }
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                    excelgrid.Clear();
                                    break;
                                default:
                                    break;
                            }

                            excelgrid.SetDataGridValue(sender as ObservableCollection<List<string>>, false);
                        }
                    };
                }
            }
        }

        #endregion

        static ExcelDataGrid()
        {
            // Default Clipboard handling
            CommandManager.RegisterClassCommandBinding(typeof(DataGrid),
                                                       new CommandBinding(ApplicationCommands.Paste,
                                                       new ExecutedRoutedEventHandler(OnExecutedPaste),
                                                       new CanExecuteRoutedEventHandler(OnCanExecutedPaste)));
        }

        private static void OnCanExecutedPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            string dataText = Clipboard.GetText();
            e.CanExecute = !string.IsNullOrEmpty(dataText);
        }

        private static void OnExecutedPaste(object sender, ExecutedRoutedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            ExcelDataGrid excelGrid = dataGrid.TemplatedParent as ExcelDataGrid;

            int rowIdx = dataGrid.Items.IndexOf(dataGrid.SelectedCells[0].Item);
            int columnIdx = dataGrid.Columns.IndexOf(dataGrid.SelectedCells[0].Column);

            string dataText = Clipboard.GetText();
            if (dataText.EndsWith("\r\n"))
                dataText = dataText.Substring(0, dataText.LastIndexOf("\r\n"));
            string[] dataSplits = dataText.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            int maxCopyColumn = dataSplits.Max(o => o.Split("\t").Length);

            //赋值
            int oldColumnsCount = excelGrid.ColumnsCount;
            IList<List<string>> lstData = (excelGrid.ItemsSource as IList<List<string>>);
            for (int i = 0; i < dataSplits.Length; i++)
            {
                int row = rowIdx + i;
                if (lstData.Count <= row)
                    lstData.Add(new List<string>());

                string[] columnsData = dataSplits[i].Split("\t");
                for (int j = 0; j < columnsData.Length; j++)
                {
                    int col = columnIdx + j;
                    if (lstData[row].Count <= col)
                        lstData[row].Add(columnsData[j]);
                    else
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
            _datagrid.ItemsSource = _expandoObjects;
        }

        public void RefreshDataGridValue()
        {
            SetDataGridValue(this.ItemsSource as IList<List<string>>, false);
        }

        public void SetDataGridValue(IList<List<string>> lstData, bool needRecreate = true)
        {
            if (needRecreate)
                CreateDataGrid(lstData.Count == 0 ? DefaultRowsCount : lstData.Count,
                               lstData.Count == 0 ? DefaultColumnsCount : lstData.Max(o => o.Count));

            for (int i = 0; i < this.RowsCount; i++)
            {
                if (lstData.Count <= i)
                    lstData.Add(new List<string>());

                IDictionary<string, object> dic = _expandoObjects[i] as IDictionary<string, object>;
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    if (lstData[i].Count <= j)
                        lstData[i].Add("");

                    dic[$"{ColumnPrefix}{j}"] = lstData[i][j];
                }
            }
        }

        public void SaveDataGridValue()
        {
            this.IsSaving = true;

            IList<List<string>> lstData = this.ItemsSource as IList<List<string>>;
            for (int i = 0; i < this.RowsCount; i++)
            {
                if (lstData.Count <= i)
                    lstData.Add(new List<string>());

                IDictionary<string, object> dic = _expandoObjects[i] as IDictionary<string, object>;
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    dic.TryGetValue($"{ColumnPrefix}{j}", out object cellValue);

                    string value = cellValue == null ? "" : cellValue.ToString();
                    if (lstData[i].Count <= j)
                        lstData[i].Add(value);
                    else
                        lstData[i][j] = value;
                }
            }

            this.IsSaving = false;
        }

        #region 右键菜单

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
                DeleteRow(_expandoObjects.IndexOf(obj));
            }
            _datagrid.Items.Refresh();
        }

        private RelayCommand _deleteColCommand;

        public RelayCommand DeleteColCommand =>
            _deleteColCommand ?? (_deleteColCommand = new RelayCommand(ExcuteDeleteColCommand));

        private void ExcuteDeleteColCommand()
        {
            var lstColumns = _datagrid.SelectedCells.Select(o => o.Column).Distinct().ToList();
            foreach (var column in lstColumns)
            {
                DeleteCol(_datagrid.Columns.IndexOf(column));
            }
            RefreshColumnsHeader();
        }

        #endregion

        #region Methods

        private void InsertRow(int insertIndex)
        {
            dynamic newObj = new ExpandoObject();
            _expandoObjects.Insert(insertIndex, newObj);
            _datagrid.Items.Refresh();
        }

        private void InsertCol(int insertIndex)
        {
            DataGridTextColumn newCol = new DataGridTextColumn();
            newCol.Width = DefaulgColumnWidth;
            _datagrid.Columns.Insert(insertIndex, newCol);

            //右移
            foreach (var obj in _expandoObjects)
            {
                IDictionary<string, object> dic = obj as IDictionary<string, object>;
                for (int i = _datagrid.Columns.Count - 1; i > insertIndex; i--)
                {
                    dic[$"{ColumnPrefix}{i}"] = dic[$"{ColumnPrefix}{i - 1}"];
                }

                dic[$"{ColumnPrefix}{insertIndex}"] = "";
            }

            RefreshColumnsHeader(insertIndex);
        }

        private void DeleteRow(int index)
        {
            _expandoObjects.RemoveAt(index);
        }

        private void DeleteCol(int index)
        {
            _datagrid.Columns.RemoveAt(index);

            //左移
            foreach (var obj in _expandoObjects)
            {
                IDictionary<string, object> dic = obj as IDictionary<string, object>;
                for (int i = index; i < _datagrid.Columns.Count; i++)
                {
                    dic[$"{ColumnPrefix}{i}"] = dic[$"{ColumnPrefix}{i + 1}"];
                }
                dic.Remove($"{ColumnPrefix}{_datagrid.Columns.Count}");
            }
        }

        private void Clear()
        {
            _expandoObjects.Clear();
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

        private void ChangeColumnsCount(int changedColumnsCount)
        {
            if (changedColumnsCount != 0)
            {
                if (changedColumnsCount > 0)
                {
                    int insertIndex = this.ColumnsCount - changedColumnsCount;
                    for (int i = 0; i < changedColumnsCount; i++)
                    {
                        this.InsertCol(insertIndex + i);
                    }
                }
                else if (changedColumnsCount < 0)
                {
                    int deleteIndex = this.ColumnsCount + changedColumnsCount;
                    for (int i = 0; i < -changedColumnsCount; i++)
                    {
                        this.DeleteCol(deleteIndex - i);
                    }
                }
                this.RefreshColumnsHeader();
                this.RefreshDataGridValue();
            }
        }
        #endregion

        #endregion
    }
}
