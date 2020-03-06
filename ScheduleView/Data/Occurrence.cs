using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.Data
{
    public class Occurrence
    {
        public Occurrence(Appointment appointment, Appointment masterAppointment, OccurrenceType occurrenceType, Interval interval)
        {
            Appointment = appointment;
            MasterAppointment = masterAppointment;
            OccurrenceType = occurrenceType;
            Interval = interval;
        }

        public Appointment Appointment { get; }
        public Appointment MasterAppointment { get; }
        public OccurrenceType OccurrenceType { get; }
        public Interval Interval { get; }
    }
}
