using GalaSoft.MvvmLight.Command;
using PracticalControls.Common.Helpers;
using PracticalControls.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace PracticalControls.Controls
{
    /// <summary>
    /// 支持树结构的DataGridColumn
    /// </summary>
    public class DataGridTreeColumn : DataGridTemplateColumn
    {
        private BindingBase _binding;
        public virtual BindingBase Binding
        {
            get
            {
                return _binding;
            }
            set
            {
                if (_binding != value)
                {
                    BindingBase binding = _binding;
                    _binding = value;
                    CoerceValue(DataGridColumn.IsReadOnlyProperty);
                    CoerceValue(DataGridColumn.SortMemberPathProperty);
                    //OnBindingChanged(binding, _binding);
                }
            }
        }

        private bool _canEdit = true;
        /// <summary>
        /// 单元格是否可编辑
        /// </summary>
        public bool CanEdit
        {
            get { return _canEdit; }
            set { _canEdit = value; }
        }

        private string _addNewItemTip;
        /// <summary>
        /// 界面新行提示
        /// </summary>
        public string AddNewItemTip
        {
            get { return _addNewItemTip; }
            set { _addNewItemTip = value; }
        }

        /// <summary>
        /// 显示内容模板
        /// </summary>
        public DataTemplate ContentTemplate { get; set; }

        private RelayCommand<bool?> _refreshCollectionViewCommand;
        public RelayCommand<bool?> RefreshCollectionViewCommand =>
            _refreshCollectionViewCommand ?? (_refreshCollectionViewCommand = new RelayCommand<bool?>(ExecuteRefreshCollectionViewCommand));

        /// <summary>
        /// 刷新ICollectionView对象
        /// </summary>
        /// <param name="isExpanded"></param>
        private void ExecuteRefreshCollectionViewCommand(bool? isExpanded)
        {
            this.DataGridOwner.FinishEditing();
            ICollectionView cv = this.DataGridOwner.ItemsSource as ICollectionView;
            if (cv != null)
                cv.Refresh();
        }

        #region Constructor

        public DataGridTreeColumn()
        {

        }

        #endregion

        #region Element Generation

        List<object> editableItems = new List<object>();

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            SetCellTemplate();

            var element = base.GenerateElement(cell, dataItem) as ContentPresenter;
            element.ApplyTemplate();

            ContentPresenter content = (ContentPresenter)element.ContentTemplate.FindName("content", element);
            if (content != null)
            {
                if (this.ContentTemplate != null)
                    content.ContentTemplate = this.ContentTemplate;
                content.ApplyTemplate();

                //绑定显示内容
                TextBlock tbContent = (TextBlock)content.ContentTemplate.FindName("tbContent", content);
                if (tbContent != null)
                {
                    BindingExpression bindingExpr = tbContent.GetBindingExpression(TextBlock.TextProperty);
                    if (bindingExpr == null)
                        tbContent.SetBinding(TextBlock.TextProperty, Binding);
                }
            }

            //绑定收缩展开事件
            ToggleButton toggleButton = (ToggleButton)element.ContentTemplate.FindName("expander", element);
            Binding binding = new Binding("RefreshCollectionViewCommand") { Source = this };
            toggleButton?.SetBinding(ToggleButton.CommandProperty, binding);
            toggleButton?.SetBinding(ToggleButton.CommandParameterProperty, new Binding("IsChecked") { Source = toggleButton });

            //新增行提示
            TextBlock tbNew = (TextBlock)element.ContentTemplate.FindName("tbNew", element);
            tbNew.Text = this.AddNewItemTip;

            return element;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            SetCellEditingTemplate();

            var element = base.GenerateEditingElement(cell, dataItem) as ContentPresenter;

            //控制编辑
            DataGridRow row = UIHelper.FindAncestor<DataGridRow>(cell);
            if (row.IsNewItem && !editableItems.Contains(row.DataContext))
                editableItems.Add(row.DataContext);
            if (!CanEdit && !row.IsNewItem && !editableItems.Contains(row.DataContext))
            {
                this.DataGridOwner.FinishEditing();
                return null;
            }

            element.ApplyTemplate();

            //绑定显示内容
            TextBox tbContent = (TextBox)element.ContentTemplate.FindName("tbxContent", element);
            tbContent?.SetBinding(TextBox.TextProperty, Binding);

            return element;
        }

        #endregion

        #region Editing

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox textBox = UIHelper.FindDescendant<TextBox>(editingElement, null, true);
            if (textBox != null)
            {
                textBox.Focus();

                string originalValue = textBox.Text;

                TextCompositionEventArgs textArgs = editingEventArgs as TextCompositionEventArgs;
                if (textArgs != null)
                {
                    // If text input started the edit, then replace the text with what was typed.
                    string inputText = ConvertTextForEdit(textArgs.Text);
                    textBox.Text = inputText;

                    // Place the caret after the end of the text.
                    textBox.Select(inputText.Length, 0);
                }
                else
                {
                    // If a mouse click started the edit, then place the caret under the mouse.
                    MouseButtonEventArgs mouseArgs = editingEventArgs as MouseButtonEventArgs;
                    if ((mouseArgs == null) || !PlaceCaretOnTextBox(textBox, Mouse.GetPosition(textBox)))
                    {
                        // If the mouse isn't over the textbox or something else started the edit, then select the text.
                        textBox.SelectAll();
                    }
                }

                return originalValue;
            }

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        // convert text the user has typed into the appropriate string to enter into the editable TextBox
        string ConvertTextForEdit(string s)
        {
            // Backspace becomes the empty string
            if (s == "\b")
            {
                s = string.Empty;
            }

            return s;
        }

        private static bool PlaceCaretOnTextBox(TextBox textBox, Point position)
        {
            int characterIndex = textBox.GetCharacterIndexFromPoint(position, /* snapToText = */ false);
            if (characterIndex >= 0)
            {
                textBox.Select(characterIndex, 0);
                return true;
            }

            return false;
        }

        #endregion

        #region 若模板为空，则自动使用默认模板

        private void SetCellTemplate()
        {
            if (this.CellTemplate == null)
                this.CellTemplate = Application.Current.FindResource("dgTreeColumnTemplate") as DataTemplate;
        }

        private void SetCellEditingTemplate()
        {
            if (this.CellEditingTemplate == null)
                this.CellEditingTemplate = Application.Current.FindResource("dgTreeColumnEditingTemplate") as DataTemplate;
        }

        #endregion

    }
}
