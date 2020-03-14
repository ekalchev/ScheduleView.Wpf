using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleView.Wpf.Controls
{
    interface IAnimationScrollObservable
    {
        void NotifyScrollAnimationStarted(double scrollOffset, ScrollDirection direction);
        void NotifyScrollAnimationCompleted();
    }

    internal enum ScrollDirection
    {
        None = 0,
        Up = -1,
        Down = 1
    }
}
