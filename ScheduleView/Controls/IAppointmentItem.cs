using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.Controls
{
    public interface IAppointmentItem
    {
        Interval Interval { get; }
    }
}
