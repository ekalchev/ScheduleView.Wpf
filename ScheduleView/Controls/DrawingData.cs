using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScheduleView.Wpf.Controls
{
    internal class DrawingData
    {
        public int ColumnsCount { get; } = 6;
        public int RowsCount { get; } = 5;

        public DrawingData()
        {
        }

        public void Update(Size availableSize)
        {
            ColumnWidth = LayoutHelper.RoundLayoutValue(availableSize.Width / ColumnsCount);
            RowsHeight = LayoutHelper.RoundLayoutValue(availableSize.Height / RowsCount);
            Bounds = new Rect(0, 0, ColumnWidth * ColumnsCount, RowsHeight * RowsCount);
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

        public Rect Bounds { get; private set; }

        public double ColumnWidth { get; private set; }
        public double RowsHeight { get; private set; }
    }
}