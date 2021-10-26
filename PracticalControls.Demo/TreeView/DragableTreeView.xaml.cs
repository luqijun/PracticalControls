using GalaSoft.MvvmLight;
using PracticalControls.Common;
using PracticalControls.Common.Helpers;
using PracticalControls.Enums;
using PracticalControls.Interfaces;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

namespace PracticalControls.Demo.TreeView
{
    /// <summary>
    /// DragableTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class DragableTreeView : UserControl
    {
        #region
        private bool IsDragging = false;

        private ObservableCollection<DragableTreeItem> list = new ObservableCollection<DragableTreeItem>();
        #endregion

        public DragableTreeView()
        {
            InitializeComponent();

            this.DataContext = new DragableTreeViewModel();
        }
    }

    public class DragableTreeViewModel : ViewModelBase
    {
        private bool _isDragging = true;

        public bool IsDragging
        {
            get { return _isDragging; }
            set { Set(ref _isDragging, value); }
        }

        private ObservableCollection<DragableTreeItem> _lstTreeItem;

        public ObservableCollection<DragableTreeItem> LstTreeItem
        {
            get { return _lstTreeItem; }
            set { Set(ref _lstTreeItem, value); }
        }

        private TreeViewItemDragDropAction _dragDropItemAction;

        public TreeViewItemDragDropAction DragDropItemAction
        {
            get { return _dragDropItemAction; }
            set { Set(ref _dragDropItemAction, value); }
        }



        public DragableTreeViewModel()
        {
            InitData();
        }

        private void InitData()
        {
            this.LstTreeItem = new ObservableCollection<DragableTreeItem>();

            DragableTreeItem ti = new DragableTreeItem();
            for (int i = 0; i < 3; i++)
            {
                DragableTreeItem ti0 = new DragableTreeItem() { Name = "item1" + i, CanDrop = false };
                this.LstTreeItem.Add(ti0);
                for (int j = 0; j < 4; j++)
                {
                    DragableTreeItem ti1 = new DragableTreeItem() { Name = "item2" + j };
                    ti0.Children.Add(ti1);
                    ti1.Parent = ti0;
                    for (int k = 0; k < 5; k++)
                    {
                        DragableTreeItem ti2 = new DragableTreeItem() { Name = "item3" + k, CanDrag = k != 3 };
                        ti1.Children.Add(ti2);
                        ti2.Parent = ti1;
                    }
                }
            }

            DragDropItemAction += (arg1, arg2, arg3) =>
            {
                HandleDragDrop(arg1 as DragableTreeItem, arg2 as DragableTreeItem, (InsertMode)arg3);
            };
        }

        /// <summary>
        /// 处理DragDrop
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="mode"></param>
        public void HandleDragDrop(DragableTreeItem source, DragableTreeItem target, InsertMode mode)
        {
            if (source == null || target == null || source.Id == target.Id)
                return;

            source = TreeViewHelper.GetNode(this.LstTreeItem, o => o.Id == source.Id) as DragableTreeItem;
            target = TreeViewHelper.GetNode(this.LstTreeItem, o => o.Id == target.Id) as DragableTreeItem;

            //移除source
            if (source.Parent == null)
                LstTreeItem.Remove(source);
            else
                source.Parent.Children.Remove(source);

            switch (mode)
            {
                case InsertMode.SameGrade:

                    //插入
                    if (target.Parent == null)
                    {
                        int insertIndex = this.LstTreeItem.IndexOf(target) + 1;
                        this.LstTreeItem.Insert(insertIndex, source);
                        source.Parent = null;
                    }
                    else
                    {
                        int insertIndex = target.Parent.Children.IndexOf(target) + 1;
                        target.Parent.Children.Insert(insertIndex, source);
                        source.Parent = target.Parent;
                    }

                    break;
                case InsertMode.NextGrade:

                    //插入
                    target.Children.Insert(0, source);
                    source.Parent = target;
                    break;
                default:
                    break;
            }
        }
    }

    public class DragableTreeItem : TreeViewItemBase<DragableTreeItem>, ICanDragDrop
    {
        #region IDragDrop接口实现

        private bool _canDrag = true;

        public bool CanDrag
        {
            get { return _canDrag; }
            set { Set(ref _canDrag, value); }
        }

        private bool _canDrop = true;

        public bool CanDrop
        {
            get { return _canDrop; }
            set { Set(ref _canDrop, value); }
        }

        #endregion

        static int id_count = 0;

        public DragableTreeItem()
        {
            this.IsExpanded = true;
            Id = id_count++.ToString();
        }
    }
}
