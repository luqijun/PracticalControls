using GalaSoft.MvvmLight.Command;
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

        private RelayCommand<bool?> _refreshCollectionViewCommand;
        public RelayCommand<bool?> RefreshCollectionViewCommand =>
            _refreshCollectionViewCommand ?? (_refreshCollectionViewCommand = new RelayCommand<bool?>(ExecuteRefreshCollectionViewCommand));

        /// <summary>
        /// 刷新ICollectionView对象
        /// </summary>
        /// <param name="isExpanded"></param>
        private void ExecuteRefreshCollectionViewCommand(bool? isExpanded)
        {
            this.DataGridOwner.FinishingEditing();
            ICollectionView cv = this.DataGridOwner.ItemsSource as ICollectionView;
            if (cv != null)
                cv.Refresh();
        }

        public DataGridTreeColumn()
        {
            this.CellTemplate = Application.Current.FindResource("dgTreeColumnTemplate") as DataTemplate;

            this.CellEditingTemplate= Application.Current.FindResource("dgTreeColumnEditingTemplate") as DataTemplate;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateElement(cell, dataItem) as ContentPresenter;
            element.ApplyTemplate();

            //绑定显示内容
            TextBlock tbContent = (TextBlock)element.ContentTemplate.FindName("tbContent", element);
            tbContent?.SetBinding(TextBlock.TextProperty, Binding);

            //绑定收缩展开事件
            ToggleButton toggleButton = (ToggleButton)element.ContentTemplate.FindName("expander", element);
            Binding binding = new Binding("RefreshCollectionViewCommand") { Source = this };
            toggleButton?.SetBinding(ToggleButton.CommandProperty, binding);
            toggleButton?.SetBinding(ToggleButton.CommandParameterProperty, new Binding("IsChecked") { Source = toggleButton });

            return element;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateEditingElement(cell, dataItem) as ContentPresenter;
            element.ApplyTemplate();

            //绑定显示内容
            TextBox tbContent = (TextBox)element.ContentTemplate.FindName("tbxContent", element);
            tbContent?.SetBinding(TextBox.TextProperty, Binding);

            return element;
        }
    }
}
