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
            this.Loaded += CrossHair_Loaded;
        }

        private void CrossHair_Loaded(object sender, System.Windows.RoutedEventArgs e)
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



        #endregion


        private void AddCrossHair()
        {


            _horizontalLine = new Line() { Stroke = this.HorizontalLineStroke };
            _verticalLine = new Line() { Stroke = this.VerticalLineStroke };
            _circle = new Ellipse() { Stroke = this.CircleStroke, Width = CircleRadius, Height = CircleRadius };

            _text = new TextBlock() { Foreground = Brushes.Yellow };

            this.Children.Add(_horizontalLine);
            this.Children.Add(_verticalLine);
            this.Children.Add(_circle);
            this.Children.Add(_text);
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

            Canvas.SetTop(_circle, _currentMousePoint.Y - CircleRadius / 2);
            Canvas.SetLeft(_circle, _currentMousePoint.X - CircleRadius / 2);


            _text.Text = $"({_currentMousePoint.X}, {_currentMousePoint.Y})";
            Canvas.SetTop(_text, _currentMousePoint.Y - 32);
            Canvas.SetLeft(_text, _currentMousePoint.X + 20);

        }
    }
}
