using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewGrid : Control
    {
        private static readonly Brush lineBrush;
        private static readonly Pen linePen;
        private DpiScale dpiScale;
        private static readonly CultureInfo culture = CultureInfo.GetCultureInfo("en-us");
        private static readonly Typeface segoeTypeface = new Typeface("Segoe UI");

        internal ScheduleView ScheduleView { get; set; }
        private MonthViewData Data => ScheduleView.MonthsViewData;

        static MonthViewGrid()
        {
            lineBrush = new SolidColorBrush(Colors.Red);
            lineBrush.Freeze();

            linePen = new Pen(lineBrush, 1);
            linePen.Freeze();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            dpiScale = VisualTreeHelper.GetDpi(this);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double columnOffset = 0;
            for (int i = 0; i < Data.ColumnsCount; i++)
            {
                drawingContext.DrawVerticalSnappedLine(
                    new Point(Data.Bounds.TopLeft.X + columnOffset, Data.Bounds.TopLeft.Y),
                    new Point(Data.Bounds.BottomLeft.X + columnOffset, Data.Bounds.BottomLeft.Y),
                    linePen);

                columnOffset += Data.ColumnWidth;
            }

            drawingContext.DrawVerticalSnappedLine(
                    new Point(Data.Bounds.TopLeft.X + columnOffset, Data.Bounds.TopLeft.Y),
                    new Point(Data.Bounds.BottomLeft.X + columnOffset, Data.Bounds.BottomLeft.Y),
                    linePen);

            double rowOffset = 0;
            for (int i = 0; i < Data.RowsCount; i++)
            {
                drawingContext.DrawHorizontalSnappedLine(
                    new Point(Data.Bounds.TopLeft.X, Data.Bounds.TopLeft.Y + rowOffset),
                    new Point(Data.Bounds.TopRight.X, Data.Bounds.TopRight.Y + rowOffset),
                    linePen);

                rowOffset += Data.RowsHeight;
            }

            drawingContext.DrawHorizontalSnappedLine(
                new Point(Data.Bounds.TopLeft.X, Data.Bounds.TopLeft.Y + rowOffset),
                new Point(Data.Bounds.TopRight.X, Data.Bounds.TopRight.Y + rowOffset),
                linePen);

            
            columnOffset = 0;
            double textOffset = 5;
            for (int i = 0; i < Data.ColumnsCount; i++)
            {
                rowOffset = 0;

                for (int j = 0; j < Data.RowsCount; j++)
                {
                    FormattedText formattedText = new FormattedText((i + j).ToString(),
                        culture,
                        this.FlowDirection,
                        segoeTypeface,
                        13,
                        Brushes.Black,
                        dpiScale.PixelsPerDip);

                    drawingContext.DrawText(formattedText, new Point(columnOffset + textOffset, rowOffset));
                    rowOffset += Data.RowsHeight;
                }

                columnOffset += Data.ColumnWidth;
            }
        }
    }
}
