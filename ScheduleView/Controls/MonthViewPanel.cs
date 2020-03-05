using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewPanel : Panel
    {
        MonthViewDayContainer[] monthViewDayItems = null;
        Size lastMeasureSize;

        public MonthViewPanel()
        {
        }

        internal ScheduleView ScheduleView { get; set; }
        private MonthViewData MonthViewData => ScheduleView.MonthsViewData;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (monthViewDayItems == null)
            {
                var monthViewDayItemCount = MonthViewData.ColumnsCount * MonthViewData.RowsCount;
                monthViewDayItems = new MonthViewDayContainer[monthViewDayItemCount];

                for (int cellIndex = 0; cellIndex < MonthViewData.CellsCount; cellIndex++)
                {
                    var currentItem = new MonthViewDayContainer();
                    monthViewDayItems[cellIndex] = currentItem;
                    Children.Add(currentItem);
                }
            }
            
            if(lastMeasureSize != availableSize)
            {
                lastMeasureSize = availableSize;
                for (int cellIndex = 0; cellIndex < MonthViewData.CellsCount; cellIndex++)
                {
                    monthViewDayItems[cellIndex].Measure(MonthViewData.GridCellSize);
                }
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int currentItemIndex = 0;
            foreach(var day in MonthViewData.GridDays)
            {
                var currentItem = monthViewDayItems[currentItemIndex++];
                currentItem.Arrange(day.GridCell);
            }

            return finalSize;
        }
    }
}
