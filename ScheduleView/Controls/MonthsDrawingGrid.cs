using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScheduleView.Wpf.Controls
{
    internal class MonthsDrawingGrid
    {
        private const uint columns = 6;
        private const uint rows = 5;

        public MonthsDrawingGrid(DateTime startDay, Size availableSize)
        {
            Days = days;
        }

        public IEnumerable<Size> Days { get; }
    }
}
