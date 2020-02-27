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
        private static readonly Brush lineBrush;
        private static readonly Pen linePen;

        public double Pos
        {
            get { return (double)GetValue(PosProperty); }
            set { SetValue(PosProperty, value); }
        }

        public static readonly DependencyProperty PosProperty =
            DependencyProperty.Register("Pos", typeof(double), typeof(ScheduleView), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));


        internal MonthViewData MonthsViewData { get; } = new MonthViewData();
        
        static ScheduleView()
        {
            lineBrush = new SolidColorBrush(Colors.Red);
            lineBrush.Freeze();

            linePen = new Pen(lineBrush, 1);
            linePen.Freeze();
        }

        public ScheduleView()
        {
        }

        private MonthsViewAppointmentsPanel monthsViewPanel;
        private MonthViewGrid monthsViewGrid;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            monthsViewPanel = this.GetTemplateChild("PART_MonthsViewAppointmentsPanel") as MonthsViewAppointmentsPanel;
            monthsViewPanel.ScheduleView = this;

            monthsViewGrid = this.GetTemplateChild("PART_MonthsViewGrid") as MonthViewGrid;
            monthsViewGrid.ScheduleView = this;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            MonthsViewData.Update(constraint);

            return base.MeasureOverride(constraint);
        }
    }
}
