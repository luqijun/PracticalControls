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
    [StyleTypedProperty(Property = "ThumbStyle", StyleTargetType = typeof(Thumb))]
    public class EditablePolyline : Shape
    {
        #region Constructors

        static EditablePolyline()
        {
            // For auto find template in Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditablePolyline), new FrameworkPropertyMetadata(typeof(EditablePolyline)));
        }

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



        /// <summary>
        /// Thumbs Interval
        /// </summary>
        public int ThumbsInterval
        {
            get { return (int)GetValue(ThumbsIntervalProperty); }
            set { SetValue(ThumbsIntervalProperty, value); }
        }

        public static readonly DependencyProperty ThumbsIntervalProperty =
            DependencyProperty.Register("ThumbsInterval", typeof(int), typeof(EditablePolyline), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Drag mode 
        /// </summary>
        public DragMode DragMode
        {
            get { return (DragMode)GetValue(DragModeProperty); }
            set { SetValue(DragModeProperty, value); }
        }

        public static readonly DependencyProperty DragModeProperty =
            DependencyProperty.Register("DragMode", typeof(DragMode), typeof(EditablePolyline), new PropertyMetadata(DragMode.Both));

        #endregion

        #region Custom Events

        public static readonly RoutedEvent DragThumbStartedEvent = EventManager.RegisterRoutedEvent(
          name: "DragThumbStarted",
          routingStrategy: RoutingStrategy.Bubble,
          handlerType: typeof(RoutedEventHandler),
          ownerType: typeof(EditablePolyline));

        /// <summary>
        /// Drag thumb started
        /// </summary>
        public event RoutedEventHandler DragThumbStarted
        {
            add { AddHandler(DragThumbStartedEvent, value); }
            remove { RemoveHandler(DragThumbStartedEvent, value); }
        }

        public static readonly RoutedEvent DragThumbDeltaEvent = EventManager.RegisterRoutedEvent(
           name: "DragThumbDelta",
           routingStrategy: RoutingStrategy.Bubble,
           handlerType: typeof(RoutedEventHandler),
           ownerType: typeof(EditablePolyline));

        /// <summary>
        /// Drag thumb delta
        /// </summary>
        public event RoutedEventHandler DragThumbDelta
        {
            add { AddHandler(DragThumbDeltaEvent, value); }
            remove { RemoveHandler(DragThumbDeltaEvent, value); }
        }


        public static readonly RoutedEvent DragThumbCompletedEvent = EventManager.RegisterRoutedEvent(
            name: "DragThumbCompleted",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(EditablePolyline));

        /// <summary>
        /// Drag thumb completed
        /// </summary>
        public event RoutedEventHandler DragThumbCompleted
        {
            add { AddHandler(DragThumbCompletedEvent, value); }
            remove { RemoveHandler(DragThumbCompletedEvent, value); }
        }

        private void RaiseDragThumbStartedEvent(object sender)
        {
            PloylineDragEventArgs routedEventArgs = new(routedEvent: DragThumbStartedEvent);
            routedEventArgs.Source = sender;
            routedEventArgs.PointIndex = _draggingPointIndex;
            routedEventArgs.OldPoint = _dragStartPoint;
            routedEventArgs.NewPoint = _dragStartPoint;

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }

        private void RaiseDragThumbDeltaEvent(object sender)
        {
            PloylineDragEventArgs routedEventArgs = new(routedEvent: DragThumbDeltaEvent);
            routedEventArgs.Source = sender;
            routedEventArgs.PointIndex = _draggingPointIndex;
            routedEventArgs.OldPoint = _draggingOldPoint;
            routedEventArgs.NewPoint = this.Points[_draggingPointIndex];

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }

        private void RaiseDragThumbCompletedEvent(object sender)
        {
            // Create a RoutedEventArgs instance.
            PloylineDragEventArgs routedEventArgs = new(routedEvent: DragThumbCompletedEvent);
            routedEventArgs.Source = sender;
            routedEventArgs.PointIndex = _draggingPointIndex;
            routedEventArgs.OldPoint = _dragStartPoint;
            routedEventArgs.NewPoint = this.Points[_draggingPointIndex];

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }

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
                new FrameworkPropertyMetadata(new PointCollection(), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, OnPointsPropertyChanged));

        private static void OnPointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

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

            // Clear Thumbs
            if (!this.ShowThumb)
            {
                foreach (var visual in _visuals)
                {
                    if (visual is Thumb)
                        RemoveVisualChild(visual);
                }
                _visuals.Clear();
                return size;
            }

            if (pointCollection == null || pointCollection.Count == 0 || _isDragging)
                return size;

            //Synchronize the number of thumbs
            SynchronizeThumbs();

            //Arrange the thumbs
            for (int i = 0; i < _visuals.Count; i++)
            {
                Thumb thumb = _visuals[i] as Thumb;
                if (thumb == null)
                    continue;

                int pointIndex = (int)thumb.Tag;
                Point point = pointCollection[pointIndex];

                //Measure the desiredSize of Thumb
                thumb.Measure(finalSize);

                //Ellipse ellipse = new Ellipse() { Stroke = new SolidColorBrush(Colors.Red), Fill = new SolidColorBrush(Colors.White), StrokeThickness = 1 };
                Point topLeftPoint = new Point(point.X - thumb.DesiredSize.Width / 2, point.Y - thumb.DesiredSize.Height / 2);
                Rect rect = new Rect(topLeftPoint, thumb.DesiredSize);
                thumb.Arrange(rect);
            }

            return size;
        }

        /// <summary>
        /// Synchronize the number of thumbs
        /// </summary>
        private void SynchronizeThumbs()
        {
            if (this.ThumbsInterval == -1)
                return;

            PointCollection pointCollection = Points;
            if (pointCollection.Count == 0)
                return;

            int thumbsInterval = Math.Max(1, this.ThumbsInterval);

            int currentThumbCount = _visuals.Count;
            int targetThumbCount = (int)Math.Ceiling(pointCollection.Count * 1.0 / thumbsInterval) + 1;

            // Add Thumbs
            if (currentThumbCount < targetThumbCount)
            {
                for (int i = currentThumbCount; i < targetThumbCount; i++)
                {
                    Thumb thumb = new Thumb();
                    thumb.Tag = Math.Min(pointCollection.Count - 1, i * thumbsInterval);
                    thumb.PreviewMouseLeftButtonDown += Thumb_PreviewMouseLeftButtonDown;
                    thumb.DragStarted += Thumb_DragStarted;
                    thumb.DragDelta += Thumb_DragDelta;
                    thumb.DragCompleted += Thumb_DragCompleted;
                    thumb.Style = this.ThumbStyle;

                    _visuals.Add(thumb);
                    AddVisualChild(thumb);
                }
            }

            // Remove Thumbs
            if (currentThumbCount > targetThumbCount)
            {
                int subCount = currentThumbCount - targetThumbCount;
                for (int i = 0; i < subCount; i++)
                {
                    var visual = _visuals.LastOrDefault();
                    if (visual is Thumb thumb)
                    {
                        thumb.DragStarted -= Thumb_DragStarted;
                        thumb.DragDelta -= Thumb_DragDelta;
                        thumb.DragCompleted -= Thumb_DragCompleted;
                    }

                    RemoveVisualChild(visual);
                    _visuals.Remove(visual);
                }

                // Reset Tag
                for (int i = 0; i < _visuals.Count; i++)
                {
                    Thumb thumb = _visuals[i] as Thumb;
                    thumb.Tag = Math.Min(pointCollection.Count - 1, i * thumbsInterval);
                }
            }
        }

        private void Thumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        #region Thumb Events

        int _draggingPointIndex = -1;
        bool _isDragging = false;
        Point _dragStartPoint;
        Point _draggingOldPoint;

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            _draggingPointIndex = (int)((sender as Thumb).Tag);
            _dragStartPoint = this.Points[_draggingPointIndex];
            _draggingOldPoint = _dragStartPoint;

            RaiseDragThumbStartedEvent(sender);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;

            //Change point value
            int index = (int)((sender as Thumb).Tag);
            double horizontalChagne = this.DragMode == DragMode.Vertical ? 0d : e.HorizontalChange;
            double verticalChagne = this.DragMode == DragMode.Horizontal ? 0d : e.VerticalChange;
            Point point = Points[index];
            Point newPoint = new Point(point.X + horizontalChagne, point.Y + verticalChagne);

            //Rearrange the dragging thumb
            Point topLeftPoint = new Point(newPoint.X - thumb.DesiredSize.Width / 2, newPoint.Y - thumb.DesiredSize.Height / 2);
            Rect rect = new Rect(topLeftPoint, thumb.DesiredSize);
            thumb.Arrange(rect);

            this.Points[index] = newPoint;
            this.InvalidateMeasure();
            this.InvalidateVisual();
            //this.Points = new PointCollection(this.Points);

            RaiseDragThumbDeltaEvent(sender);

            _draggingOldPoint = newPoint;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            RaiseDragThumbCompletedEvent(sender);
        }

        #endregion

        #endregion Internal methods

        #region Public methods

        /// <summary>
        /// Move the whole polyline
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public void Move(double offsetX, double offsetY)
        {
            PointCollection newPoints = new PointCollection();
            for (int i = 0; i < this.Points.Count; i++)
            {
                Point point = this.Points[i];
                point.X += offsetX;
                point.Y += offsetY;
                newPoints.Add(point);
            }
            this.Points = newPoints;
        }

        #endregion
    }


    public class PloylineDragEventArgs : RoutedEventArgs
    {
        public double HorizontalChange { get; set; }

        public double VerticalChange { get; set; }

        public int PointIndex { get; set; }

        public Point OldPoint { get; set; }

        public Point NewPoint { get; set; }

        public PloylineDragEventArgs()
        {

        }

        public PloylineDragEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }

        public PloylineDragEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {

        }
    }

    public enum DragMode
    {
        Both,
        Horizontal,//Only support Horizontal drag
        Vertical,//Only support Vertical drag
    }
}
