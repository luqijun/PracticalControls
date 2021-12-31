using Microsoft.Xaml.Behaviors;
using PracticalControls.Adorners;
using PracticalControls.Enums;
using PracticalControls.Interfaces;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PracticalControls.Common.Helpers
{
    public class TreeViewHelper
    {
        #region 自定义SelectedItemProperty

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewHelper), new UIPropertyMetadata(new object(), SelectedItemChanged));

        private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is TreeView))
                return;

            var behavior = Interaction.GetBehaviors(obj).OfType<TreeViewSelectedItemBehavior>().FirstOrDefault();
            if (behavior == null)
            {
                behavior = new TreeViewSelectedItemBehavior(obj as TreeView);
                Interaction.GetBehaviors(obj).Add(behavior);
            }

            behavior.ChangeSelectedItem(e.NewValue);
        }

        public static object GetSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// 行为：SelectedItemBehavior
        /// </summary>
        class TreeViewSelectedItemBehavior : Behavior<TreeView>
        {
            TreeView view;
            public TreeViewSelectedItemBehavior(TreeView view)
            {
                this.view = view;
                view.SelectedItemChanged += (sender, e) => TreeViewHelper.SetSelectedItem(view, e.NewValue);
            }

            public void ChangeSelectedItem(object p)
            {
                TreeViewItem item = (TreeViewItem)view.ItemContainerGenerator.ContainerFromItem(p);
                if (item != null)
                    item.IsSelected = true;
            }
        }

        #endregion

        #region 是否可拖拽


        public static bool GetCanDragDropItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDragDropItemProperty);
        }

        public static void SetCanDragDropItem(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDragDropItemProperty, value);
        }

        public static readonly DependencyProperty CanDragDropItemProperty =
            DependencyProperty.RegisterAttached("CanDragDropItem", typeof(bool), typeof(TreeViewHelper), new PropertyMetadata(false, DragDropItemChangedCallback));

        private static void DragDropItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView tv = d as TreeView;
            if (tv == null)
                return;

            tv.PreviewMouseDown += Tv_PreviewMouseDown;
            tv.PreviewMouseLeftButtonUp += Tv_PreviewMouseLeftButtonUp;
            tv.MouseMove += Tv_MouseMove;

            tv.PreviewDragOver += Tv_PreviewDragOver;
            tv.PreviewDrop += Tv_PreviewDrop;
        }

        #endregion

        #region 是否正在拖拽

        public static bool GetIsDragging(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDraggingProperty);
        }

        public static void SetIsDragging(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDraggingProperty, value);
        }

        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(TreeViewHelper), new PropertyMetadata(false));

        #endregion

        #region 拖拽中显示

        static Point? _dragStartPosition = null;
        static DefaultTreeViewItemDragAdorner _dragAdorner = null;
        static AdornerLayer _adornerLayer = null;
        static List<TreeViewItem> _treeViewItems = new List<TreeViewItem>();

        static object _draggingObject;

        private static void Tv_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeView tv = sender as TreeView;
            var tvItem = UIHelper.FindAncestor<TreeViewItem>(e.OriginalSource as FrameworkElement);
            if (tvItem == null)
                return;

            ICanDragDrop item = tvItem.DataContext as ICanDragDrop;
            if (item == null || !item.CanDrag)
                return;

            if (!_treeViewItems.Contains(tvItem))
            {
                _treeViewItems.Add(tvItem);
                _draggingObject = tvItem.DataContext;
            }

            _dragStartPosition = NativeMethods.GetCursorPos();
        }
        private static void Tv_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeView tv = sender as TreeView;
            _treeViewItems.Clear();
            _dragStartPosition = null;
        }

        private static void Tv_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var viewElement = sender as FrameworkElement;
            if (_treeViewItems.Count == 0 || e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                return;

            //拖拽中
            TreeViewHelper.SetIsDragging(viewElement, true);
            _treeViewItems[0].SetCurrentValue(TreeViewItem.IsExpandedProperty, false);


            if (_adornerLayer == null || _dragAdorner == null)
            {
                _dragAdorner = new DefaultTreeViewItemDragAdorner(viewElement, _treeViewItems[0]) { Width = 100, Height = 40 };
                _adornerLayer = AdornerLayer.GetAdornerLayer(viewElement);
                _adornerLayer.Add(_dragAdorner);

                System.Diagnostics.Debug.WriteLine("2");
            }

            viewElement.GiveFeedback -= ViewElement_GiveFeedback;
            viewElement.GiveFeedback += ViewElement_GiveFeedback;
            try
            {
                DragDrop.DoDragDrop(viewElement, new DataObject("TreeViewDraggedData", _draggingObject), DragDropEffects.Move);
            }
            catch (Exception ex)
            {
            }

            viewElement.GiveFeedback -= ViewElement_GiveFeedback;

            _treeViewItems.Clear();
            _adornerLayer.Remove(_dragAdorner);
            _dragAdorner = null;
            _dragStartPosition = null;
            TreeViewHelper.SetIsDragging(viewElement, false);
        }

        private static void ViewElement_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (!_dragStartPosition.HasValue)
                return;

            //var viewElement = sender as FrameworkElement;
            var delta = NativeMethods.GetCursorPos() - _dragStartPosition.Value;
            if (Math.Abs(delta.X) < 1.0d || Math.Abs(delta.Y) < 1.0d)
                return;

            _dragAdorner.DragTranform = new TranslateTransform(delta.X, delta.Y);
            //Mouse.SetCursor(Cursors.Hand);
        }

        private static void Tv_PreviewDragOver(object sender, DragEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;

            ScrollViewer scrollViewer = UIHelper.FindDescendant<ScrollViewer>(element);
            if (scrollViewer == null)
                return;

            double tolerance = 60;
            double verticalPos = e.GetPosition(element).Y;
            double offset = 20;

            if (verticalPos < tolerance) // Top of visible list? 
            {
                //Scroll up
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
            }
            else if (verticalPos > element.ActualHeight - tolerance) //Bottom of visible list? 
            {
                //Scroll down
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
            }
        }

        #endregion

        #region 拖拽后处理

        private static void Tv_PreviewDrop(object sender, DragEventArgs e)
        {
            var action = TreeViewHelper.GetDragDropItemAction(sender as DependencyObject);
            var element = e.OriginalSource as FrameworkElement;
            ICanDragDrop targetItem = element.DataContext as ICanDragDrop;
            InsertMode mode = InsertMode.None;
            if (targetItem.CanDrop)
            {
                if (targetItem.CanDropToSameGrade && targetItem.CanDropToNextGrade)
                    mode = element.Name == "leftRect" ? InsertMode.SameGrade :
                                                         (element.Name == "rightRect" ? InsertMode.NextGrade : InsertMode.None);
                else if (targetItem.CanDropToSameGrade && element.Name == "leftRect")
                    mode = InsertMode.SameGrade;
                else if (targetItem.CanDropToNextGrade && element.Name == "rightRect")
                    mode = InsertMode.NextGrade;
            }

            if (mode != InsertMode.None)
                action?.Invoke(e.Data.GetData("TreeViewDraggedData"), element.DataContext, mode);
        }

        public static TreeViewItemDragDropAction GetDragDropItemAction(DependencyObject obj)
        {
            return (TreeViewItemDragDropAction)obj.GetValue(DragDropItemActionProperty);
        }

        public static void SetDragDropItemAction(DependencyObject obj, Action<object, object, object> value)
        {
            obj.SetValue(DragDropItemActionProperty, value);
        }

        public static readonly DependencyProperty DragDropItemActionProperty =
            DependencyProperty.RegisterAttached("DragDropItemAction", typeof(TreeViewItemDragDropAction), typeof(TreeViewHelper), new PropertyMetadata(null));
        
        #endregion

        #region 静态方法

        /// <summary>
        /// 收缩展开公共方法
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="isExpand"></param>
        public static void ShrinkOrExpandNodes<T>(IEnumerable<T> lstNodes, bool isExpand) where T : TreeViewItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                node.IsExpanded = isExpand;
                ShrinkOrExpandNodes(node.Children, isExpand);
            }
        }

        /// <summary>
        /// 获取满足条件的后代节点(单个根节点）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <param name="containself">是否包含自身</param>
        /// <returns></returns>
        public static List<T> GetDescendants<T>(T node, Func<T, bool> predicate, bool containself = true) where T : TreeViewItemBase<T>
        {
            return GetDescendants(new List<T> { node }, predicate, containself);
        }

        /// <summary>
        /// 获取满足条件的后代节点（多个根节点）
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="predicate"></param>
        /// <param name="containself">是否包含自身</param>
        /// <returns></returns>
        public static List<T> GetDescendants<T>(IEnumerable<T> lstNodes, Func<T, bool> predicate, bool containself = true) where T : TreeViewItemBase<T>
        {
            List<T> lstResults = new List<T>();
            foreach (var node in lstNodes)
            {
                if (containself)
                {
                    if (predicate(node))
                        lstResults.Add(node);
                }

                lstResults.AddRange(GetDescendants(node.Children, predicate));
            }
            return lstResults;
        }


        /// <summary>
        /// 递归处理节点
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="action"></param>
        /// <param name="containself"></param>
        public static void TraverseDescendants<T>(IEnumerable<T> lstNodes, Action<T> action, bool containself = true) where T : TreeViewItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                if (containself)
                {
                    action(node);
                }
                TraverseDescendants(node.Children, action);
            }
        }

        /// <summary>
        /// 递归获取节点
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="func"></param>
        /// <param name="containsTopLevelNode">是否处理顶级节点</param>
        public static TreeViewItemBase<T> GetNode<T>(IEnumerable<T> lstNodes, Func<T, bool> func, bool containsTopLevelNode = true) where T : TreeViewItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                if (containsTopLevelNode && func(node))
                    return node;

                var childNode = GetNode(node.Children, func);
                if (childNode != null)
                    return childNode;
            }
            return null;
        }

        /// <summary>
        /// 递归获取选中节点
        /// </summary>
        /// <param name="lstNodes"></param>
        public static TreeViewItemBase<T> GetSelectedNode<T>(IEnumerable<T> lstNodes) where T : TreeViewItemBase<T>
        {
            return GetNode(lstNodes, node => node.IsSelected);
        }


        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="curNode"></param>
        public static TreeViewItemBase<T> TraverseAncestors<T>(IEnumerable<T> lstNodes, TreeViewItemBase<T> curNode) where T : TreeViewItemBase<T>
        {
            if (curNode.Parent != null)
                return curNode.Parent;

            if (curNode.ParentId == null)
                return null;

            //通过Id查找
            return GetNode(lstNodes, node => node.Id == curNode.ParentId);
        }


        /// <summary>
        /// 递归设置父节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstNodes"></param>
        /// <param name="parent"></param>
        public static void SetParent<T>(IEnumerable<T> lstNodes, T parent = null) where T : TreeViewItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                node.Parent = parent;
                SetParent(node.Children, node);
            }
        }

        /// <summary>
        /// 设置父子关系
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstNodes"></param>
        /// <param name="topLevelNode">最顶层节点</param>
        public static IEnumerable<T> SetRelationShip<T>(IEnumerable<T> lstNodes, T topLevelNode = null) where T : TreeViewItemBase<T>
        {
            Dictionary<string, T> dicComponentInfo = new Dictionary<string, T>();
            foreach (var node in lstNodes)
            {
                if (!dicComponentInfo.ContainsKey(node.Id))
                    dicComponentInfo.Add(node.Id, node);

                //ParentId为空则为顶层节点子节点
                if (string.IsNullOrEmpty(node.ParentId))
                {
                    if (topLevelNode != null)
                    {
                        node.Parent = topLevelNode;
                        node.Parent.Children.Add(node);
                    }
                    continue;
                }

                //确保当前节点有父节点
                if (!dicComponentInfo.ContainsKey(node.ParentId))
                {
                    var parent = lstNodes.FirstOrDefault(o => o.Id == node.ParentId);
                    if (parent == null)
                        throw new Exception($"节点{node.Name}的父节点丢失！");
                    dicComponentInfo.Add(node.ParentId, parent);
                }

                node.Parent = dicComponentInfo[node.ParentId];
                node.Parent.Children.Add(node);
            }

            return lstNodes.Where(node => string.IsNullOrEmpty(node.ParentId));
        }

        public static void ClearSelection<T>(IEnumerable<T> lstNodes) where T : TreeViewItemBase<T>
        {
            var selNode = GetNode(lstNodes, node => node.IsSelected);
            if (selNode != null)
                selNode.IsSelected = false;
        }

        /// <summary>
        /// 递归遍历判断是否有满足指定条件的节点
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="func"></param>
        /// <param name="containsTopLevelNode">是否处理顶级节点</param>
        public static bool HasNode<T>(IEnumerable<T> lstNodes, Func<T, bool> func, bool containsTopLevelNode = true) where T : TreeViewItemBase<T>
        {
            return GetNode(lstNodes, func, containsTopLevelNode) != null;
        }

        /// <summary>
        /// 递归遍历判断是否有满足指定条件的节点
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="func"></param>
        /// <param name="containsTopLevelNode">是否处理顶级节点</param>
        public static bool HasNode<T>(T node, Func<T, bool> func, bool containsTopLevelNode = true) where T : TreeViewItemBase<T>
        {
            if (containsTopLevelNode && func(node))
                return true;

            return GetNode(node.Children, func, containsTopLevelNode) != null;
        }

        /// <summary>
        /// 重置数据源时 展开原来已经展开的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstNodes"></param>
        /// <param name="resetFunc">重置操作</param>
        public static void ResetSourceWithExpand<T>(IList<T> lstNodes, Func<IEnumerable<T>> resetFunc) where T : TreeViewItemBase<T>
        {

            if (lstNodes == null)
            {
                //重置数据源
                resetFunc.Invoke();
            }
            else
            {
                //获取原有展开信息
                List<T> lstExpandedNode = TreeViewHelper.GetDescendants(lstNodes, node => node.IsExpanded);

                //重置数据源
                var newSource = resetFunc.Invoke();

                //展开节点
                TreeViewHelper.TraverseDescendants(newSource, node =>
                {
                    if (lstExpandedNode.Any(n => n.Id == node.Id))
                        node.IsExpanded = true;
                });
            }
        }

        #endregion
    }
}
