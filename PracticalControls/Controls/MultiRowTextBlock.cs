using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PracticalControls.Controls
{
    public class MultiRowTextBlock : SelectableTextBlock
    {
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(MultiRowTextBlock), new PropertyMetadata(null, ItemsSourcePropertyChanged));

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiRowTextBlock btb = d as MultiRowTextBlock;
            IList<TextBlockRow> rows = e.NewValue as IList<TextBlockRow>;
            if (rows != null)
            {
                btb.ShowContent(rows);

                if (rows is System.Collections.Specialized.INotifyCollectionChanged nc)
                {
                    nc.CollectionChanged += (sender, e) => btb.ShowContent(btb.ItemsSource as IList<TextBlockRow>);
                }
            }
        }

        public Brush RowHeaderForegroud
        {
            get { return (Brush)GetValue(RowHeaderForegroudProperty); }
            set { SetValue(RowHeaderForegroudProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderForegroudProperty =
            DependencyProperty.Register("RowHeaderForegroud", typeof(Brush), typeof(MultiRowTextBlock), new PropertyMetadata(null));



        public Brush RowContentBackground
        {
            get { return (Brush)GetValue(RowContentBackgroundProperty); }
            set { SetValue(RowContentBackgroundProperty, value); }
        }

        public static readonly DependencyProperty RowContentBackgroundProperty =
            DependencyProperty.Register("RowContentBackground", typeof(Brush), typeof(MultiRowTextBlock), new PropertyMetadata(null));



        private void ShowContent(IList<TextBlockRow> contentRows)
        {
            this.Inlines.Clear();
            foreach (var row in contentRows)
            {
                Run run;

                //rowHeader
                run = new Run($"{row.Header}");
                if (this.RowHeaderForegroud != null)
                    run.Foreground = RowHeaderForegroud;
                this.Inlines.Add(run);

                //blank space
                this.Inlines.Add("     ");

                //content
                run = new Run($"{row.Content }");
                if (this.RowContentBackground != null)
                    run.Background = RowContentBackground;
                this.Inlines.Add(run);

                //fill dots
                string dots = "·";
                for (int i = 0; i < 100; i++)
                    dots += "  ·";
                run = new Run($"  {dots}\r\n");
                run.BaselineAlignment = BaselineAlignment.TextTop;
                this.Inlines.Add(run);

            }
        }


        #region Selectable

        //TextPointer StartSelectPosition;
        //TextPointer EndSelectPosition;
        //public string SelectedText = "";

        //public delegate void TextSelectedHandler(string SelectedText);
        //public event TextSelectedHandler TextSelected;

        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    Point mouseDownPoint = e.GetPosition(this);
        //    StartSelectPosition = this.GetPositionFromPoint(mouseDownPoint, true);
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);

        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        this.Cursor = Cursors.IBeam;
        //        Point mouseUpPoint = e.GetPosition(this);
        //        EndSelectPosition = this.GetPositionFromPoint(mouseUpPoint, true);

        //        TextRange otr = new TextRange(this.ContentStart, this.ContentEnd);
        //        otr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));

        //        TextRange ntr = new TextRange(StartSelectPosition, EndSelectPosition);
        //        ntr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.LightBlue));

        //        SelectedText = ntr.Text;
        //        if (!(TextSelected == null))
        //        {
        //            TextSelected(SelectedText);
        //        }
        //    }
        //}

        //protected override void OnMouseUp(MouseButtonEventArgs e)
        //{
        //    base.OnMouseUp(e);
        //}

        #endregion
    }
}
