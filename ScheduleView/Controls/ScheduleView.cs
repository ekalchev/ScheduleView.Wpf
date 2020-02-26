using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScheduleView.Wpf.Controls
{
    public class ScheduleView : Control
    {
        public ScheduleView()
        {
            monthsViewPanel = new MonthsViewAppointmentsPanel();
        }

        private MonthsViewAppointmentsPanel monthsViewPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            monthsViewPanel = this.GetTemplateChild("PART_MonthsViewAppointmentsPanel") as MonthsViewAppointmentsPanel;
        }
    }
}
