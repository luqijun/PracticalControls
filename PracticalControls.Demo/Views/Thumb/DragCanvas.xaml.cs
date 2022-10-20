using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PracticalControls.Demo.Views
{
    /// <summary>
    /// Interaction logic for MyThumb.xaml
    /// </summary>
    public partial class DragCanvas : UserControl
    {
        public DragCanvas()
        {
            InitializeComponent();
        }

        void onDragStarted(object sender, DragStartedEventArgs e)
        {
            myThumb.Background = Brushes.Orange;
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = myCanvasStretch.Height + e.VerticalChange;
            double xadjust = myCanvasStretch.Width + e.HorizontalChange;
            if ((xadjust >= 0) && (yadjust >= 0))
            {
                myCanvasStretch.Width = xadjust;
                myCanvasStretch.Height = yadjust;
                Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) +
                e.HorizontalChange);
                Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) +
                                        e.VerticalChange);
                changes.Text = "Size: " +
                                myCanvasStretch.Width.ToString() +
                                 ", " +
                                myCanvasStretch.Height.ToString();
            }
        }

        void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            myThumb.Background = Brushes.Blue;
        }
    }
}
