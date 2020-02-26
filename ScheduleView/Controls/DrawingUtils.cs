using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    public static class DrawingUtils
    {
        public static void DrawSnappedLinesBetweenPoints(this DrawingContext dc, Pen pen, double lineThickness, params Point[] points)
        {
            var guidelineSet = new GuidelineSet();

            foreach (var point in points)
            {
                guidelineSet.GuidelinesX.Add(point.X);
                guidelineSet.GuidelinesY.Add(point.Y);
            }

            var half = lineThickness * 1.25 / 2;
            points = points.Select(p => new Point(p.X + half, p.Y + half)).ToArray();

            dc.PushGuidelineSet(guidelineSet);

            for (var i = 0; i < points.Length - 1; i = i + 2)
            {
                dc.DrawLine(pen, points[i], points[i + 1]);
            }

            dc.Pop();
        }
    }
}
