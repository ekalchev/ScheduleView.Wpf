using NodaTime;
using ScheduleView.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewData
    {
        public MonthViewDay[][] Grid { get; private set; }
        public Size GridCellSize { get; private set; }

        public int ColumnsCount { get; }
        public int RowsCount { get; }
        public int CellsCount { get; }

        public double HeaderHeight = LayoutHelper.RoundLayoutValue(20);

        public MonthViewData()
        {
            ColumnsCount = 7;
            RowsCount = 5;
            CellsCount = RowsCount * ColumnsCount;
        }

        public void Update(Size availableSize, Instant firstVisibleDay, IEnumerable<Occurrence> appointmentItems)
        {
            ColumnWidth = LayoutHelper.RoundLayoutValue(availableSize.Width / ColumnsCount);

            if (DoubleUtil.GreaterThanOrClose(ColumnWidth * ColumnsCount, availableSize.Width) == true)
            {
                ColumnWidth = LayoutHelper.FloorLayoutValue(availableSize.Width / ColumnsCount);
            }

            RowsHeight = LayoutHelper.RoundLayoutValue(availableSize.Height / RowsCount);

            if (DoubleUtil.GreaterThanOrClose(RowsHeight * RowsCount, availableSize.Height) == true)
            {
                RowsHeight = LayoutHelper.FloorLayoutValue(availableSize.Height / RowsCount);
            }

            GridCellSize = new Size(ColumnWidth, RowsHeight);

            Bounds = LayoutHelper.RoundLayoutRect3(new Rect(0, 0, ColumnWidth * ColumnsCount, RowsHeight * RowsCount));

            double columnOffset = 0;
            Grid = new MonthViewDay[RowsCount][];
            Instant currentDay = firstVisibleDay;

            for (int rowIndex = 0; rowIndex < RowsCount; rowIndex++)
            {
                columnOffset = 0;
                Grid[rowIndex] = new MonthViewDay[ColumnsCount];

                for (int columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
                {
                    // ColumnWidth and RowHeight should be already layout rounded - so no need to round the rect bounds
                    var day = new MonthViewDay();
                    day.GridCell = new Rect(columnIndex * ColumnWidth, rowIndex * RowsHeight, ColumnWidth, RowsHeight);
                    var nextDay = currentDay.Plus(NodaTime.Duration.FromDays(1));
                    day.Day = new Interval(currentDay, nextDay); // may be we should use 23:59:99999 as end interval?????
                    currentDay = nextDay;
                    Grid[rowIndex][columnIndex] = day;
                }

                columnOffset += ColumnWidth;
            }
        }

        public IEnumerable<MonthViewDay> GridDays
        {
            get
            {
                for (int rowIndex = 0; rowIndex < RowsCount; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
                    {
                        yield return Grid[rowIndex][columnIndex];
                    }
                }
            }
        }

        public Rect Bounds { get; private set; }

        public double ColumnWidth { get; private set; }
        public double RowsHeight { get; private set; }
    }
}
