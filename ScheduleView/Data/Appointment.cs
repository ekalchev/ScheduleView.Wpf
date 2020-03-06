using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.Data
{
    public class Appointment
    {
        public Appointment(int id, Interval interval, bool allDay, string title)
        {
            Id = id;
            Interval = interval;
            AllDay = allDay;
            Title = title;
        }

        public int Id { get; }
        public Interval Interval { get; }
        public bool AllDay { get; }
        public string Title { get; }
    }
}
