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
        public Rect[][] Cells { get; private set; }

        public int ColumnsCount { get; } = 6;
        public int RowsCount { get; } = 5;
        public double HeaderHeight = LayoutHelper.RoundLayoutValue(20);

        public void Update(Size availableSize)
        {
            ColumnWidth = LayoutHelper.RoundLayoutValue(availableSize.Width / ColumnsCount);
            RowsHeight = LayoutHelper.RoundLayoutValue(availableSize.Height / RowsCount);
            Bounds = LayoutHelper.RoundLayoutRect2(new Rect(0, 0, ColumnWidth * ColumnsCount, RowsHeight * RowsCount));

            CalculateCells();
        }

        private double[] CalculateOffset(int count, double offset)
        {
            var offsets = new double[count];
            double nextOffset = 0d;
            for (int i = 0; i < count; i++)
            {
                offsets[i] = nextOffset;
                nextOffset += offset;
            }

            return offsets;
        }

        private void CalculateCells()
        {
            var rowsCount = RowsCount;
            var columnsCount = ColumnsCount;

            Cells = new Rect[rowsCount][];

            for (int row = 0; row < rowsCount; row++)
            {
                Cells[row] = new Rect[columnsCount];

                for (int column = 0; column < columnsCount; column++)
                {
                    Cells[row][column] = new Rect(column * ColumnWidth, row * RowsHeight, column + 1 * ColumnWidth, row + 1 * RowsHeight);
                }
            }
        }

        public Rect Bounds { get; private set; }

        public double ColumnWidth { get; private set; }
        public double RowsHeight { get; private set; }
    }
}
