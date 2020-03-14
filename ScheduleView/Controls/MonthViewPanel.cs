using NodaTime;
using ScheduleView.Wpf.Controls.MonthView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private MonthViewContainerCache containerCache;

        private Size lastMeasureSize;
        private const int ColumnsCount = 7;
        private const int RowsCount = 5;
        public MonthViewDay[][] ActiveCells { get; private set; }
        private ScrollDirection scrollDirection;
        private int arrangeCounter;

        public MonthViewPanel()
        {
            MouseMove += MonthViewPanel_MouseMove;
            MouseLeftButtonDown += MonthViewPanel_MouseLeftButtonDown;
            MouseLeftButtonUp += MonthViewPanel_MouseLeftButtonUp;
        }

        private void ContainerCache_NewContainerCreated(object sender, MonthViewDayContainer e)
        {
            Children.Add(e);
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
            }

            lastMeasureSize = availableSize;

            if (containerCache == null)
            {
                var totalContainersNeeded = MonthViewData.ColumnsCount * MonthViewData.RowsCount + ColumnsCount; // we add one additional row that is used for animated scrolling
                monthViewDayItems = new MonthViewDayContainer[totalContainersNeeded];
                containerCache = new MonthViewContainerCache(totalContainersNeeded);
                containerCache.NewContainerCreated += ContainerCache_NewContainerCreated;
                containerCache.EnsureContainersCache();
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine("Arrange " + arrangeCounter++);
            containerCache.Reset();

            double columnOffset = 0;
            double columnWidth = ScheduleView.GridCellSize.Width;
            double rowHeight = ScheduleView.GridCellSize.Height;

            Rect arrangeRect;
            MonthViewDayContainer container;
            int containerIndex = 0;
            Instant currentDay = ScheduleView.MonthViewStartDate;

            for (int rowIndex = 0; rowIndex < RowsCount; rowIndex++)
            {
                columnOffset = 0;

                for (int columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
                {
                    arrangeRect = new Rect(columnIndex * columnWidth, rowIndex * rowHeight, columnWidth, rowHeight);
                    container = containerCache.GetContainer(currentDay.InUtc().Day);
                    currentDay = currentDay.Plus(NodaTime.Duration.FromDays(1));

                    monthViewDayItems[containerIndex++] = container;
                    container.Arrange(arrangeRect);
                }

                columnOffset += ScheduleView.GridCellSize.Width;
            }

            // arrange extra row above or below for the animation scrolling
            if (scrollDirection != ScrollDirection.None)
            {
                int topCoef = scrollDirection == ScrollDirection.Up ? RowsCount : -1;

                if (scrollDirection == ScrollDirection.Up)
                {
                    currentDay = ScheduleView.MonthViewStartDate.Plus(NodaTime.Duration.FromDays(RowsCount * ColumnsCount));
                }
                else if (scrollDirection == ScrollDirection.Down)
                {
                    currentDay = ScheduleView.MonthViewStartDate.Minus(NodaTime.Duration.FromDays(ColumnsCount));
                }

                for (int columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
                {
                    arrangeRect = new Rect(columnIndex * columnWidth, topCoef * rowHeight, columnWidth, rowHeight);
                    container = containerCache.GetContainer(currentDay.InUtc().Day);
                    currentDay = currentDay.Plus(NodaTime.Duration.FromDays(1));
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
