using ScheduleView.Wpf.Data;
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
    internal class MonthsViewAppointmentsPanel : Panel
    {
        private Stack<AppointmentItem> appointmentItemsCache = new Stack<AppointmentItem>();
        private Dictionary<Occurrence, OccurrenceContainer> occurenceContainerMap = new Dictionary<Occurrence, OccurrenceContainer>();

        public IEnumerable<Occurrence> Occurrences { get; set; }
        public MonthViewData Data { get; set; }

        public MonthsViewAppointmentsPanel()
        {
            for (int i = 0; i < 500; i++)
            {
                var appointmentItem = new AppointmentItem();
                appointmentItemsCache.Push(appointmentItem);
                Children.Add(appointmentItem);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // occurrence map that will be used in this measure
            var newOccurenceContainerMap = new Dictionary<Occurrence, OccurrenceContainer>();

            // try to match occurrence with container from the last measure and re-use it
            foreach (var occurrence in Occurrences)
            {
                OccurrenceContainer occurrenceContainer = null;

                if (occurenceContainerMap.ContainsKey(occurrence) == true)
                {
                    occurrenceContainer = occurenceContainerMap[occurrence];
                    occurenceContainerMap.Remove(occurrence);
                }
                else
                {
                    var container = appointmentItemsCache.Count > 0 ? appointmentItemsCache.Pop() : new AppointmentItem();

                    occurrenceContainer = new OccurrenceContainer()
                    {
                        Occurrence = occurrence,
                        Container = container
                    };
                }

                newOccurenceContainerMap.Add(occurrence, occurrenceContainer);
            }

            // move unmatched containers back to cache
            foreach (var occurrenceContainer in occurenceContainerMap.Values)
            {
                appointmentItemsCache.Push(occurrenceContainer.Container);
            }

            occurenceContainerMap = newOccurenceContainerMap;

            // TODO: this loop can be avoided if measure is done above
            foreach (var occurenceContainer in occurenceContainerMap.Values)
            {
                occurenceContainer.Container.Measure(Data.GridCellSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Dictionary<Rect, List<AppointmentItem>> rectGroups = new Dictionary<Rect, List<AppointmentItem>>();

            // group appointmentItems in groups by arrange rects 
            foreach (var occurrenceContainer in occurenceContainerMap.Values)
            {
                var rect = Data.Intersect(occurrenceContainer.Occurrence.Interval);

                if (rect.HasValue)
                {
                    if (rectGroups.ContainsKey(rect.Value) == false)
                    {
                        rectGroups.Add(rect.Value, new List<AppointmentItem>());
                    }

                    rectGroups[rect.Value].Add(occurrenceContainer.Container);
                }
            }

            double headerHeight = 20;
            double currentOffset;
            double appointmentItemHeight = 20;
            Thickness margin = new Thickness(0, 0, 15, 5);
            Rect arrangeRect;

            foreach (var rectGroup in rectGroups)
            {
                arrangeRect = rectGroup.Key;
                currentOffset = headerHeight;

                foreach (var appointmentItem in rectGroup.Value)
                {
                    if (currentOffset + appointmentItemHeight < arrangeRect.Height)
                    {
                        appointmentItem.Arrange(new Rect(arrangeRect.Left, arrangeRect.Top + currentOffset, arrangeRect.Width - margin.Right, appointmentItemHeight));
                        appointmentItem.Visibility = Visibility.Visible;
                        currentOffset += appointmentItemHeight + margin.Bottom;
                    }
                    else
                    {
                        // there are hidden appointments

                        appointmentItem.Visibility = Visibility.Hidden;
                    }
                }
            }

            foreach (var notUsedAppointmentItem in appointmentItemsCache)
            {
                notUsedAppointmentItem.Visibility = Visibility.Collapsed;
            }

            return finalSize;
        }

        private class OccurrenceContainer
        {
            public Occurrence Occurrence { get; set; }
            public AppointmentItem Container { get; set; }
        }
    }
}
