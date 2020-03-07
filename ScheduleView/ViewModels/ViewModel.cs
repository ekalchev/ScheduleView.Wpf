using NodaTime;
using Prism.Mvvm;
using ScheduleView.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.ViewModels
{
    public class ViewModel : BindableBase
    {
        public ViewModel()
        {
            List<Appointment> appointments = new List<Appointment>();

            int days = 5 * 7;
            int appointmentsPerDay = 3;
            Random random = new Random();
            Instant now = SystemClock.Instance.GetCurrentInstant();

            for (int i = 0; i < days; i++)
            {
                Instant appointmentStart = now;

                for (int j = 0; j < random.Next() % 10; j++)
                {
                    var appointmentEnd = appointmentStart.Plus(Duration.FromMinutes(15));
                    appointments.Add(new Appointment(i, new Interval(appointmentStart, appointmentEnd), false, Guid.NewGuid().ToString()));
                    appointmentStart = appointmentEnd.Plus(Duration.FromMinutes(15));
                }

                now = now.Plus(Duration.FromDays(1));
            }

            Appointments = appointments;
        }

        private IEnumerable<Appointment> appointments;
        public IEnumerable<Appointment> Appointments
        {
            get
            {
                return appointments;
            }

            set
            {
                SetProperty(ref appointments, value);
            }
        }
    }
}
