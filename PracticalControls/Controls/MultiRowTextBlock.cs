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
    public class MultiRowTextBlock : TextBlock
    {
        //TextPointer StartSelectPosition;
        //TextPointer EndSelectPosition;
        //public String SelectedText = "";

        //public delegate void TextSelectedHandler(string SelectedText);
        //public event TextSelectedHandler TextSelected;

        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    Point mouseDownPoint = e.GetPosition(this);
        //    StartSelectPosition = this.GetPositionFromPoint(mouseDownPoint, true);
        //}

        //protected override void OnMouseUp(MouseButtonEventArgs e)
        //{
        //    base.OnMouseUp(e);
        //    Point mouseUpPoint = e.GetPosition(this);
        //    EndSelectPosition = this.GetPositionFromPoint(mouseUpPoint, true);

        //    TextRange otr = new TextRange(this.ContentStart, this.ContentEnd);
        //    otr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.GreenYellow));

        //    TextRange ntr = new TextRange(StartSelectPosition, EndSelectPosition);
        //    ntr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));

        //    SelectedText = ntr.Text;
        //    if (!(TextSelected == null))
        //    {
        //        TextSelected(SelectedText);
        //    }
        //}

        static MultiRowTextBlock()
        {
            FocusableProperty.OverrideMetadata(typeof(MultiRowTextBlock), new FrameworkPropertyMetadata(true));
            TextEditorWrapper.RegisterCommandHandlers(typeof(MultiRowTextBlock), true, true, true);

            // remove the focus rectangle around the control
            FocusVisualStyleProperty.OverrideMetadata(typeof(MultiRowTextBlock), new FrameworkPropertyMetadata((object)null));
        }

        private readonly TextEditorWrapper _editor;

        public MultiRowTextBlock()
        {
            _editor = TextEditorWrapper.Create(this);
        }


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
            }
        }

        private void ShowContent(IList<TextBlockRow> contentRows)
        {
            this.Inlines.Clear();
            foreach (var row in contentRows)
            {
                this.Inlines.Add($"{row.Header} {row.Content}\r\n");
            }
        }
    }


    class TextEditorWrapper
    {
        private static readonly Type TextEditorType = Type.GetType("System.Windows.Documents.TextEditor, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        private static readonly PropertyInfo IsReadOnlyProp = TextEditorType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo TextViewProp = TextEditorType.GetProperty("TextView", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo RegisterMethod = TextEditorType.GetMethod("RegisterCommandHandlers",
            BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Type), typeof(bool), typeof(bool), typeof(bool) }, null);

        private static readonly Type TextContainerType = Type.GetType("System.Windows.Documents.ITextContainer, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        private static readonly PropertyInfo TextContainerTextViewProp = TextContainerType.GetProperty("TextView");

        private static readonly PropertyInfo TextContainerProp = typeof(TextBlock).GetProperty("TextContainer", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void RegisterCommandHandlers(Type controlType, bool acceptsRichContent, bool readOnly, bool registerEventListeners)
        {
            RegisterMethod.Invoke(null, new object[] { controlType, acceptsRichContent, readOnly, registerEventListeners });
        }

        public static TextEditorWrapper Create(TextBlock tb)
        {
            var textContainer = TextContainerProp.GetValue(tb);

            var editor = new TextEditorWrapper(textContainer, tb, false);
            IsReadOnlyProp.SetValue(editor._editor, true);
            TextViewProp.SetValue(editor._editor, TextContainerTextViewProp.GetValue(textContainer));

            return editor;
        }

        private readonly object _editor;

        public TextEditorWrapper(object textContainer, FrameworkElement uiScope, bool isUndoEnabled)
        {
            _editor = Activator.CreateInstance(TextEditorType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null, new[] { textContainer, uiScope, isUndoEnabled }, null);
        }
    }
}
