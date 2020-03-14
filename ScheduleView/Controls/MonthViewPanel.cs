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

namespace ScheduleView.Wpf.Controls
{
    internal partial class MonthViewPanel : Panel, IAnimationScrollObservable
    {
        private MonthViewDayContainer[] monthViewDayItems = null;
        private Size lastMeasureSize;
        private const int ColumnsCount = 7;
        private const int RowsCount = 5;
        public MonthViewDay[][] ActiveCells { get; private set; }
        private ScrollDirection scrollDirection;
        private List<Cell> ArrangeData;

        public MonthViewPanel()
        {
            MouseMove += MonthViewPanel_MouseMove;
            MouseLeftButtonDown += MonthViewPanel_MouseLeftButtonDown;
            MouseLeftButtonUp += MonthViewPanel_MouseLeftButtonUp;

            ActiveCells = new MonthViewDay[RowsCount][];

            for (int rowIndex = 0; rowIndex < ActiveCells.Length; rowIndex++)
            {
                ActiveCells[rowIndex] = new MonthViewDay[ColumnsCount];
            }

            ArrangeData = new List<Cell>((RowsCount * ColumnsCount) + 1);
        }

        internal ScheduleView ScheduleView { get; set; }
        private MonthViewData MonthViewData => ScheduleView.MonthsViewData;

        private Size CalculateGridCell(Size availableSize)
        {
            var ColumnWidth = LayoutHelper.RoundLayoutValue(availableSize.Width / ColumnsCount);

            if (DoubleUtil.GreaterThanOrClose(ColumnWidth * ColumnsCount, availableSize.Width) == true)
            {
                ColumnWidth = LayoutHelper.FloorLayoutValue(availableSize.Width / ColumnsCount);
            }

            var RowsHeight = LayoutHelper.RoundLayoutValue(availableSize.Height / RowsCount);

            if (DoubleUtil.GreaterThanOrClose(RowsHeight * RowsCount, availableSize.Height) == true)
            {
                RowsHeight = LayoutHelper.FloorLayoutValue(availableSize.Height / RowsCount);
            }

            return new Size(ColumnWidth, RowsHeight);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (lastMeasureSize != availableSize)
            {
                ScheduleView.GridCellSize = CalculateGridCell(availableSize);

                lastMeasureSize = availableSize;

                if (monthViewDayItems == null)
                {
                    var monthViewDayItemCount = MonthViewData.ColumnsCount * MonthViewData.RowsCount + ColumnsCount; // + one extra row of containers
                    monthViewDayItems = new MonthViewDayContainer[monthViewDayItemCount];

                    for (int cellIndex = 0; cellIndex < monthViewDayItems.Length; cellIndex++)
                    {
                        var currentItem = new MonthViewDayContainer();
                        monthViewDayItems[cellIndex] = currentItem;
                        Children.Add(currentItem);
                    }
                }
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double columnOffset = 0;
            double columnWidth = ScheduleView.GridCellSize.Width;
            double rowHeight = ScheduleView.GridCellSize.Height;

            Instant currentDay = ScheduleView.MonthViewStartDate;
            Rect arrangeRect;
            MonthViewDayContainer container;
            int containerIndex = 0;

            for (int rowIndex = 0; rowIndex < RowsCount; rowIndex++)
            {
                columnOffset = 0;

                for (int columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
                {
                    arrangeRect = new Rect(columnIndex * columnWidth, rowIndex * rowHeight, columnWidth, rowHeight);
                    container = monthViewDayItems[containerIndex++];
                    container.Arrange(arrangeRect);
                }

                columnOffset += ScheduleView.GridCellSize.Width;
            }

            // arrange extra row above or below for the animation scrolling
            if (scrollDirection != ScrollDirection.None)
            {
                int topCoef = scrollDirection == ScrollDirection.Up ? RowsCount : -1;
                for (int columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
                {
                    arrangeRect = new Rect(columnIndex * columnWidth, topCoef * rowHeight, columnWidth, rowHeight);
                    container = monthViewDayItems[containerIndex++];
                    container.Arrange(new Rect(arrangeRect.Left, arrangeRect.Top, arrangeRect.Width, arrangeRect.Height));
                }
            }

            //for(int index = containerIndex; index < monthViewDayItems.Length; index++)
            //{
            //    monthViewDayItems[index].Visibility = Visibility.Hidden; // hide unused containers
            //}

            return finalSize;
        }

        public void NotifyScrollAnimationStarted(double scrollOffset, ScrollDirection direction)
        {
            scrollDirection = direction;

            InvalidateArrange();
            UpdateLayout();
        }

        public void NotifyScrollAnimationCompleted()
        {
            scrollDirection = ScrollDirection.None;

            InvalidateArrange();
            UpdateLayout();
        }
    }
}
