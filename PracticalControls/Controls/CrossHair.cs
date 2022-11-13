using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PracticalControls.Controls
{
    public class CrossHair : Canvas
    {
        Line _horizontalLine;
        Line _verticalLine;
        Ellipse _circle;
        TextBlock _text;

        const double CircleRadius = 40;

        private Point _currentMousePoint;

        static CrossHair()
        {
            BackgroundProperty.OverrideMetadata(typeof(CrossHair), new FrameworkPropertyMetadata(Brushes.Transparent));
        }

        public CrossHair()
        {

        }

        protected override void OnInitialized(EventArgs e)
        {
            AddCrossHair();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateCursorCrosshair();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _currentMousePoint = e.GetPosition(this);
            UpdateCursorCrosshair();
        }


        #region Custom Dependency Properties


        public Brush HorizontalLineStroke
        {
            get { return (Brush)GetValue(HorizontalLineStrokeProperty); }
            set { SetValue(HorizontalLineStrokeProperty, value); }
        }

        public static readonly DependencyProperty HorizontalLineStrokeProperty =
            DependencyProperty.Register("HorizontalLineStroke", typeof(Brush), typeof(CrossHair), new PropertyMetadata(Brushes.Green));


        public Brush VerticalLineStroke
        {
            get { return (Brush)GetValue(VerticalLineStrokeProperty); }
            set { SetValue(VerticalLineStrokeProperty, value); }
        }

        public static readonly DependencyProperty VerticalLineStrokeProperty =
            DependencyProperty.Register("VerticalLineStroke", typeof(Brush), typeof(CrossHair), new PropertyMetadata(Brushes.Blue));


        public Brush CircleStroke
        {
            get { return (Brush)GetValue(CircleStrokeProperty); }
            set { SetValue(CircleStrokeProperty, value); }
        }

        public static readonly DependencyProperty CircleStrokeProperty =
            DependencyProperty.Register("CircleStroke", typeof(Brush), typeof(CrossHair), new PropertyMetadata(Brushes.Red));




        public bool ShowCoordinates
        {
            get { return (bool)GetValue(ShowCoordinatesProperty); }
            set { SetValue(ShowCoordinatesProperty, value); }
        }

        public static readonly DependencyProperty ShowCoordinatesProperty =
            DependencyProperty.Register("ShowCoordinates", typeof(bool), typeof(CrossHair), new PropertyMetadata(true));


        public bool ShowCircle
        {
            get { return (bool)GetValue(ShowCircleProperty); }
            set { SetValue(ShowCircleProperty, value); }
        }

        public static readonly DependencyProperty ShowCircleProperty =
            DependencyProperty.Register("ShowCircle", typeof(bool), typeof(CrossHair), new PropertyMetadata(true));


        #endregion


        private void AddCrossHair()
        {

            _horizontalLine = new Line() { Stroke = this.HorizontalLineStroke, StrokeThickness = 2 };
            _verticalLine = new Line() { Stroke = this.VerticalLineStroke, StrokeThickness = 2 };
            this.Children.Add(_horizontalLine);
            this.Children.Add(_verticalLine);

            if (this.ShowCircle)
            {
                _circle = new Ellipse() { Stroke = this.CircleStroke, Width = CircleRadius, Height = CircleRadius };
                this.Children.Add(_circle);
            }

            if (this.ShowCoordinates)
            {
                _text = new TextBlock() { Foreground = Brushes.Yellow };
                this.Children.Add(_text);
            }

            _currentMousePoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            UpdateCursorCrosshair();
        }

        private void UpdateCursorCrosshair()
        {
            if (!this.IsLoaded)
                return;

            //set horizontalLine postion
            _horizontalLine.X1 = 0;
            _horizontalLine.Y1 = _currentMousePoint.Y;
            _horizontalLine.X2 = this.ActualWidth;
            _horizontalLine.Y2 = _currentMousePoint.Y;

            //set verticalLine postion
            _verticalLine.X1 = _currentMousePoint.X;
            _verticalLine.Y1 = 0;
            _verticalLine.X2 = _currentMousePoint.X;
            _verticalLine.Y2 = this.ActualHeight;

            if (_circle != null)
            {
                Canvas.SetTop(_circle, _currentMousePoint.Y - CircleRadius / 2);
                Canvas.SetLeft(_circle, _currentMousePoint.X - CircleRadius / 2);
            }

            if (_text != null)
            {
                _text.Text = $"({_currentMousePoint.X}, {_currentMousePoint.Y})";
                Canvas.SetTop(_text, _currentMousePoint.Y - 32);
                Canvas.SetLeft(_text, _currentMousePoint.X + 20);
            }
        }
    }
}
