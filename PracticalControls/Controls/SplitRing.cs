using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PracticalControls.Controls
{
    public class SplitRing : FrameworkElement
    {
        //Ring
        DrawingVisual _ring;

        List<TextBlock> _textBlocks = new List<TextBlock>();
        List<Visual> _visuals = new List<Visual>();
        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        public SplitRing()
        {
            _ring = new DrawingVisual();
            _visuals.Add(_ring);

            InitBrushes();
        }


        #region Custom Dependency Properties

        /// <summary>
        /// The ratio of outter radius and inner radius.
        /// </summary>
        public double RadiusRatio
        {
            get { return (double)GetValue(RadiusRatioProperty); }
            set { SetValue(RadiusRatioProperty, value); }
        }

        public static readonly DependencyProperty RadiusRatioProperty =
            DependencyProperty.Register("RadiusRatio", typeof(double), typeof(SplitRing), new PropertyMetadata(2d));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty = TextBlock.FontSizeProperty.AddOwner(typeof(SplitRing));

        /// <summary>
        /// Texts in split areas
        /// </summary>
        public IList<string> TextsSource
        {
            get { return (List<string>)GetValue(TextsSourceProperty); }
            set { SetValue(TextsSourceProperty, value); }
        }

        public static readonly DependencyProperty TextsSourceProperty =
            DependencyProperty.Register("TextsSource", typeof(IList<string>), typeof(SplitRing), new PropertyMetadata(new List<string>()));



        /// <summary>
        /// The count of split areas
        /// </summary>
        public int SplitCount
        {
            get { return (int)GetValue(SplitCountProperty); }
            set { SetValue(SplitCountProperty, value); }
        }

        public static readonly DependencyProperty SplitCountProperty =
            DependencyProperty.Register("SplitCount", typeof(int), typeof(SplitRing), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));




        public bool IsRandomColor
        {
            get { return (bool)GetValue(IsRandomColorProperty); }
            set { SetValue(IsRandomColorProperty, value); }
        }

        public static readonly DependencyProperty IsRandomColorProperty =
            DependencyProperty.Register("IsRandomColor", typeof(bool), typeof(SplitRing), new PropertyMetadata(false));




        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(SplitRing), new PropertyMetadata(0d));


        #endregion


        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            RedrawRing();
        }

        private void RedrawRing()
        {
            DrawingContext dc = _ring.RenderOpen();

            double strokeThickness = 2d;

            var blackPen = new Pen(Brushes.Black, strokeThickness);

            double outterRadius = Math.Min(this.ActualWidth / 2, this.ActualHeight / 2);
            double innerRaius = outterRadius / this.RadiusRatio;

            if (this.SplitCount > 0)
            {
                Typeface typeface = new Typeface("Times New Roman");

                double perAngle = 360 / this.SplitCount;
                Size outterArcSize = new Size(outterRadius, outterRadius);
                Size innererArcSize = new Size(innerRaius, innerRaius);

                Point centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
                //Inner Circle
                dc.DrawEllipse(IsRandomColor ? GetRandomBrush() : Brushes.White, new Pen(), centerPoint, innerRaius, innerRaius);

                Point p1 = new Point(centerPoint.X - outterRadius, centerPoint.Y);
                Point p2 = new Point(centerPoint.X - innerRaius, centerPoint.Y);
                for (int i = 0; i < this.SplitCount; i++)
                {
                    Brush brush = IsRandomColor ? GetRandomBrush() : Brushes.White;

                    //Draw Sector
                    Point p3 = RotatePoint(p2, centerPoint, i == 0 ? (Angle + perAngle) : perAngle);
                    Point p4 = RotatePoint(p1, centerPoint, i == 0 ? (Angle + perAngle) : perAngle);
                    var geometry = CreateSectorGeometry(outterArcSize, innererArcSize, p1, p2, p3, p4);
                    dc.DrawGeometry(brush, blackPen, geometry);
                    p1 = p4;
                    p2 = p3;

                    //Draw Digitals
                    double angle = 90 + i * perAngle;
                    if (this.TextsSource != null && this.TextsSource.Count > i)
                    {
                        var textP1 = RotatePoint(p1, centerPoint, -perAngle / 2);
                        var textP2 = RotatePoint(p2, centerPoint, -perAngle / 2);

                        Point textPoint = new Point(centerPoint.X - innerRaius, centerPoint.Y);
                        FormattedText text = new FormattedText(this.TextsSource[i], CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, this.FontSize, Brushes.Black);
                        var textCenterPoint = new Point((textP1.X + textP2.X) / 2 - text.Width / 2, (textP1.Y + textP2.Y) / 2 - text.Height / 2);
                        dc.DrawText(text, textCenterPoint);
                    }

                }
            }

            dc.Close();
        }

        private void DrawChart1()
        {
            DrawingContext dc = _ring.RenderOpen();

            double strokeThickness = 2d;

            var transparentBrush = new SolidColorBrush(Colors.Transparent);
            var blackBrush = new SolidColorBrush(Colors.Black);
            var blackPen = new Pen(blackBrush, strokeThickness);

            double outterRadius = Math.Min(this.ActualWidth / 2, this.ActualHeight / 2);
            double innerRaius = outterRadius / 2;

            //Draw circles
            Point centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            dc.DrawEllipse(transparentBrush, blackPen, centerPoint, outterRadius, outterRadius);
            dc.DrawEllipse(transparentBrush, blackPen, centerPoint, innerRaius, innerRaius);


            //Draw lines
            if (this.SplitCount > 0)
            {
                Typeface typeface = new Typeface("Times New Roman");

                Point originPoint1 = new Point(centerPoint.X - outterRadius, centerPoint.Y);
                Point originPoint2 = new Point(centerPoint.X - innerRaius, centerPoint.Y);

                //dc.DrawLine(blackPen, originPoint1, originPoint2);
                double perAngle = 360 / this.SplitCount;
                for (int i = 0; i < this.SplitCount; i++)
                {
                    double angle = 90 + i * perAngle;
                    Point p1 = RotatePoint(originPoint1, centerPoint, angle);
                    Point p2 = RotatePoint(originPoint2, centerPoint, angle);
                    dc.DrawLine(blackPen, p1, p2);

                    //Draw Digitals
                    if (this.TextsSource != null && this.TextsSource.Count > i)
                    {
                        p1 = RotatePoint(originPoint1, centerPoint, angle + perAngle / 2);
                        p2 = RotatePoint(originPoint2, centerPoint, angle + perAngle / 2);

                        Point textPoint = new Point(centerPoint.X - innerRaius, centerPoint.Y);
                        FormattedText text = new FormattedText(this.TextsSource[i], CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, this.FontSize, Brushes.Black);
                        var textCenterPoint = new Point((p1.X + p2.X) / 2 - text.Width / 2, (p1.Y + p2.Y) / 2 - text.Height / 2);
                        dc.DrawText(text, textCenterPoint);
                    }
                }
            }

            dc.Close();
        }

        /// <summary>
        /// Create a sector
        /// </summary>
        /// <param name="outterArcSize"></param>
        /// <param name="innerArcSize"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private Geometry CreateSectorGeometry(Size outterArcSize, Size innerArcSize, params Point[] points)
        {
            if (points.Length != 4)
                return new StreamGeometry();

            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(points[0], true, true);

                //PointCollection tempPoints = new PointCollection(points.Skip(1));
                //geometryContext.PolyLineTo(tempPoints, true, true);

                geometryContext.LineTo(points[1], true, true);
                geometryContext.ArcTo(points[2], innerArcSize, 0, false, SweepDirection.Clockwise, true, true);
                geometryContext.LineTo(points[3], true, true);
                geometryContext.ArcTo(points[0], outterArcSize, 0, false, SweepDirection.Counterclockwise, true, true);
            }
            return streamGeometry;
        }


        /// <summary>
        /// 顺时针旋转
        /// </summary>
        /// <param name="point"></param>
        /// <param name="centerPoint"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Point RotatePoint(Point point, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);

            double originX = point.X - centerPoint.X;
            double originY = point.Y - centerPoint.Y;

            //[cos -sin]
            //[sin  cos]
            double x = Math.Cos(angleInRadians) * originX - Math.Sin(angleInRadians) * originY;
            double y = Math.Sin(angleInRadians) * originX + Math.Cos(angleInRadians) * originY;

            return new Point(x + centerPoint.X, y + centerPoint.Y);
        }

        /// <summary>
        /// 顺时针旋转(Matrix实现)
        /// </summary>
        public Point RotatePoint1(Point point, Point centerPoint, double angleInRadians)
        {
            double originX = point.X - centerPoint.X;
            double originY = point.Y - centerPoint.Y;

            Vector vector = new Vector(originX, originY);
            Matrix matrix = new Matrix();
            matrix.Rotate(angleInRadians);

            Vector newVector = Vector.Multiply(vector, matrix);

            return new Point(newVector.X + centerPoint.X, newVector.Y + centerPoint.Y);
        }

        #region Random Brushes

        private List<Brush> _brushes;
        private Random _rand = new Random();

        private void InitBrushes()
        {
            _brushes = new List<Brush>();
            var props = typeof(Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var propInfo in props)
            {
                _brushes.Add((Brush)propInfo.GetValue(null, null));
            }
        }
        private Brush GetRandomBrush()
        {
            return _brushes[_rand.Next(_brushes.Count)];
        }
        #endregion
    }
}
