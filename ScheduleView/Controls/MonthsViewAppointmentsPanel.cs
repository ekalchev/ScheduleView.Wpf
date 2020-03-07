using ScheduleView.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IControlCache<UIElement> hiddenAppointmentsButtonsCache;

        private Dictionary<Occurrence, OccurrenceContainer> occurenceContainerMap = new Dictionary<Occurrence, OccurrenceContainer>();
        private IEnumerable<AppointmentMeasureGroup> appointmentMeasureGroups = Enumerable.Empty<AppointmentMeasureGroup>();

        public IEnumerable<Occurrence> Occurrences { get; set; }
        public MonthViewData Data { get; set; }
        private int measureCounter = 0;
        private int arrangeCounter = 0;

        public MonthsViewAppointmentsPanel()
        {
            hiddenAppointmentsButtonsCache = new ControlCache<ExpandArrow>(0);

            for (int i = 0; i < 500; i++)
            {
                var appointmentItem = new AppointmentItem();
                appointmentItemsCache.Push(appointmentItem);
                Children.Add(appointmentItem);
            }
        }

        /// <summary>
        /// We assume that measure size == arrange size
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Debug.WriteLine($"measure {measureCounter++}");

            // dispose previous measurement groups
            foreach (var appointmentMeasureGroup in appointmentMeasureGroups)
            {
                appointmentMeasureGroup.Dispose();
            }

            Dictionary<Rect, AppointmentMeasureGroup> rectGroups = new Dictionary<Rect, AppointmentMeasureGroup>();

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

                // group appointmentItems in groups by grid cells 

                // find in which grid cell the occurrent should be arranged
                var rect = Data.Intersect(occurrence.Interval);

                if (rect.HasValue)
                {
                    if (rectGroups.ContainsKey(rect.Value) == false)
                    {
                        rectGroups.Add(rect.Value, new AppointmentMeasureGroup(rect.Value, this, hiddenAppointmentsButtonsCache.Get()));
                    }

                    rectGroups[rect.Value].Add(occurrenceContainer.Container);
                }

                newOccurenceContainerMap.Add(occurrence, occurrenceContainer);
            }

            appointmentMeasureGroups = rectGroups.Values;

            foreach(var appointmentMeasureGroup in appointmentMeasureGroups)
            {
                appointmentMeasureGroup.PrepareAppointments();
            }

            // move unmatched containers back to cache
            foreach (var occurrenceContainer in occurenceContainerMap.Values)
            {
                appointmentItemsCache.Push(occurrenceContainer.Container);
            }

            occurenceContainerMap = newOccurenceContainerMap;

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine($"arrange {arrangeCounter++}");

            foreach (var appointmentMeasureGroup in appointmentMeasureGroups)
            {
                appointmentMeasureGroup.ArrangeVisibleAppointments();
            }

            return finalSize;
        }

        private class OccurrenceContainer
        {
            public Occurrence Occurrence { get; set; }
            public AppointmentItem Container { get; set; }
        }

        private class AppointmentMeasureGroup : IDisposable
        {
            private double headerHeight = LayoutHelper.RoundLayoutValue(20);
            private double currentOffset;
            private double appointmentItemHeight = LayoutHelper.RoundLayoutValue(20);
            private Thickness margin = new Thickness(0, 0, LayoutHelper.RoundLayoutValue(15), LayoutHelper.RoundLayoutValue(5));

            private List<AppointmentItem> visibleAppointments = new List<AppointmentItem>();
            private List<AppointmentItem> hiddenAppointments = new List<AppointmentItem>();
            private readonly Panel panel;
            private readonly ICacheItem<UIElement> expandButton;

            public AppointmentMeasureGroup(Rect rect, Panel panel, ICacheItem<UIElement> expandButton)
            {
                Rect = rect;
                this.panel = panel;
                this.expandButton = expandButton;
                currentOffset = headerHeight;
            }

            public Rect Rect { get; }
            public IEnumerable<AppointmentItem> VisibleAppointments => visibleAppointments;
            public IEnumerable<AppointmentItem> HiddenAppointments => hiddenAppointments;

            public void PrepareAppointments()
            {
                foreach (var visibleAppointment in VisibleAppointments)
                {
                    visibleAppointment.Visibility = Visibility.Visible;
                    visibleAppointment.Measure(new Size(Rect.Width - margin.Right, appointmentItemHeight));
                }

                foreach (var hiddenAppointment in HiddenAppointments)
                {
                    hiddenAppointment.Visibility = Visibility.Hidden;
                }

                if(HiddenAppointments.Count() > 0)
                {
                    panel.Children.Add(expandButton.Item);
                    expandButton.Item.Measure(Rect.Size);
                }
            }

            public void ArrangeVisibleAppointments()
            {
                double currentOffset = this.headerHeight;

                foreach (var visibleAppointment in VisibleAppointments)
                {
                    visibleAppointment.Arrange(new Rect(Rect.Left, Rect.Top + currentOffset, Rect.Width - margin.Right, appointmentItemHeight));
                    currentOffset += appointmentItemHeight + margin.Bottom;
                }

                if (HiddenAppointments.Count() > 0)
                {
                    expandButton.Item.Arrange(new Rect(Rect.Right - expandButton.Item.DesiredSize.Width, Rect.Bottom - expandButton.Item.DesiredSize.Height, expandButton.Item.DesiredSize.Width, expandButton.Item.DesiredSize.Height));
                }
            }

            public void Add(AppointmentItem appointmentItem)
            {
                if (currentOffset + appointmentItemHeight < Rect.Height)
                {
                    visibleAppointments.Add(appointmentItem);
                    currentOffset += appointmentItemHeight + margin.Bottom;
                }
                else
                {
                    hiddenAppointments.Add(appointmentItem);
                }
            }

            public void Dispose()
            {
                panel.Children.Remove(expandButton.Item);
                expandButton.Dispose();
            }
        }
    }
}
