using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PracticalControls.Adorners
{
    public class DefaultTreeViewItemDragAdorner : Adorner
    {
        Transform _startTransform;
        TransformGroup _moveTransform = new TransformGroup();
        Transform _dragTranform;
        public Transform DragTranform
        {
            get
            {
                return _dragTranform;
            }
            set
            {
                _dragTranform = value;
                _moveTransform.Children.Clear();
                _moveTransform.Children.Add(_dragTranform);
            }
        }

        FrameworkElement _previewControl { get; set; }

        List<Visual> _visuals;

        public DefaultTreeViewItemDragAdorner(UIElement adornedElement, TreeViewItem tv) : base(adornedElement)
        {
            _previewControl = new TreeViewItem()
            {
                Opacity = 0.5,
                Style = tv.Style,
                Header = tv.Header,
                Template = tv.Template,
                ItemTemplate = tv.ItemTemplate,
                HeaderTemplate = tv.HeaderTemplate,
                DataContext = tv.DataContext,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.LightBlue),
            };

            _visuals = new List<Visual>() { _previewControl };

            var startPoint = Mouse.GetPosition(adornedElement);
            _startTransform = new TranslateTransform(0, startPoint.Y - tv.ActualHeight / 2);
        }

        #region 重写方法

        protected override int VisualChildrenCount => _visuals.Count;
        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override Size MeasureOverride(Size finalSize)
        {
            _previewControl.Measure(finalSize);
            return _previewControl.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _previewControl.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            TransformGroup group = new TransformGroup();
            group.Children.Add(this._startTransform);
            group.Children.Add(this._moveTransform);
            group.Children.Add(transform as Transform);
            System.Diagnostics.Debug.WriteLine("1");
            return group;
        }

        #endregion


        #region 其他方法实现跟随鼠标

        ///// <summary>
        ///// Set the position of and redraw the adorner.
        ///// Call when the mouse cursor position changes.
        ///// </summary>
        ///// <param name="position">Adorner's new position relative to AdornerLayer origin</param>
        //public void SetMousePosition(Point position)
        //{
        //    this._adornerOffset.X = position.X - this._adornerOrigin.X;
        //    this._adornerOffset.Y = position.Y - this._adornerOrigin.Y;
        //    UpdatePosition();
        //}

        //private void UpdatePosition()
        //{
        //    AdornerLayer adornerLayer = (AdornerLayer)this.Parent;
        //    if (adornerLayer != null)
        //    {
        //        adornerLayer.Update(this.AdornedElement);
        //    }
        //}

        //public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        //{
        //    GeneralTransformGroup newTransform = new GeneralTransformGroup();
        //    newTransform.Children.Add(base.GetDesiredTransform(transform));
        //    newTransform.Children.Add(new TranslateTransform(this._adornerOffset.X, this._adornerOffset.Y));
        //    return newTransform;
        //}

        #endregion
    }
}
