using NodaTime;
using ScheduleView.Wpf.Controls.MonthView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewContainerCache
    {
        private Dictionary<int, MonthViewDayContainer> dateToContainerMapping = new Dictionary<int, MonthViewDayContainer>();
        private HashSet<MonthViewDayContainer> allContainers;
        private HashSet<MonthViewDayContainer> freeContainers;
        private readonly int capacity;
        private bool initialized;

        public event EventHandler<MonthViewDayContainer> NewContainerCreated;

        public MonthViewContainerCache(int capacity)
        {
            this.capacity = capacity;
        }

        public void EnsureContainersCache()
        {
            if (initialized == false)
            {
                initialized = true;

                freeContainers = new HashSet<MonthViewDayContainer>(capacity);
                allContainers = new HashSet<MonthViewDayContainer>(capacity);

                MonthViewDayContainer container;

                for (int index = 0; index < capacity; index++)
                {
                    container = CreateNewContainer();
                    allContainers.Add(container);
                    freeContainers.Add(container);
                }
            }
        }

        private MonthViewDayContainer CreateNewContainer()
        {
            var container = new MonthViewDayContainer();
            freeContainers.Add(container);
            NewContainerCreated?.Invoke(this, container);
            return container;
        }

        public MonthViewDayContainer GetContainer(int dayNumber)
        {
            MonthViewDayContainer container = null;

            // try to find container that is already used to that date
            if (dateToContainerMapping.ContainsKey(dayNumber) == true)
            {
                var candidateContainer = dateToContainerMapping[dayNumber];
                if (freeContainers.Contains(candidateContainer) == true)
                {
                    freeContainers.Remove(candidateContainer);
                    container = candidateContainer;
                }
            }

            // if not found just get container from free containers
            if (container == null && freeContainers.Count > 0)
            {
                container = freeContainers.First();
                freeContainers.Remove(container);
            }

            // if we still we don't have a container, create a new one
            if (container == null)
            {
                container = CreateNewContainer();
                allContainers.Add(container);
            }

            container.Day = dayNumber;

            container.Visibility = System.Windows.Visibility.Visible;

            if (dateToContainerMapping.ContainsKey(dayNumber) == false)
            {
                dateToContainerMapping.Add(dayNumber, container);
            }

            return container;
        }

        public void Reset()
        {
            foreach (var container in freeContainers)
            {
                container.Visibility = System.Windows.Visibility.Hidden;
            }

            freeContainers = new HashSet<MonthViewDayContainer>(allContainers);
        }
    }
}
