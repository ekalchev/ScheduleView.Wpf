using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ScheduleView.Wpf.Controls
{
    public class MonthsViewAppointmentsPanel : Panel
    {
        private List<AppointmentItem> appointmentItems = new List<AppointmentItem>();
        internal ScheduleView ScheduleView { get; set; }

        private MonthViewData Data => ScheduleView.MonthsViewData;

        public MonthsViewAppointmentsPanel()
        {
            for (int i = 0; i < 500; i++)
            {
                var appointmentItem = new AppointmentItem();
                appointmentItems.Add(appointmentItem);
                Children.Add(appointmentItem);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rowsCount = Data.RowsCount;
            var columnsCount = Data.ColumnsCount;
            int currentAppointmentItemIndex = 0;

            List<UIElement> appointmentItemsInUse = new List<UIElement>();

            for (int row = 0; row < rowsCount; row++)
            {
                for (int column = 0; column < columnsCount; column++)
                {
                    Rect arrangeRect = new Rect(column * Data.ColumnWidth, row * Data.RowsHeight, Data.ColumnWidth, Data.RowsHeight);

                    double currentHeight = Data.HeaderHeight;

                    while(currentHeight + 20 < arrangeRect.Height)
                    {
                        var appointmentItem = Children[currentAppointmentItemIndex++];
                        appointmentItem.Visibility = Visibility.Visible;

                        appointmentItem.Arrange(new Rect(arrangeRect.Left, arrangeRect.Top + currentHeight, arrangeRect.Width - 15, 20));
                        appointmentItemsInUse.Add(appointmentItem);

                        currentHeight += 20;
                    }
                }
            }

            foreach(var notUsedAppointmentItem in appointmentItems.Except(appointmentItemsInUse))
            {
                notUsedAppointmentItem.Visibility = Visibility.Collapsed;
            }

            return finalSize;
        }
    }
}
