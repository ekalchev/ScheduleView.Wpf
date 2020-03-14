using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls.MonthView
{
    internal class MonthViewDayContainer : Control
    {
        private static readonly Brush lineBrush;
        private static readonly Pen linePen;
        private static readonly Brush fillBrush;
        private static readonly Brush selectedBrush;

        private static readonly Style style;

        static MonthViewDayContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthViewDayContainer), new FrameworkPropertyMetadata(typeof(MonthViewDayContainer)));
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MonthViewDayContainer), new PropertyMetadata(false));

        public int Day
        {
            get { return (int)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(int), typeof(MonthViewDayContainer), new PropertyMetadata(0));
    }
}
