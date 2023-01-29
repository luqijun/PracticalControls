using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PracticalControls.Controls
{
    /// <summary>
    /// Angle Measurer
    /// </summary>
    [StyleTypedProperty(Property = "ThumbStyle", StyleTargetType = typeof(Thumb))]
    public class AngleMeasurer : Shape
    {
        PointCollection _originPoints;

        #region Constructors

        static AngleMeasurer()
        {
            // Initialize CommandCollection & CommandLink(s)
            //InitializeCommands();

            // For auto find template in Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AngleMeasurer), new FrameworkPropertyMetadata(typeof(AngleMeasurer)));
        }

        /// <summary>
        /// Instantiates a new instance of a Ruler.
        /// </summary>
        public AngleMeasurer()
        {
            this.IsHitTestVisible = true;
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

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            InitVisuals();

            this.Cursor = Cursors.SizeAll;
            this.Fill = new SolidColorBrush(Colors.Yellow);

            this.PreviewMouseLeftButtonDown += AngleMeasurer_PreviewMouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += AngleMeasurer_PreviewMouseLeftButtonUp;
            this.PreviewMouseMove += AngleMeasurer_PreviewMouseMove;
        }

        #region Move 

        Point? _startPoint;

        private void AngleMeasurer_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is AngleMeasurer)
                _startPoint = e.GetPosition(this);

        }
        private void AngleMeasurer_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = null;
        }

        private void AngleMeasurer_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_startPoint != null && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(this);
                if (currentPoint.X < 0 || currentPoint.X >= this.ActualWidth)
                    return;
                if (currentPoint.Y < 0 || currentPoint.Y >= this.ActualHeight)
                    return;

                Vector moveDirection = currentPoint - _startPoint.Value;
                if (moveDirection.Length > 0)
                {
                    var newPoints = this.Points.Select(p => p + moveDirection);
                    this.Points = new PointCollection(newPoints);
                    _startPoint = currentPoint;
                }
            }
        }

        #endregion


        /// <summary>
        /// Whether auto resize line when resizing control
        /// </summary>
        public bool AutoResize
        {
            get { return (bool)GetValue(AutoResizeProperty); }
            set { SetValue(AutoResizeProperty, value); }
        }

        public static readonly DependencyProperty AutoResizeProperty =
            DependencyProperty.Register("AutoResize", typeof(bool), typeof(AngleMeasurer), new PropertyMetadata(false));


        /// <summary>
        /// Origin X / UI X
        /// </summary>
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(AngleMeasurer), new PropertyMetadata(1d, OnScalePorpertyChanged));


        /// <summary>
        /// Origin Y / UI Y
        /// </summary>
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(AngleMeasurer), new PropertyMetadata(1d, OnScalePorpertyChanged));

        private static void OnScalePorpertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var measurer = d as AngleMeasurer;
            measurer.UpdatePointsSource();
        }

        private void UpdatePointsSource()
        {
            if (_startPoint != null)
                return;

            if (_originPoints != null)
            {
                PointCollection points = new PointCollection(_originPoints.Select(p => ConvertToUIPoint(p)));
                this.Points = points;
            }
        }

        private Point ConvertToOriginPoint(Point point)
        {
            if (!this.AutoResize)
                return point;

            return new Point(point.X / this.ScaleX, point.Y / this.ScaleY);
        }

        private Point ConvertToUIPoint(Point point)
        {
            if (!this.AutoResize)
                return point;

            return new Point(point.X * this.ScaleX, point.Y * this.ScaleY);
        }


        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(AngleMeasurer),
                                        new FrameworkPropertyMetadata(new PointCollection(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPointsPropertyChanged));

        private static void OnPointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var angleMeasurer = d as AngleMeasurer;
            var newPoints = e.NewValue as PointCollection;
            angleMeasurer._originPoints = new PointCollection(newPoints.Select(p => angleMeasurer.ConvertToOriginPoint(p)));
        }

        /// <summary>
        /// Thumb Style
        /// </summary>
        public Style ThumbStyle
        {
            get { return (Style)GetValue(ThumbStyleProperty); }
            set { SetValue(ThumbStyleProperty, value); }
        }

        public static readonly DependencyProperty ThumbStyleProperty =
                DependencyProperty.Register("ThumbStyle", typeof(Style), typeof(AngleMeasurer), new PropertyMetadata(default));

        /// <summary>
        /// The radius of arc
        /// </summary>
        public double ArcRadius
        {
            get { return (double)GetValue(ArcRadiusProperty); }
            set { SetValue(ArcRadiusProperty, value); }
        }

        public static readonly DependencyProperty ArcRadiusProperty =
            DependencyProperty.Register("ArcRadius", typeof(double), typeof(AngleMeasurer), new PropertyMetadata(20d));


        #region Angle Shape

        Thumb _leftThumb;
        Thumb _rightThumb;

        TextBlock _tbAngle;

        private void InitVisuals()
        {
            Thumb CreateThumb(string name, object tag = null)
            {
                Thumb thumb = new Thumb() { Name = name };
                thumb.Tag = tag;
                thumb.DragStarted += Thumb_DragStarted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.Style = ThumbStyle;
                return thumb;
            }

            _leftThumb = CreateThumb("leftThumb", 0);
            _rightThumb = CreateThumb("rightThumb", 2);

            _visuals.Add(_leftThumb);
            _visuals.Add(_rightThumb);
            AddVisualChild(_leftThumb);
            AddVisualChild(_rightThumb);

            //Angle Text
            _tbAngle = new TextBlock() { Foreground = this.Stroke };
            _visuals.Add(_tbAngle);
            AddVisualChild(_tbAngle);
        }

        bool _isDragging = false;

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            //_draggingPointIndex = (int)((sender as Thumb).Tag);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;

            //Change point value
            int index = (int)((sender as Thumb).Tag);
            Point point = Points[index];
            Point newPoint = new Point(point.X + e.HorizontalChange, point.Y + e.VerticalChange);
            newPoint.X = Math.Min(this.ActualWidth, newPoint.X);
            newPoint.Y = Math.Min(this.ActualHeight, newPoint.Y);
            _originPoints[index] = ConvertToOriginPoint(newPoint);
            this.Points[index] = newPoint;
            this.InvalidateMeasure();
            this.InvalidateVisual();
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            //RaiseDragThumbCompletedEvent(sender);
        }


        #endregion

        private Geometry _polylineGeometry = Geometry.Empty;
        protected override Geometry DefiningGeometry => _polylineGeometry;

        protected override Size MeasureOverride(Size constraint)
        {
            CacheDefiningGeometry(constraint);
            return base.MeasureOverride(constraint);
        }

        private void CacheDefiningGeometry(Size constraint)
        {
            PointCollection pointCollection = Points;

            // Are we degenerate?
            // Yes, if we don't have data
            if (pointCollection == null)
                return;

            if (pointCollection.Count < 3)
                return;

            if (_originPoints.Count < 3)
                _originPoints = new PointCollection(pointCollection.Select(p => ConvertToOriginPoint(p)));

            var startPoint = pointCollection[0];
            var centerPoint = pointCollection[1];
            var endPoint = pointCollection[2];

            GeometryGroup geometryGroup = new GeometryGroup();

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.FillRule = FillRule.EvenOdd;
            geometryGroup.Children.Add(pathGeometry);

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = startPoint;
            Point[] array = new Point[pointCollection.Count - 1];
            for (int i = 1; i < 3; i++)
            {
                array[i - 1] = pointCollection[i];
            }
            pathFigure.Segments.Add(new PolyLineSegment(array, true));
            pathGeometry.Figures.Add(pathFigure);

            // Arc Line  先计算单位向量 再乘以半径
            double radius = this.ArcRadius;
            Vector leftVector = startPoint - centerPoint;
            leftVector.Normalize();
            leftVector *= radius;
            Vector rightVector = endPoint - centerPoint;
            rightVector.Normalize();
            rightVector *= radius;

            //判断基准线方向是否为正向
            bool isBaseLinePositive = leftVector.X > 0;

            //判断夹角是否大于180度 (方法：将第二个点带入y=kx+b中计算)
            double tempY = (leftVector.Y / leftVector.X) * (endPoint.X - startPoint.X) + startPoint.Y;
            bool isLargeArc = (tempY - endPoint.Y) < 0;
            if (isBaseLinePositive)
                isLargeArc = !isLargeArc;

            //计算两个向量之间的夹角 (使用原始数据点计算)
            Vector originLeftVector = _originPoints[0] - _originPoints[1];
            Vector originRightVector = _originPoints[2] - _originPoints[1];
            double theta = Math.Acos(originLeftVector * originRightVector / (originLeftVector.Length * originRightVector.Length));
            double angle = isLargeArc ? (360 - 180 * theta / Math.PI) : 180 * theta / Math.PI;
            _tbAngle.Text = angle.ToString("0.00") + "°";
            _tbAngle.InvalidateProperty(TextBlock.TextProperty);

            Point arcStartPoint = centerPoint + leftVector;
            Point arcEndPoint = centerPoint + rightVector;
            pathFigure = new PathFigure();
            pathFigure.StartPoint = arcStartPoint;
            pathFigure.Segments.Add(new ArcSegment(arcEndPoint, new Size(radius, radius), 0d, isLargeArc, SweepDirection.Clockwise, true));
            pathFigure.Segments.Add(new LineSegment(centerPoint, true));
            pathFigure.Segments.Add(new LineSegment(startPoint, true));
            pathGeometry.Figures.Add(pathFigure);


            // Add rectangle for mousemoving test
            //RectangleGeometry rectangleGeometry = new RectangleGeometry();
            //rectangleGeometry.Rect = new Rect(constraint);
            //pathGeometry.AddGeometry(rectangleGeometry);

            if (pathGeometry.Bounds == Rect.Empty)
                _polylineGeometry = Geometry.Empty;
            else
                _polylineGeometry = geometryGroup;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            PointCollection pointCollection = Points;

            if (pointCollection == null || pointCollection.Count < 3)
                return size;

            void ArrangeThumb(Thumb thumb, Point point)
            {
                //Measure the desiredSize of Thumb
                thumb.Measure(finalSize);

                //Ellipse ellipse = new Ellipse() { Stroke = new SolidColorBrush(Colors.Red), Fill = new SolidColorBrush(Colors.White), StrokeThickness = 1 };
                Point topLeftPoint = new Point(point.X - thumb.DesiredSize.Width / 2, point.Y - thumb.DesiredSize.Height / 2);
                Rect rect = new Rect(topLeftPoint, thumb.DesiredSize);
                thumb.Arrange(rect);
            }

            //Arrange the thumbs
            ArrangeThumb(_leftThumb, pointCollection[0]);
            ArrangeThumb(_rightThumb, pointCollection[2]);

            //Arrange the text
            _tbAngle.Measure(finalSize);
            Point txtStartPoint = pointCollection[1];
            Rect txtRect = new Rect(txtStartPoint, txtStartPoint + new Vector(_tbAngle.DesiredSize.Width, _tbAngle.DesiredSize.Height));
            _tbAngle.Arrange(txtRect);

            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // To draw background. This can help to hit the control.
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 1), new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        private bool IsPointValid(Point p)
        {
            if (p.X == double.NaN || p.Y == double.NaN || double.IsInfinity(p.X) || double.IsInfinity(p.Y))
            {
                return false;
            }

            return true;
        }
    }
}
