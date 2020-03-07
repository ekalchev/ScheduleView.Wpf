using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScheduleView.Wpf.Controls
{
    internal class ExpandArrow : Button
    {
        static ExpandArrow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandArrow), new FrameworkPropertyMetadata(typeof(ExpandArrow)));
        }
    }
}
