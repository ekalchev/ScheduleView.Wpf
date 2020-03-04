using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ScheduleView.Wpf.Controls
{
    internal class AnimatedScrollPanel : Panel, IScheduleViewAware, IScrollInfo
    {
        private Size extendSize = new Size();
        private double scrollLine;
        private const int rowsCount = 7;
        private bool scrollAnimationInProgress = false;
        private double scrollOffset;

        public AnimatedScrollPanel()
        {

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            scrollOffset = scrollLine = LayoutHelper.RoundLayoutValue(availableSize.Height / (rowsCount - 2));
            extendSize = new Size(availableSize.Width, availableSize.Height + (scrollLine * 2));

            ScheduleView.MonthsViewData.Update(availableSize);

            foreach (var child in Children.OfType<UIElement>())
            {
                child.Measure(extendSize);
            }

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            return extendSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var element in Children.OfType<UIElement>())
            {
                ResetTranslateTransform(element);

                element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            scrollAnimationInProgress = false;

            foreach (var child in Children.OfType<UIElement>())
            {
                ResetTranslateTransform(child);
            }
        }

        private void ResetTranslateTransform(UIElement element)
        {
            TranslateTransform trans = element.RenderTransform as TranslateTransform;

            element.RenderTransformOrigin = new Point(0, 0);
            trans = new TranslateTransform(0, -scrollLine);
            element.RenderTransform = trans;
        }

        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;

        public bool CanVerticallyScroll { get; set; } = true;
        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth => extendSize.Width;

        public double ExtentHeight => extendSize.Height;

        public double ViewportWidth => 100;

        public double ViewportHeight => 100;

        public double HorizontalOffset { get; private set; }

        public double VerticalOffset => ExtentHeight / 2;

        public ScrollViewer ScrollOwner { get; set; }
        public ScheduleView ScheduleView { get; set; }

        public void LineDown() { SetVerticalOffset(VerticalOffset + LineSize); }

        public void LineUp() { SetVerticalOffset(VerticalOffset - LineSize); }

        public void MouseWheelDown() { SetVerticalOffset(VerticalOffset + WheelSize); }

        public void MouseWheelUp() { SetVerticalOffset(VerticalOffset - WheelSize); }

        public void PageDown() { SetVerticalOffset(VerticalOffset + ViewportHeight); }

        public void PageUp() { SetVerticalOffset(VerticalOffset - ViewportHeight); }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect(0, 0, 0, 0);
        }

        public void MouseWheelLeft()
        {
        }

        public void MouseWheelRight()
        {
        }

        public void PageLeft()
        {
        }

        public void PageRight()
        {
        }

        public void SetHorizontalOffset(double offset)
        {
        }

        public void SetVerticalOffset(double offset)
        {
            scrollOffset = DoubleUtil.GreaterThan(VerticalOffset, offset) == true ? 0 : scrollLine * 2;

            TranslateTransform trans = null;

            if (scrollAnimationInProgress == false)
            {
                scrollAnimationInProgress = true;

                foreach (var child in Children.OfType<UIElement>())
                {
                    trans = child.RenderTransform as TranslateTransform;

                    var a = (child.RenderTransform as TranslateTransform).Y;
                    var scrollAnimation = new DoubleAnimation(-scrollOffset, new Duration(TimeSpan.FromMilliseconds(250)));
                    scrollAnimation.Completed += Animation_Completed;

                    trans.BeginAnimation(TranslateTransform.YProperty, scrollAnimation, HandoffBehavior.Compose);
                }
            }
        }

        public void LineLeft()
        {
        }

        public void LineRight()
        {
        }
    }
}
