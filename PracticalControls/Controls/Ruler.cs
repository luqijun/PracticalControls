
using GalaSoft.MvvmLight;
using PracticalControls.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PracticalControls.Controls
{
    public class Ruler : Control
    {
        #region Constructors

        static Ruler()
        {
            // Initialize CommandCollection & CommandLink(s)
            InitializeCommands();

            // For auto find template in Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(typeof(Ruler)));
        }

        /// <summary>
        /// Instantiates a new instance of a Ruler.
        /// </summary>
        public Ruler()
        {
            this.IsHitTestVisible = true;
        }

        #endregion

        #region Commands

        private static RoutedCommand _deleteLineCommand = null;

        /// <summary>
        /// Top Buttons Click
        /// </summary>
        public static RoutedCommand DeleteLineCommand
        {
            get { return _deleteLineCommand; }
        }

        static void InitializeCommands()
        {
            _deleteLineCommand = new RoutedCommand("DeleteLineCommand", typeof(Ruler));
            CommandManager.RegisterClassCommandBinding(typeof(Ruler), new CommandBinding(_deleteLineCommand, OnExecuteDeleteLineCommand, null));
        }

        private static void OnExecuteDeleteLineCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Ruler ruler = (Ruler)sender;
            MeasureLineInfo selectedLineInfo = e.Parameter as MeasureLineInfo;
            var linesSource = ruler.LinesSource as IList<MeasureLineInfo>;
            linesSource.Remove(selectedLineInfo);
        }

        #endregion


        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(Ruler), new PropertyMetadata(Brushes.Black));


        /// <summary>
        /// Whether auto resize line when resizing control
        /// </summary>
        public bool AutoResize
        {
            get { return (bool)GetValue(AutoResizeProperty); }
            set { SetValue(AutoResizeProperty, value); }
        }

        public static readonly DependencyProperty AutoResizeProperty =
            DependencyProperty.Register("AutoResize", typeof(bool), typeof(Ruler), new PropertyMetadata(false));


        /// <summary>
        /// Origin X / UI X
        /// </summary>
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(Ruler), new PropertyMetadata(1d, OnScalePorpertyChanged));


        /// <summary>
        /// Origin Y / UI Y
        /// </summary>
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(Ruler), new PropertyMetadata(1d, OnScalePorpertyChanged));

        private static void OnScalePorpertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ruler = d as Ruler;
            ruler.UpdateLinesSource();
        }

        private void UpdateLinesSource()
        {
            if (this.LinesSource is IList<MeasureLineInfo> lstLineInfo)
            {
                foreach (var lineInfo in lstLineInfo)
                {
                    lineInfo.StartPoint = ConvertToUIPoint(lineInfo.OriginStartPoint);
                    lineInfo.EndPoint = ConvertToUIPoint(lineInfo.OriginEndPoint);
                }
            }
        }

        public object LinesSource
        {
            get { return (object)GetValue(LinesSourceProperty); }
            set { SetValue(LinesSourceProperty, value); }
        }

        public static readonly DependencyProperty LinesSourceProperty =
            DependencyProperty.Register("LinesSource", typeof(object), typeof(Ruler), new FrameworkPropertyMetadata(new List<MeasureLineInfo>(), FrameworkPropertyMetadataOptions.None, OnLinesSourcePropertyChanged));

        private static void OnLinesSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ruler = d as Ruler;
            ruler.UpdateLinesSource();
        }

        public bool IsMeasuring
        {
            get { return (bool)GetValue(IsMeasuringProperty); }
            set { SetValue(IsMeasuringProperty, value); }
        }

        public static readonly DependencyProperty IsMeasuringProperty =
            DependencyProperty.Register("IsMeasuring", typeof(bool), typeof(Ruler), new PropertyMetadata(false));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PreviewMouseLeftButtonDown += Ruler_PreviewMouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += Ruler_PreviewMouseLeftButtonUp;
            this.PreviewMouseMove += Ruler_PreviewMouseMove;
        }

        private void Ruler_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _movingLine = null;
            _currentPoint = e.GetPosition(this);

            if (this.IsMeasuring)
            {
                // End drawing
                this.IsMeasuring = false;
                //var lstLineInfo = this.LinesSource as IList<MeasureLineInfo>;
                //lstLineInfo.Add(new MeasureLineInfo() { StartPoint = _startPoint, EndPoint = _currentPoint });
                //InvalidateProperty(LinesSourceProperty);
                //InvalidateVisual();
            }
            else
            {
                _startPoint = _currentPoint;

                //HitTest to test moving line
                HitTestResult hitResult = VisualTreeHelper.HitTest(this, _currentPoint);
                if (hitResult.VisualHit is Line line)
                {
                    _movingLine = line.DataContext as MeasureLineInfo;
                    return;
                }

                // Start drawing
                this.IsMeasuring = true;
                _drawingLine = new MeasureLineInfo();
                _drawingLine.StartPoint = _startPoint;
                _drawingLine.EndPoint = _currentPoint;
                _drawingLine.OriginStartPoint = ConvertToOriginPoint(_drawingLine.StartPoint);
                _drawingLine.OriginEndPoint = ConvertToOriginPoint(_drawingLine.EndPoint);

                var lstLineInfo = this.LinesSource as IList<MeasureLineInfo>;
                lstLineInfo.Add(_drawingLine);

                //InvalidateProperty(LinesSourceProperty);
                //InvalidateVisual();
            }
        }

        private void Ruler_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _movingLine = null;
        }

        private void Ruler_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _currentPoint = e.GetPosition(this);

            // Move line
            if (_movingLine != null && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                double deltaX = _currentPoint.X - _startPoint.X;
                double deltaY = _currentPoint.Y - _startPoint.Y;
                _movingLine.StartPoint = new Point(_movingLine.StartPoint.X + deltaX, _movingLine.StartPoint.Y + deltaY);
                _movingLine.EndPoint = new Point(_movingLine.EndPoint.X + deltaX, _movingLine.EndPoint.Y + deltaY);
                _movingLine.OriginStartPoint = ConvertToOriginPoint(_movingLine.StartPoint);
                _movingLine.OriginEndPoint = ConvertToOriginPoint(_movingLine.EndPoint);
                _startPoint = _currentPoint;
            }

            // Measuring
            if (this.IsMeasuring)
            {
                _drawingLine.EndPoint = _currentPoint;
                _drawingLine.OriginEndPoint = ConvertToOriginPoint(_drawingLine.EndPoint);
                _drawingLine.OriginDistance = Math.Round(CalcuteDistance(_drawingLine.StartPoint, _drawingLine.EndPoint), 2);
                //InvalidateVisual();
            }
        }

        private double CalcuteDistance(Point startPoint, Point endPoint)
        {
            double deltaX = Math.Abs(endPoint.X - startPoint.X) / ScaleX;
            double deltaY = Math.Abs(endPoint.Y - startPoint.Y) / ScaleY;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
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

        #region Draw Measure Line

        Point _startPoint;
        Point _currentPoint;

        MeasureLineInfo _drawingLine;// current drawing line

        MeasureLineInfo _movingLine;// current moving line


        //Brush _circleBursh = new SolidColorBrush(Colors.Transparent);
        //Pen _linePen = new Pen(new SolidColorBrush(Colors.Red), 1);

        #region TEST OnRender

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);

        //    if (this.IsMeasuring)
        //        DrawLine(drawingContext, _startPoint, _currentPoint);
        //}

        ///// <summary>
        ///// Draw a line when mouse click and move
        ///// </summary>
        ///// <param name="dc"></param>
        ///// <param name="startPoint"></param>
        ///// <param name="endPoint"></param>
        //private void DrawLine(DrawingContext dc, Point startPoint, Point endPoint)
        //{
        //    if (!this.IsMeasuring)
        //        return;

        //    dc.DrawEllipse(_circleBursh, _linePen, _startPoint, 5, 5);
        //    dc.DrawLine(_linePen, _startPoint, endPoint);
        //    dc.DrawEllipse(_circleBursh, _linePen, endPoint, 5, 5);
        //}

        #endregion

        #endregion

    }

    public class MeasureLineInfo : ViewModelBase
    {
        private Point _originStartPoint;

        public Point OriginStartPoint
        {
            get { return _originStartPoint; }
            set { Set(ref _originStartPoint, value); }
        }

        private Point _originEndPoint;

        public Point OriginEndPoint
        {
            get { return _originEndPoint; }
            set { Set(ref _originEndPoint, value); }
        }

        private double _originDistance;

        public double OriginDistance
        {
            get { return _originDistance; }
            set { Set(ref _originDistance, value); }
        }


        private Point _startPoint;
        /// <summary>
        /// The start line position in UI
        /// </summary>
        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                if (Set(ref _startPoint, value))
                {
                    RaisePropertyChanged(nameof(TextPositionX));
                    RaisePropertyChanged(nameof(TextPositionY));
                }
            }
        }

        private Point _endPoint;
        /// <summary>
        /// The end line position in UI
        /// </summary>
        public Point EndPoint
        {
            get { return _endPoint; }
            set { Set(ref _endPoint, value); }
        }

        public double TextPositionX
        {
            get
            {
                return StartPoint.X - 30;
            }
        }

        public double TextPositionY
        {
            get
            {
                return StartPoint.Y + 5;
            }
        }
    }
}
