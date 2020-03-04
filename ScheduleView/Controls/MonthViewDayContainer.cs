using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewDayContainer : Control
    {
        private static readonly Brush lineBrush;
        private static readonly Pen linePen;

        static MonthViewDayContainer()
        {
            lineBrush = new SolidColorBrush(Colors.Red);
            lineBrush.Freeze();

            linePen = new Pen(lineBrush, 1);
            linePen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawSnappedRectangle(new Rect(0,0,ActualWidth, ActualHeight), linePen, null);
        }
    }
}
