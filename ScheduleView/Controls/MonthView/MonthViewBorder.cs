using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls.MonthView
{
    internal class MonthViewBorder : Control
    {
        private static readonly Brush lineBrush;
        private static readonly Pen linePen;
        private static readonly Brush fillBrush;
        private static readonly Brush selectedBrush;

        static MonthViewBorder()
        {
            BackgroundProperty.OverrideMetadata(typeof(MonthViewBorder), new FrameworkPropertyMetadata((Brush)null, FrameworkPropertyMetadataOptions.AffectsRender));

            fillBrush = new SolidColorBrush(Colors.White);
            fillBrush.Freeze();

            selectedBrush = new SolidColorBrush(Colors.Blue);
            selectedBrush.Freeze();

            lineBrush = new SolidColorBrush(Colors.Red);
            lineBrush.Freeze();

            linePen = new Pen(lineBrush, 1);
            linePen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawSnappedRectangle(new Rect(0, 0, ActualWidth, ActualHeight), linePen, Background);
        }
    }
}
