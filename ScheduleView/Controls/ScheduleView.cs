using NodaTime;
using ScheduleView.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace ScheduleView.Wpf.Controls
{
    public class ScheduleView : Control
    {
        internal MonthViewData MonthsViewData { get; } = new MonthViewData();
        private IEnumerable<Occurrence> occurrencies = Enumerable.Empty<Occurrence>();

        public IEnumerable<Appointment> AppointmentItems
        {
            get { return (IEnumerable<Appointment>)GetValue(AppointmentItemsProperty); }
            set { SetValue(AppointmentItemsProperty, value); }
        }

        public static readonly DependencyProperty AppointmentItemsProperty =
            DependencyProperty.Register("AppointmentItems", typeof(IEnumerable<Appointment>), typeof(ScheduleView), new FrameworkPropertyMetadata(null, OnAppointmentItemsPropertyChanged));

        private static void OnAppointmentItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appointments = e.NewValue as IEnumerable<Appointment>;
            var scheduleView = d as ScheduleView;

            var occurrencies = new List<Occurrence>();

            foreach (var appointment in appointments)
            {
                // TODO: build correct occurrencies from appointment data
                occurrencies.Add(new Occurrence(appointment, appointment, OccurrenceType.Single, appointment.Interval));
            }

            scheduleView.occurrencies = occurrencies;

            scheduleView.InvalidateMeasure();
            scheduleView.UpdateLayout();
        }

        public ScheduleView()
        {
        }

        private MonthsViewAppointmentsPanel monthsViewPanel;
        private MonthViewPanel monthsViewGrid;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            monthsViewPanel = this.GetTemplateChild("PART_MonthsViewAppointmentsPanel") as MonthsViewAppointmentsPanel;

            monthsViewGrid = this.GetTemplateChild("PART_MonthsViewGrid") as MonthViewPanel;
            monthsViewGrid.ScheduleView = this;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            MonthsViewData.Update(constraint, SystemClock.Instance.GetCurrentInstant());

            monthsViewPanel.Data = MonthsViewData;
            monthsViewPanel.Occurrences = occurrencies;

            return base.MeasureOverride(constraint);
        }
    }
}
