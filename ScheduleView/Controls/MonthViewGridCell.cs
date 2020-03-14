using ScheduleView.Wpf.Controls.MonthView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.Controls
{
    internal partial class MonthViewPanel
    {
        private struct Cell
        {
            public MonthViewDay Day { get; set; }
            public MonthViewBorder Container { get; set; }
        }
    }
}
