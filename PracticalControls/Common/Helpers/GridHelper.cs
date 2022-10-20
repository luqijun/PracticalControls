using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace PracticalControls.Common.Helpers
{
    public class GridHelper
    {


        public static Brush GetBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BorderBrushProperty);
        }

        public static void SetBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(GridHelper), new PropertyMetadata(new SolidColorBrush(Colors.Black)));




        public static bool GetShowBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowBorderProperty);
        }
        public static void SetShowBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowBorderProperty, value);
        }

        /// <summary>
        /// 是否显示边框
        /// </summary>
        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridHelper), new PropertyMetadata(false, OnShowBorderChanged));


        private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Grid grid)) return;

            grid.Loaded += OnLoaded;
        }


        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is Grid grid)) return;

            var rowCount = grid.RowDefinitions.Count;
            var columnCount = grid.ColumnDefinitions.Count;
            var thickness = new Thickness(1);
            var bottomThickness = new Thickness(0, 0, 0, 1);
            var rightThickness = new Thickness(0, 0, 1, 0);
            var headerBack = new SolidColorBrush(Color.FromArgb(255, 129, 133, 145));

            var borderBursh = GridHelper.GetBorderBrush(grid);
            for (int i = 0; i < rowCount; i++)
            {
                Border border = new Border()
                {
                    BorderBrush = borderBursh,
                    BorderThickness = i == 0 ? thickness : bottomThickness,
                    //Background = i == 0 ? headerBack : Brushes.Transparent,
                };
                border.SetValue(Panel.ZIndexProperty, -1000);
                border.SetValue(Grid.RowProperty, i);
                border.SetValue(Grid.ColumnProperty, 0);
                border.SetValue(Grid.ColumnSpanProperty, columnCount);
                grid.Children.Add(border);
            }
            for (int i = 0; i < columnCount; i++)
            {
                Border border = new Border()
                {
                    BorderBrush = borderBursh,
                    BorderThickness = i == 0 ? thickness : rightThickness,
                    Background = Brushes.Transparent,
                };
                border.SetValue(Panel.ZIndexProperty, -1000);
                border.SetValue(Grid.RowProperty, 0);
                border.SetValue(Grid.ColumnProperty, i);
                border.SetValue(Grid.RowSpanProperty, rowCount);
                grid.Children.Add(border);
            }
            grid.Loaded -= OnLoaded;
        }

    }
}
