using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthViewDay
    {
        public Interval Day { get; set; }
        public Rect GridCell { get; set; } 
    }
}
