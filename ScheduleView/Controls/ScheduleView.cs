using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace ScheduleView.Wpf.Controls
{
    public class ScheduleView : Control
    {
        internal MonthViewData MonthsViewData { get; } = new MonthViewData();

        public IEnumerable<IAppointmentItem> AppointmentItems
        {
            get { return (IEnumerable<IAppointmentItem>)GetValue(AppointmentItemsProperty); }
            set { SetValue(AppointmentItemsProperty, value); }
        }

        public static readonly DependencyProperty AppointmentItemsProperty =
            DependencyProperty.Register("AppointmentItems", typeof(IEnumerable<IAppointmentItem>), typeof(ScheduleView), new PropertyMetadata(null));

        public ScheduleView()
        {
        }

        private MonthsViewAppointmentsPanel monthsViewPanel;
        private MonthViewPanel monthsViewGrid;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            monthsViewPanel = this.GetTemplateChild("PART_MonthsViewAppointmentsPanel") as MonthsViewAppointmentsPanel;
            monthsViewPanel.ScheduleView = this;

            monthsViewGrid = this.GetTemplateChild("PART_MonthsViewGrid") as MonthViewPanel;
            monthsViewGrid.ScheduleView = this;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            MonthsViewData.Update(constraint, SystemClock.Instance.GetCurrentInstant(), AppointmentItems);

            return base.MeasureOverride(constraint);
        }
    }
}
