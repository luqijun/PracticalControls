using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PracticalControls.Controls
{
    public class DragTabPanel : Panel
    {
        private int _itemCount;

        /// <summary>
        ///     是否可以更新
        /// </summary>
        internal bool CanUpdate = true;

        /// <summary>
        ///     选项卡字典
        /// </summary>
        internal Dictionary<int, DragTabItem> ItemDic = new Dictionary<int, DragTabItem>();

        /// <summary>
        ///     选项卡宽度
        /// </summary>
        internal Dictionary<int, double> ItemWidthDic = new Dictionary<int, double>();

        public static readonly DependencyPropertyKey FluidMoveDurationPropertyKey =
            DependencyProperty.RegisterReadOnly("FluidMoveDuration", typeof(Duration), typeof(DragTabPanel),
                new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(0))));

        public static readonly DependencyProperty FluidMoveDurationProperty =
            FluidMoveDurationPropertyKey.DependencyProperty;

        /// <summary>
        ///     流式行为持续时间
        /// </summary>
        public Duration FluidMoveDuration
        {
            get => (Duration)GetValue(FluidMoveDurationProperty);
            set => SetValue(FluidMoveDurationProperty, value);
        }

        public static readonly DependencyProperty IsTabFillEnabledProperty = DependencyProperty.Register(
            "IsTabFillEnabled", typeof(bool), typeof(DragTabPanel), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     是否将标签填充
        /// </summary>
        public bool IsTabFillEnabled
        {
            get => (bool)GetValue(IsTabFillEnabledProperty);
            set => SetValue(IsTabFillEnabledProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否自动设置Tab宽度
        /// </summary>
        public bool IsAutoTabWidth
        {
            get { return (bool)GetValue(IsAutoTabWidthProperty); }
            set { SetValue(IsAutoTabWidthProperty, value); }
        }

        public static readonly DependencyProperty IsAutoTabWidthProperty =
            DependencyProperty.Register("IsAutoTabWidth", typeof(bool), typeof(DragTabPanel), new PropertyMetadata(ValueBoxes.TrueBox));

        /// <summary>
        ///     标签宽度
        /// </summary>
        public static readonly DependencyProperty TabItemWidthProperty = DependencyProperty.Register(
            "TabItemWidth", typeof(double), typeof(DragTabPanel), new PropertyMetadata(200.0));

        /// <summary>
        ///     标签宽度
        /// </summary>
        public double TabItemWidth
        {
            get => (double)GetValue(TabItemWidthProperty);
            set => SetValue(TabItemWidthProperty, value);
        }

        /// <summary>
        ///     标签高度
        /// </summary>
        public static readonly DependencyProperty TabItemHeightProperty = DependencyProperty.Register(
            "TabItemHeight", typeof(double), typeof(DragTabPanel), new PropertyMetadata(30.0));

        /// <summary>
        ///     标签高度
        /// </summary>
        public double TabItemHeight
        {
            get => (double)GetValue(TabItemHeightProperty);
            set => SetValue(TabItemHeightProperty, value);
        }

        /// <summary>
        ///     是否可以强制更新
        /// </summary>
        internal bool ForceUpdate { get; set; }

        private Size _oldSize;

        /// <summary>
        ///     是否已经加载
        /// </summary>
        private bool _isLoaded;

        int _initMeasureCount = 0;

        protected override Size MeasureOverride(Size constraint)
        {
            _initMeasureCount++;
            if ((_itemCount == InternalChildren.Count || !CanUpdate) && !ForceUpdate && !IsTabFillEnabled)
                if (_initMeasureCount > 20)
                    return _oldSize;

            constraint.Height = TabItemHeight;
            _itemCount = InternalChildren.Count;

            var totalSize = new Size();

            ItemDic.Clear();
            ItemWidthDic.Clear();

            var count = InternalChildren.Count;
            if (count == 0)
            {
                _oldSize = new Size();
                return _oldSize;
            }
            constraint.Width += InternalChildren.Count;

            var itemWidth = .0;
            var arr = new int[count];

            if (!IsTabFillEnabled)
            {
                itemWidth = TabItemWidth;
            }
            else
            {
                if (TemplatedParent is DragTabControl tabControl)
                {
                    arr = ArithmeticHelper.DivideInt2Arr((int)tabControl.ActualWidth + InternalChildren.Count, count);
                }
            }

            for (var index = 0; index < count; index++)
            {
                if (IsTabFillEnabled)
                {
                    itemWidth = arr[index];
                }
                if (InternalChildren[index] is DragTabItem tabItem)
                {
                    if (IsAutoTabWidth)
                    {
                        tabItem.Measure(constraint);
                        itemWidth = tabItem.DesiredSize.Width;
                    }
                    else
                        tabItem.MaxWidth = itemWidth;

                    //var rect = new Rect
                    //{
                    //    X = totalSize.Width - tabItem.BorderThickness.Left,
                    //    Width = itemWidth,
                    //    Height = TabItemHeight
                    //};
                    //tabItem.Arrange(rect);

                    tabItem.RenderTransform = new TranslateTransform();
                    tabItem.ItemWidth = itemWidth;// itemWidth - tabItem.BorderThickness.Left;
                    tabItem.CurrentIndex = index;
                    tabItem.TargetOffsetX = 0;
                    ItemDic[index] = tabItem;
                    ItemWidthDic[index] = itemWidth;
                    totalSize.Width += itemWidth;
                }
            }
            totalSize.Height = constraint.Height;
            _oldSize = totalSize;
            return totalSize;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            double totalWidth = 0d;
            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                    continue;

                //Thickness margin = (Thickness)child.GetValue(MarginProperty);
                //double leftOffset = margin.Left;
                //double rightOffset = margin.Right;
                //double topOffset = margin.Top;
                //double bottomOffset = margin.Bottom;

                //Length left, top, right, bottom;
                Size cellSize = new Size(child.DesiredSize.Width, TabItemHeight);

                child.Arrange(new Rect(totalWidth, 0d, cellSize.Width, cellSize.Height));

                //Size childSize = cellSize;
                //childSize.Height = Math.Max(0d, childSize.Height - topOffset - bottomOffset);
                //childSize.Width = Math.Max(0d, childSize.Width - leftOffset - rightOffset);

                // Calculate the offset for the next child
                totalWidth += cellSize.Width;
            }

            return finalSize;
            //return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Override of <seealso cref="UIElement.GetLayoutClip"/>.
        /// </summary>
        /// <returns>Geometry to use as additional clip in case when element is larger then available space</returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }

        public DragTabPanel()
        {
            Loaded += (s, e) =>
            {
                if (_isLoaded) return;
                ForceUpdate = true;
                Measure(new Size(DesiredSize.Width, ActualHeight));
                ForceUpdate = false;
                foreach (var item in ItemDic.Values)
                {
                    item.TabPanel = this;
                }
                _isLoaded = true;
            };
        }

        /// <summary>
        /// 通过offset获取Items数量
        /// </summary>
        /// <param name="offsetX"></param>
        /// <returns></returns>
        internal int GetItemsCountFromOffset(double offsetX, bool isFromActualList = true)
        {
            int res = 0;
            double width = 0.0;

            if (isFromActualList)
            {
                DragTabControl tabControl = TemplatedParent as DragTabControl;
                var lst = tabControl?.GetActualList();
                if (lst != null)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var item = tabControl.ItemContainerGenerator.ContainerFromIndex(i) as DragTabItem;
                        if (item == null) continue;

                        if (width + item.ActualWidth > offsetX)
                        {
                            //向上取整
                            if (offsetX - width > item.ActualWidth / 2)
                                res++;
                            break;
                        }

                        width += item.ActualWidth;
                        res++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ItemWidthDic.Count; i++)
                {
                    if (width + ItemWidthDic[i] > offsetX)
                    {
                        //向上取整
                        if (offsetX - width > ItemWidthDic[i] / 2)
                            res++;
                        break;
                    }

                    width += ItemWidthDic[i];
                    res++;
                }
            }

            return res;
        }

        /// <summary>
        /// 获取像个索引间的tab跨度
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        internal double GetItemMoveDistance(int fromIndex, int toIndex)
        {
            double result = 0.0;

            if (fromIndex < toIndex)
            {
                for (int i = toIndex; i > fromIndex; i--)
                {
                    result += ItemDic[i].ActualWidth;
                }
            }
            else
            {
                for (int i = toIndex; i < fromIndex; i++)
                {
                    result -= ItemDic[i].ActualWidth;
                }
            }

            return result;
        }

        /// <summary>
        /// 不包含右边
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        internal double GetItemsWidth(int startIndex, int endIndex)
        {
            double result = 0.0;

            startIndex = Math.Max(startIndex, 0);
            endIndex = Math.Min(endIndex, _itemCount);

            for (int i = startIndex; i < endIndex; i++)
            {
                result += ItemDic[i].ActualWidth;
            }
            return result;
        }
    }


    /// <summary>
    ///     包含内部使用的一些简单算法
    /// </summary>
    internal class ArithmeticHelper
    {
        /// <summary>
        ///     平分一个整数到一个数组中
        /// </summary>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] DivideInt2Arr(int num, int count)
        {
            var arr = new int[count];
            var div = num / count;
            var rest = num % count;
            for (var i = 0; i < count; i++)
            {
                arr[i] = div;
            }
            for (var i = 0; i < rest; i++)
            {
                arr[i] += 1;
            }
            return arr;
        }

        /// <summary>
        ///     计算控件在窗口中的可见坐标
        /// </summary>
        public static Point CalSafePoint(FrameworkElement element, FrameworkElement showElement, Thickness thickness = default)
        {
            if (element == null || showElement == null) return default;
            var point = element.PointToScreen(new Point(0, 0));

            if (point.X < 0) point.X = 0;
            if (point.Y < 0) point.Y = 0;

            var maxLeft = SystemParameters.WorkArea.Width -
                          ((double.IsNaN(showElement.Width) ? showElement.ActualWidth : showElement.Width) +
                           thickness.Left + thickness.Right);
            var maxTop = SystemParameters.WorkArea.Height -
                         ((double.IsNaN(showElement.Height) ? showElement.ActualHeight : showElement.Height) +
                          thickness.Top + thickness.Bottom);
            return new Point(maxLeft > point.X ? point.X : maxLeft, maxTop > point.Y ? point.Y : maxTop);
        }

        /// <summary>
        ///     获取布局范围框
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Rect GetLayoutRect(FrameworkElement element)
        {
            var num1 = element.ActualWidth;
            var num2 = element.ActualHeight;
            if (element is Image || element is MediaElement)
                if (element.Parent is Canvas)
                {
                    num1 = double.IsNaN(element.Width) ? num1 : element.Width;
                    num2 = double.IsNaN(element.Height) ? num2 : element.Height;
                }
                else
                {
                    num1 = element.RenderSize.Width;
                    num2 = element.RenderSize.Height;
                }
            var width = element.Visibility == Visibility.Collapsed ? 0.0 : num1;
            var height = element.Visibility == Visibility.Collapsed ? 0.0 : num2;
            var margin = element.Margin;
            var layoutSlot = LayoutInformation.GetLayoutSlot(element);
            var x = 0.0;
            var y = 0.0;
            x = element.HorizontalAlignment switch
            {
                HorizontalAlignment.Left => layoutSlot.Left + margin.Left,
                HorizontalAlignment.Center => (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 -
                                              width / 2.0,
                HorizontalAlignment.Right => layoutSlot.Right - margin.Right - width,
                HorizontalAlignment.Stretch => Math.Max(layoutSlot.Left + margin.Left,
                    (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - width / 2.0),
                _ => x,
            };
            y = element.VerticalAlignment switch
            {
                VerticalAlignment.Top => layoutSlot.Top + margin.Top,
                VerticalAlignment.Center => (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 -
                                            height / 2.0,
                VerticalAlignment.Bottom => layoutSlot.Bottom - margin.Bottom - height,
                VerticalAlignment.Stretch => Math.Max(layoutSlot.Top + margin.Top,
                    (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - height / 2.0),
                _ => y
            };
            return new Rect(x, y, width, height);
        }

        /// <summary>
        ///     计算两点的连线和x轴的夹角
        /// </summary>
        /// <param name="center"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double CalAngle(Point center, Point p) => Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI;

        /// <summary>
        ///     计算法线
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3D CalNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
    }

}
