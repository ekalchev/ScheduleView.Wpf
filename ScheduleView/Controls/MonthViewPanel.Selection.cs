using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    internal partial class MonthViewPanel
    {
        MonthViewDayContainer selectionAnchor = null;

        private void MonthViewPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
        }

        private void MonthViewPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectionAnchor = null;
            ResetSelection();

            Mouse.Capture(this, CaptureMode.Element);

            // Retrieve the coordinate of the mouse position.
            Point point = e.GetPosition((UIElement)sender);

            selectionAnchor = GetContaionerAtPoint(point);
            selectionAnchor.IsSelected = true;
        }

        private void MonthViewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if(selectionAnchor != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // Retrieve the coordinate of the mouse position.
                Point point = e.GetPosition((UIElement)sender);

                var container = GetContaionerAtPoint(point);

                if (container != null)
                {
                    MakeSelection(container);
                }
            }
        }
        private void ResetSelection()
        {
            foreach (var day in monthViewDayItems)
            {
                day.IsSelected = false;
            }
        }

        private void MakeSelection(MonthViewDayContainer container)
        {
            bool selectionStarted = false;

            foreach (var day in monthViewDayItems)
            {
                if (selectionAnchor == container)
                {
                    container.IsSelected = true;
                }
                else if (day == container || day == selectionAnchor)
                {
                    selectionStarted = !selectionStarted;
                }

                day.IsSelected = selectionStarted;
            }

            selectionAnchor.IsSelected = true;
            container.IsSelected = true;
        }

        private MonthViewDayContainer GetContaionerAtPoint(Point point)
        {
            MonthViewDayContainer container = null;

            // Perform the hit test against a given portion of the visual object tree.
            HitTestResult result = VisualTreeHelper.HitTest(this, point);

            if (result != null)
            {
                container = result.VisualHit as MonthViewDayContainer;
            }

            return container;
        }
    }
}
