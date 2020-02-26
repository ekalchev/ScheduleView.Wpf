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


        private DrawingData drawingData = new DrawingData();
        static ScheduleView()
        {
            lineBrush = new SolidColorBrush(Colors.Red);
            lineBrush.Freeze();

            linePen = new Pen(lineBrush, 1);
            linePen.Freeze();
        }

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

        protected override Size MeasureOverride(Size constraint)
        {
            drawingData.Update(constraint);

            

            return base.MeasureOverride(constraint);
        }

        double posy = 0.0d;
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Task.Delay(50).ContinueWith(t => Pos += 0.1, TaskScheduler.FromCurrentSynchronizationContext());
            posy += 1;

            Pen pen = new Pen(Brushes.Red, 10);
            Rect rect = new Rect(20, 20, 50, 60);

            double halfPenWidth = pen.Thickness / 2;

            //foreach (var currentPos in positions)
            {
                // Create a guidelines set
                GuidelineSet guidelines = new GuidelineSet();
                guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
                guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
                guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
                guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);

                drawingContext.PushGuidelineSet(guidelines);
                drawingContext.DrawRectangle(null, pen, rect);
                drawingContext.Pop();

                var halfPixel = LayoutHelper.DpiScale / 2;
                var p0 = new Point(10, LayoutHelper.RoundLayoutValue(Pos) + halfPixel);
                var p1 = new Point(301, LayoutHelper.RoundLayoutValue(Pos) + halfPixel);

                var gs = new GuidelineSet(
                    new double[] { p0.X , p1.X  },
                    new double[] { p0.Y - halfPenWidth, p1.Y + halfPenWidth });
                drawingContext.PushGuidelineSet(gs);
                drawingContext.DrawLine(pen, p0, p1);
                drawingContext.Pop();
            }
            return;

            //double halfPenWidth = linePen.Thickness / 2 * LayoutHelper.DpiScale;

            //double columnOffset = 0;
            //for (int i = 0; i < drawingData.ColumnsCount; i++)
            //{
            //    DrawingUtils.DrawSnappedLinesBetweenPoints(drawingContext, linePen, 1.0d, 
            //        new Point(drawingData.Bounds.TopLeft.X + columnOffset, drawingData.Bounds.TopLeft.Y), 
            //        new Point(drawingData.Bounds.BottomLeft.X + columnOffset, drawingData.Bounds.BottomLeft.Y));

            //    columnOffset += drawingData.ColumnWidth;
            //}

            //DrawingUtils.DrawSnappedLinesBetweenPoints(drawingContext, linePen, 1.0d,
            //        new Point(drawingData.Bounds.TopLeft.X + columnOffset, drawingData.Bounds.TopLeft.Y),
            //        new Point(drawingData.Bounds.BottomLeft.X + columnOffset, drawingData.Bounds.BottomLeft.Y));

            //double rowOffset = 0;
            //for (int i = 0; i < drawingData.RowsCount; i++)
            //{
            //    DrawingUtils.DrawSnappedLinesBetweenPoints(drawingContext, linePen, 1.0d,
            //        new Point(drawingData.Bounds.TopLeft.X, drawingData.Bounds.TopLeft.Y + rowOffset), 
            //        new Point(drawingData.Bounds.TopRight.X, drawingData.Bounds.TopRight.Y + rowOffset));

            //    rowOffset += drawingData.RowsHeight;
            //}

            //DrawingUtils.DrawSnappedLinesBetweenPoints(drawingContext, linePen, 1.0d,
            //    new Point(drawingData.Bounds.TopLeft.X, drawingData.Bounds.TopLeft.Y + rowOffset), 
            //    new Point(drawingData.Bounds.TopRight.X, drawingData.Bounds.TopRight.Y + rowOffset));
        }
    }
}
