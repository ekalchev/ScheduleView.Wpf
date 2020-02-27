using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    public static class DrawingContextExtensions
    {
        /// <summary>
        /// Draw a rectangle snapped on physical pixels
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="rect">Must be rounded on whole pixel. Use LayoutHelper.RoundLayoutRect</param>
        /// <param name="pen"></param>
        public static void DrawSnappedRectangle(this DrawingContext drawingContext, Rect rect, Pen pen, Brush brush)
        {
            // + 0.5 means shift the point at the center of the next pixel
            var snappedRect = new Rect(rect.Left + LayoutHelper.CenterPixelOffset, rect.Top + LayoutHelper.CenterPixelOffset, rect.Width, rect.Height);

            double halfPenWidth = pen.Thickness / 2;

            GuidelineSet guidelineSet = new GuidelineSet();
            guidelineSet.GuidelinesX.Add(snappedRect.Left - halfPenWidth);
            guidelineSet.GuidelinesX.Add(snappedRect.Left + halfPenWidth);
            guidelineSet.GuidelinesX.Add(snappedRect.Right - halfPenWidth);
            guidelineSet.GuidelinesX.Add(snappedRect.Right + halfPenWidth);

            guidelineSet.GuidelinesY.Add(snappedRect.Top - halfPenWidth);
            guidelineSet.GuidelinesY.Add(snappedRect.Top + halfPenWidth);
            guidelineSet.GuidelinesY.Add(snappedRect.Bottom - halfPenWidth);
            guidelineSet.GuidelinesY.Add(snappedRect.Bottom + halfPenWidth);
            guidelineSet.Freeze();

            drawingContext.PushGuidelineSet(guidelineSet);
            drawingContext.DrawRectangle(brush, pen, snappedRect);
            drawingContext.Pop();
        }

        /// <summary>
        /// Draw a line snapped on physical pixel. IMPORTANT your line coordinated need to be rounded on physical pixel
        /// use LayoutHelper.RoundLayoutValue
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        /// <param name="pen"></param>
        public static void DrawHorizontalSnappedLine(this DrawingContext drawingContext, Point point0, Point point1, Pen pen)
        {
            // ensure this is horizontal line
            Debug.Assert(DoubleUtil.AreClose(point0.Y, point1.Y));

            var snappedPoint0 = new Point(point0.X + LayoutHelper.CenterPixelOffset, point0.Y + LayoutHelper.CenterPixelOffset);
            var snappedPoint1 = new Point(point1.X + LayoutHelper.CenterPixelOffset, point1.Y + LayoutHelper.CenterPixelOffset);

            double halfPenWidth = pen.Thickness / 2;

            var guidelineSet = new GuidelineSet(
                    new double[] { snappedPoint0.X, snappedPoint1.X },
                    new double[] { snappedPoint0.Y - halfPenWidth, snappedPoint0.Y + halfPenWidth });

            guidelineSet.Freeze();

            drawingContext.PushGuidelineSet(guidelineSet);
            drawingContext.DrawLine(pen, snappedPoint0, snappedPoint1);
            drawingContext.Pop();
        }

        public static void DrawVerticalSnappedLine(this DrawingContext drawingContext, Point point0, Point point1, Pen pen)
        {
            // ensure this is horizontal line
            Debug.Assert(DoubleUtil.AreClose(point0.X, point1.X));

            var snappedPoint0 = new Point(point0.X + LayoutHelper.CenterPixelOffset, point0.Y - LayoutHelper.CenterPixelOffset);
            var snappedPoint1 = new Point(point1.X + LayoutHelper.CenterPixelOffset, point1.Y - LayoutHelper.CenterPixelOffset);

            double halfPenWidth = pen.Thickness / 2;

            var guidelineSet = new GuidelineSet(
                    new double[] { snappedPoint0.X - halfPenWidth, snappedPoint1.X + halfPenWidth },
                    new double[] { snappedPoint0.Y, snappedPoint0.Y });

            guidelineSet.Freeze();

            drawingContext.PushGuidelineSet(guidelineSet);
            drawingContext.DrawLine(pen, snappedPoint0, snappedPoint1);
            drawingContext.Pop();
        }
    }
}
