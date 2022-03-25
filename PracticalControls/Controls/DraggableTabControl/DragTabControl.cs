using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PracticalControls.Controls
{
    [TemplatePart(Name = OverflowButtonKey, Type = typeof(ContextMenuToggleButton))]
    [TemplatePart(Name = HeaderPanelKey, Type = typeof(DragTabPanel))]
    [TemplatePart(Name = OverflowScrollviewer, Type = typeof(ScrollViewerEx))]
    [TemplatePart(Name = ScrollButtonLeft, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ScrollButtonRight, Type = typeof(ButtonBase))]
    [TemplatePart(Name = HeaderBorder, Type = typeof(Border))]
    public class DragTabControl : TabControl
    {
        private const string OverflowButtonKey = "PART_OverflowButton";

        private const string HeaderPanelKey = "PART_HeaderPanel";

        private const string OverflowScrollviewer = "PART_OverflowScrollviewer";

        private const string ScrollButtonLeft = "PART_ScrollButtonLeft";

        private const string ScrollButtonRight = "PART_ScrollButtonRight";

        private const string HeaderBorder = "PART_HeaderBorder";

        private ContextMenuToggleButton _buttonOverflow;

        internal DragTabPanel HeaderPanel { get; private set; }

        private ScrollViewerEx _scrollViewerOverflow;

        private ButtonBase _buttonScrollLeft;

        private ButtonBase _buttonScrollRight;

        private Border _headerBorder;

        /// <summary>
        ///     是否为内部操作
        /// </summary>
        internal bool IsInternalAction { get; set; }

        /// <summary>
        ///     是否启用动画
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register(
            "IsAnimationEnabled", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     是否启用动画
        /// </summary>
        public bool IsAnimationEnabled
        {
            get => (bool)GetValue(IsAnimationEnabledProperty);
            set => SetValue(IsAnimationEnabledProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否可以拖动
        /// </summary>
        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register(
            "IsDraggable", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     是否可以拖动
        /// </summary>
        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否显示关闭按钮
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.RegisterAttached(
            "ShowCloseButton", typeof(bool), typeof(DragTabControl), new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowCloseButton(DependencyObject element, bool value)
            => element.SetValue(ShowCloseButtonProperty, ValueBoxes.BooleanBox(value));

        public static bool GetShowCloseButton(DependencyObject element)
            => (bool)element.GetValue(ShowCloseButtonProperty);

        /// <summary>
        ///     是否显示关闭按钮
        /// </summary>
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否显示上下文菜单
        /// </summary>
        public static readonly DependencyProperty ShowContextMenuProperty = DependencyProperty.RegisterAttached(
            "ShowContextMenu", typeof(bool), typeof(DragTabControl), new FrameworkPropertyMetadata(ValueBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowContextMenu(DependencyObject element, bool value)
            => element.SetValue(ShowContextMenuProperty, ValueBoxes.BooleanBox(value));

        public static bool GetShowContextMenu(DependencyObject element)
            => (bool)element.GetValue(ShowContextMenuProperty);

        /// <summary>
        ///     是否显示上下文菜单
        /// </summary>
        public bool ShowContextMenu
        {
            get => (bool)GetValue(ShowContextMenuProperty);
            set => SetValue(ShowContextMenuProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否将标签填充
        /// </summary>
        public static readonly DependencyProperty IsTabFillEnabledProperty = DependencyProperty.Register(
            "IsTabFillEnabled", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.FalseBox));

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
            DependencyProperty.Register("IsAutoTabWidth", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.TrueBox));

        /// <summary>
        ///     标签宽度
        /// </summary>
        public static readonly DependencyProperty TabItemWidthProperty = DependencyProperty.Register(
            "TabItemWidth", typeof(double), typeof(DragTabControl), new PropertyMetadata(200.0));

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
            "TabItemHeight", typeof(double), typeof(DragTabControl), new PropertyMetadata(30.0));

        /// <summary>
        ///     标签高度
        /// </summary>
        public double TabItemHeight
        {
            get => (double)GetValue(TabItemHeightProperty);
            set => SetValue(TabItemHeightProperty, value);
        }

        /// <summary>
        ///     是否可以滚动
        /// </summary>
        public static readonly DependencyProperty IsScrollableProperty = DependencyProperty.Register(
            "IsScrollable", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     是否可以滚动
        /// </summary>
        public bool IsScrollable
        {
            get => (bool)GetValue(IsScrollableProperty);
            set => SetValue(IsScrollableProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否显示溢出按钮
        /// </summary>
        public static readonly DependencyProperty ShowOverflowButtonProperty = DependencyProperty.Register(
            "ShowOverflowButton", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.TrueBox));

        /// <summary>
        ///     是否显示溢出按钮
        /// </summary>
        public bool ShowOverflowButton
        {
            get => (bool)GetValue(ShowOverflowButtonProperty);
            set => SetValue(ShowOverflowButtonProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     是否显示滚动按钮
        /// </summary>
        public static readonly DependencyProperty ShowScrollButtonProperty = DependencyProperty.Register(
            "ShowScrollButton", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     是否显示滚动按钮
        /// </summary>
        public bool ShowScrollButton
        {
            get => (bool)GetValue(ShowScrollButtonProperty);
            set => SetValue(ShowScrollButtonProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        ///     显示滚动按钮时，是否根据宽度自动隐藏
        /// </summary>
        public bool AutoHideScrollButton
        {
            get { return (bool)GetValue(AutoHideScrollButtonProperty); }
            set { SetValue(AutoHideScrollButtonProperty, value); }
        }

        public static readonly DependencyProperty AutoHideScrollButtonProperty =
            DependencyProperty.Register("AutoHideScrollButton", typeof(bool), typeof(DragTabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        ///     可见的标签数量
        /// </summary>
        private int _itemShowCount;

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (HeaderPanel == null)
            {
                IsInternalAction = false;
                return;
            }

            UpdateOverflowButton();

            if (IsInternalAction)
            {
                IsInternalAction = false;
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (var i = 0; i < Items.Count; i++)
                {
                    if (!(ItemContainerGenerator.ContainerFromIndex(i) is DragTabItem item)) return;
                    item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    item.TabPanel = HeaderPanel;
                }
            }

            _headerBorder?.InvalidateMeasure();
            IsInternalAction = false;
        }

        public override void OnApplyTemplate()
        {
            if (_buttonOverflow != null)
            {
                if (_buttonOverflow.Menu != null)
                {
                    _buttonOverflow.Menu.Closed -= Menu_Closed;
                    _buttonOverflow.Menu = null;
                }

                _buttonOverflow.Click -= ButtonOverflow_Click;
            }

            if (_buttonScrollLeft != null) _buttonScrollLeft.Click -= ButtonScrollLeft_Click;
            if (_buttonScrollRight != null) _buttonScrollRight.Click -= ButtonScrollRight_Click;

            base.OnApplyTemplate();
            HeaderPanel = GetTemplateChild(HeaderPanelKey) as DragTabPanel;

            if (IsTabFillEnabled) return;

            _buttonOverflow = GetTemplateChild(OverflowButtonKey) as ContextMenuToggleButton;
            _scrollViewerOverflow = GetTemplateChild(OverflowScrollviewer) as ScrollViewerEx;
            _buttonScrollLeft = GetTemplateChild(ScrollButtonLeft) as ButtonBase;
            _buttonScrollRight = GetTemplateChild(ScrollButtonRight) as ButtonBase;
            _headerBorder = GetTemplateChild(HeaderBorder) as Border;

            if (_buttonScrollLeft != null) _buttonScrollLeft.Click += ButtonScrollLeft_Click;
            if (_buttonScrollRight != null) _buttonScrollRight.Click += ButtonScrollRight_Click;

            if (_buttonOverflow != null)
            {
                var menu = new ContextMenu
                {
                    Placement = PlacementMode.Bottom,
                    PlacementTarget = _buttonOverflow
                };
                menu.Closed += Menu_Closed;
                _buttonOverflow.Menu = menu;
                _buttonOverflow.Click += ButtonOverflow_Click;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateOverflowButton();
        }

        internal void UpdateOverflowButton()
        {
            if (!IsTabFillEnabled)
            {
                _itemShowCount = GetItemShowCount();// (int)(ActualWidth / TabItemWidth);
                _buttonOverflow?.Show(ShowOverflowButton && Items.Count > 0 && Items.Count > _itemShowCount);

                //更新Scroll按钮状态
                if (AutoHideScrollButton)
                {
                    _buttonScrollLeft?.Show(ShowOverflowButton && Items.Count > 0 && Items.Count > _itemShowCount);
                    _buttonScrollRight?.Show(ShowOverflowButton && Items.Count > 0 && Items.Count > _itemShowCount);
                }
            }
        }

        private void Menu_Closed(object sender, RoutedEventArgs e) => _buttonOverflow.IsChecked = false;

        private void ButtonScrollRight_Click(object sender, RoutedEventArgs e) =>
            _scrollViewerOverflow.ScrollToHorizontalOffsetWithAnimation(Math.Min(
                _scrollViewerOverflow.CurrentHorizontalOffset + TabItemWidth, _scrollViewerOverflow.ScrollableWidth));

        private void ButtonScrollLeft_Click(object sender, RoutedEventArgs e) =>
            _scrollViewerOverflow.ScrollToHorizontalOffsetWithAnimation(Math.Max(
                _scrollViewerOverflow.CurrentHorizontalOffset - TabItemWidth, 0));

        private void ButtonOverflow_Click(object sender, RoutedEventArgs e)
        {
            if (_buttonOverflow.IsChecked == true)
            {
                _buttonOverflow.Menu.Items.Clear();
                for (var i = 0; i < Items.Count; i++)
                {
                    if (!(ItemContainerGenerator.ContainerFromIndex(i) is DragTabItem item)) continue;

                    var menuItem = new MenuItem
                    {
                        HeaderStringFormat = ItemStringFormat,
                        HeaderTemplate = ItemTemplate,
                        HeaderTemplateSelector = ItemTemplateSelector,
                        Header = item.Header,
                        Width = TabItemWidth,
                        IsChecked = item.IsSelected,
                        IsCheckable = true,
                        IsEnabled = item.IsEnabled
                    };

                    menuItem.Click += delegate
                    {
                        _buttonOverflow.IsChecked = false;

                        var list = GetActualList();
                        if (list == null) return;

                        var actualItem = ItemContainerGenerator.ItemFromContainer(item);
                        if (actualItem == null) return;

                        var index = list.IndexOf(actualItem);
                        if (index >= _itemShowCount)
                        {
                            list.Remove(actualItem);
                            list.Insert(0, actualItem);
                            HeaderPanel.SetValue(DragTabPanel.FluidMoveDurationPropertyKey,
                                IsAnimationEnabled
                                    ? new Duration(TimeSpan.FromMilliseconds(200))
                                    : new Duration(TimeSpan.FromMilliseconds(0)));
                            HeaderPanel.ForceUpdate = true;
                            HeaderPanel.Measure(new Size(HeaderPanel.DesiredSize.Width, ActualHeight));
                            HeaderPanel.ForceUpdate = false;
                            SetCurrentValue(SelectedIndexProperty, ValueBoxes.Int0Box);
                        }

                        item.IsSelected = true;
                    };
                    _buttonOverflow.Menu.Items.Add(menuItem);
                }
            }
        }

        internal double GetHorizontalOffset() => _scrollViewerOverflow?.CurrentHorizontalOffset ?? 0;

        internal void UpdateScroll() => _scrollViewerOverflow?.RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 0)
        {
            RoutedEvent = MouseWheelEvent
        });

        internal void CloseAllItems() => CloseOtherItems(null);

        internal void CloseOtherItems(DragTabItem currentItem)
        {
            var actualItem = currentItem != null ? ItemContainerGenerator.ItemFromContainer(currentItem) : null;

            var list = GetActualList();
            if (list == null) return;

            IsInternalAction = true;

            for (var i = 0; i < Items.Count; i++)
            {
                var item = list[i];
                if (!Equals(item, actualItem) && item != null)
                {
                    var argsClosing = new CancelRoutedEventArgs(DragTabItem.ClosingEvent, item);

                    if (!(ItemContainerGenerator.ContainerFromItem(item) is DragTabItem tabItem)) continue;

                    tabItem.RaiseEvent(argsClosing);
                    if (argsClosing.Cancel) return;

                    tabItem.RaiseEvent(new RoutedEventArgs(DragTabItem.ClosedEvent, item));
                    list.Remove(item);

                    i--;
                }
            }

            SetCurrentValue(SelectedIndexProperty, Items.Count == 0 ? -1 : 0);
        }

        internal IList GetActualList()
        {
            IList list;
            if (ItemsSource != null)
            {
                list = ItemsSource as IList;
            }
            else
            {
                list = Items;
            }

            return list;
        }

        /// <summary>
        /// 获取显示的Item数量
        /// </summary>
        /// <returns></returns>
        internal int GetItemShowCount()
        {
            if (HeaderPanel == null)
                return 0;

            return HeaderPanel.GetItemsCountFromOffset(ActualWidth);
        }


        protected override bool IsItemItsOwnContainerOverride(object item) => item is DragTabItem;

        protected override DependencyObject GetContainerForItemOverride() => new DragTabItem();
    }

    /// <summary>
    ///     装箱后的值类型（用于提高效率）
    /// </summary>
    internal static class ValueBoxes
    {
        internal static object TrueBox = true;

        internal static object FalseBox = false;

        internal static object VerticalBox = Orientation.Vertical;

        internal static object HorizontalBox = Orientation.Horizontal;

        internal static object Double0Box = .0;

        internal static object Double01Box = .1;

        internal static object Double1Box = 1.0;

        internal static object Double10Box = 10.0;

        internal static object Double20Box = 20.0;

        internal static object Double100Box = 100.0;

        internal static object Double200Box = 200.0;

        internal static object Double300Box = 300.0;

        internal static object DoubleNeg1Box = -1.0;

        internal static object Int0Box = 0;

        internal static object Int1Box = 1;

        internal static object Int2Box = 2;

        internal static object Int5Box = 5;

        internal static object Int99Box = 99;

        internal static object BooleanBox(bool value) => value ? TrueBox : FalseBox;

        internal static object OrientationBox(Orientation value) =>
            value == Orientation.Horizontal ? HorizontalBox : VerticalBox;
    }

    /// <summary>
    ///     带上下文菜单的切换按钮
    /// </summary>
    public class ContextMenuToggleButton : ToggleButton
    {
        public ContextMenu Menu { get; set; }

        protected override void OnClick()
        {
            base.OnClick();
            if (Menu != null)
            {
                if (IsChecked == true)
                {
                    Menu.PlacementTarget = this;
                    Menu.IsOpen = true;
                }
                else
                {
                    Menu.IsOpen = false;
                }
            }
        }
    }
}

