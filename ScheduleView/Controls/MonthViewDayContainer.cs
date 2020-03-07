using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewDayContainer : Control
    {
        private static readonly Brush lineBrush;
        private static readonly Pen linePen;
        private static readonly Brush fillBrush;
        private static readonly Brush selectedBrush;

        private static readonly Style style;

        static MonthViewDayContainer()
        {
            BackgroundProperty.OverrideMetadata(typeof(MonthViewDayContainer), new FrameworkPropertyMetadata((Brush)null, FrameworkPropertyMetadataOptions.AffectsRender));

            fillBrush = new SolidColorBrush(Colors.White);
            fillBrush.Freeze();

            selectedBrush = new SolidColorBrush(Colors.Blue);
            selectedBrush.Freeze();

            lineBrush = new SolidColorBrush(Colors.Red);
            lineBrush.Freeze();

            linePen = new Pen(lineBrush, 1);
            linePen.Freeze();

            style = new Style(typeof(MonthViewDayContainer));
            style.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = fillBrush,
            });

            Trigger trigger = new Trigger()
            {
                Property = IsSelectedProperty,
                Value = true
            };

            trigger.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = selectedBrush
            });

            style.Triggers.Add(trigger);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MonthViewDayContainer), new PropertyMetadata(false));

        public MonthViewDayContainer()
        {
            Style = style;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawSnappedRectangle(new Rect(0,0,ActualWidth, ActualHeight), linePen, Background);
        }
    }
}
