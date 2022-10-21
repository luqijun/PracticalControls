using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ControlzEx.Theming;
using System.Windows.Controls;

namespace PracticalControls.Controls
{
    /// <summary>
    /// Editable Polyline 
    /// </summary>
    public class EditablePolyline : Shape
    {


        #region Constructors

        /// <summary>
        /// Instantiates a new instance of a Polyline.
        /// </summary>
        public EditablePolyline()
        {
            this.IsHitTestVisible = true;

            this.Unloaded += EditablePolyline_Unloaded;
        }

        private void EditablePolyline_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var visual in _visuals)
            {
                if (visual is Thumb thumb)
                {
                    thumb.DragStarted -= Thumb_DragStarted;
                    thumb.DragDelta -= Thumb_DragDelta;
                    thumb.DragCompleted -= Thumb_DragCompleted;
                }
            }
        }

        #endregion Constructors


        #region Private Methods and Members

        private Geometry _polylineGeometry;

        #endregion

        #region Custom Dependency Properties


        /// <summary>
        /// Thumb Style
        /// </summary>
        public Style ThumbStyle
        {
            get { return (Style)GetValue(ThumbStyleProperty); }
            set { SetValue(ThumbStyleProperty, value); }
        }

        public static readonly DependencyProperty ThumbStyleProperty =
            DependencyProperty.Register("ThumbStyle", typeof(Style), typeof(EditablePolyline), new PropertyMetadata(default));


        /// <summary>
        /// Show Thumb
        /// </summary>
        public bool ShowThumb
        {
            get { return (bool)GetValue(ShowThumbProperty); }
            set { SetValue(ShowThumbProperty, value); }
        }

        public static readonly DependencyProperty ShowThumbProperty =
            DependencyProperty.Register("ShowThumb", typeof(bool), typeof(EditablePolyline), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));



        #endregion


        #region Override Visuals

        List<Visual> _visuals = new List<Visual>();
        protected override int VisualChildrenCount => _visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        #endregion

        #region Dynamic Properties

        /// <summary>
        /// Points property
        /// </summary>
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
                "Points", typeof(PointCollection), typeof(EditablePolyline),
                new FrameworkPropertyMetadata(new PointCollection(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Points property
        /// </summary>
        public PointCollection Points
        {
            get
            {
                return (PointCollection)GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, value);
            }
        }

        /// <summary>
        /// FillRule property
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register(
            "FillRule",
            typeof(FillRule),
            typeof(EditablePolyline),
            new FrameworkPropertyMetadata(
                FillRule.EvenOdd,
                FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// FillRule property
        /// </summary>
        public FillRule FillRule
        {
            get
            {
                return (FillRule)GetValue(FillRuleProperty);
            }
            set
            {
                SetValue(FillRuleProperty, value);
            }
        }

        #endregion Dynamic Properties

        #region Protected Methods and Properties


        /// <summary>
        /// Get the polyline that defines this shape
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                return _polylineGeometry;
            }
        }

        #endregion

        #region Internal methods

        protected override Size MeasureOverride(Size constraint)
        {
            PointCollection pointCollection = Points;

            //Geometry
            CacheDefiningGeometry();

            ////Add Circle Thumbs
            //for (int i = 0; i < pointCollection.Count; i++)
            //{
            //    Thumb thumb = new Thumb();
            //    thumb.Focusable = false;
            //    thumb.Tag = i;
            //    thumb.DragStarted += Thumb_DragStarted;
            //    thumb.DragDelta += Thumb_DragDelta;
            //    thumb.DragCompleted += Thumb_DragCompleted;
            //    thumb.Style = this.ThumbStyle;

            //    //Ellipse ellipse = new Ellipse() { Stroke = new SolidColorBrush(Colors.Red), Fill = new SolidColorBrush(Colors.White), StrokeThickness = 1 };
            //    Rect rect = new Rect(pointCollection[i].X - 5, pointCollection[i].Y - 5, 10, 10);
            //    thumb.Arrange(rect);
            //    _visuals.Add(thumb);
            //    AddVisualChild(thumb);
            //}

            return base.MeasureOverride(constraint);
        }

        internal void CacheDefiningGeometry()
        {
            PointCollection pointCollection = Points;
            PathFigure pathFigure = new PathFigure();

            // Are we degenerate?
            // Yes, if we don't have data
            if (pointCollection == null)
            {
                _polylineGeometry = Geometry.Empty;
                return;
            }

            // Create the Polyline PathGeometry
            // ISSUE-Microsoft-07/11/2003 - Bug 859068
            // The constructor for PathFigure that takes a PointCollection is internal in the Core
            // so the below causes an A/V. Consider making it public.
            if (pointCollection.Count > 0)
            {
                pathFigure.StartPoint = pointCollection[0];

                if (pointCollection.Count > 1)
                {
                    Point[] array = new Point[pointCollection.Count - 1];

                    for (int i = 1; i < pointCollection.Count; i++)
                    {
                        array[i - 1] = pointCollection[i];
                    }

                    pathFigure.Segments.Add(new PolyLineSegment(array, true));
                }
            }

            PathGeometry polylineGeometry = new PathGeometry();
            polylineGeometry.Figures.Add(pathFigure);

            ////Circle
            //for (int i = 0; i < pointCollection.Count; i++)
            //{
            //    polylineGeometry.AddGeometry(new EllipseGeometry() { Center = pointCollection[i], RadiusX = 3, RadiusY = 3, });
            //}

            // Set FillRule
            polylineGeometry.FillRule = FillRule;

            if (polylineGeometry.Bounds == Rect.Empty)
            {
                _polylineGeometry = Geometry.Empty;
            }
            else
            {
                _polylineGeometry = polylineGeometry;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            PointCollection pointCollection = Points;


            if (_isDragging || !this.ShowThumb)
                return size;

            //Clear Old
            foreach (var visual in _visuals)
            {
                if (visual is Thumb thumb)
                {
                    thumb.DragStarted -= Thumb_DragStarted;
                    thumb.DragDelta -= Thumb_DragDelta;
                    thumb.DragCompleted -= Thumb_DragCompleted;
                }

                RemoveVisualChild(visual);
            }
            _visuals.Clear();

            //Add Circle Thumbs
            for (int i = 0; i < pointCollection.Count; i++)
            {
                Thumb thumb = new Thumb();
                thumb.Tag = i;
                thumb.DragStarted += Thumb_DragStarted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.Style = this.ThumbStyle;
                thumb.Measure(finalSize);//Measure this desiredSize of Thumb

                //Ellipse ellipse = new Ellipse() { Stroke = new SolidColorBrush(Colors.Red), Fill = new SolidColorBrush(Colors.White), StrokeThickness = 1 };
                Point topLeftPoint = new Point(pointCollection[i].X - thumb.DesiredSize.Width / 2, pointCollection[i].Y - thumb.DesiredSize.Height / 2);
                Rect rect = new Rect(topLeftPoint, thumb.DesiredSize);
                thumb.Arrange(rect);
                _visuals.Add(thumb);
                AddVisualChild(thumb);
            }



            return size;
        }

        #region Thumb Events

        bool _isDragging = false;

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;

            //Change Point Value
            int index = (int)((sender as Thumb).Tag);
            Point point = Points[index];
            Point newPoint = new Point(point.X + e.HorizontalChange, point.Y + e.VerticalChange);
            this.Points[index] = newPoint;

            //Move thumb
            Point topLeftPoint = new Point(newPoint.X - thumb.DesiredSize.Width / 2, newPoint.Y - thumb.DesiredSize.Height / 2);
            Rect rect = new Rect(topLeftPoint, thumb.DesiredSize);
            thumb.Arrange(rect);

            //this.InvalidateMeasure();
            //this.InvalidateArrange();
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
        }

        #endregion

        #endregion Internal methods

    }
}
