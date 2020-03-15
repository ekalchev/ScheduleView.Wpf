using NodaTime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ScrollAnimation currentAnimation;
        private static readonly double extendHeight = 100;
        private static readonly double viewportHeight = 10;

        private double scrollOffset;

        public AnimatedScrollPanel()
        {
            ResetTranslateTransform();
            VisualClip = null;
            
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var a = VisualScrollableAreaClip;
            scrollOffset = scrollLine = LayoutHelper.RoundLayoutValue(availableSize.Height / (rowsCount - 2));
            extendSize = availableSize;

            ///ScheduleView.MonthsViewData.Update(availableSize);
            foreach (var child in Children.OfType<UIElement>())
            {
                child.Measure(availableSize);
            }

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var element in Children.OfType<UIElement>())
            {
                element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }

        private void ResetTranslateTransform()
        {
            TranslateTransform trans = this.RenderTransform as TranslateTransform;

            this.RenderTransformOrigin = new Point(0, 0);
            trans = new TranslateTransform(0, 0);
            this.RenderTransform = trans;
        }

        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;

        public bool CanVerticallyScroll { get; set; } = true;
        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth => 0;

        public double ExtentHeight => extendHeight;

        public double ViewportWidth => 0;

        public double ViewportHeight => viewportHeight;

        public double HorizontalOffset { get; private set; }

        public double VerticalOffset { get; private set; } = (extendHeight - viewportHeight) / 2;

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
            ScrollDirection direction = DoubleUtil.GreaterThan(VerticalOffset, offset) ? ScrollDirection.Down : ScrollDirection.Up;

            // if we are chaging the direction of scroll, remove all queued animations and stop current anim
            if (scrollStepQueue.Count > 0 && scrollStepQueue.Peek().Direction != direction)
            {
                scrollStepQueue.Clear();
                //StopCurrentScrollAnimation(); // this doesn't look good
            }

            var duration = new System.Windows.Duration(TimeSpan.FromMilliseconds((double)500 / Math.Max(1, Math.Min(scrollStepQueue.Count, 3))));

            UpdatePendingAnimationDurations(duration);
            Debug.WriteLine($"{scrollStepQueue.Count}, {duration.TimeSpan}");
            scrollStepQueue.Enqueue(new ScrollStep()
            {
                Offset = scrollLine * (int)direction,
                Direction = direction,
                Duration = duration
            });

            ExecuteNextAnimationInQueue();
        }

        private Queue<ScrollStep> scrollStepQueue = new Queue<ScrollStep>();

        private void UpdatePendingAnimationDurations(System.Windows.Duration duration)
        {
            foreach(var item in scrollStepQueue)
            {
                item.Duration = duration;
            }
        }

        private void ExecuteNextAnimationInQueue()
        {
            if (currentAnimation == null && scrollStepQueue.Count > 0)
            {
                var scrollStep = scrollStepQueue.Peek();

                var scrollAnimation = new DoubleAnimation(0, scrollStep.Offset, scrollStep.Duration);
                scrollAnimation.EasingFunction = new QuarticEase();
                scrollAnimation.Completed += (s, e) => AnimationCompletedHandler(scrollStep.Direction);

                foreach (var child in Children.OfType<UIElement>())
                {
                    (child as IAnimationScrollObservable)?.NotifyScrollAnimationStarted(scrollStep.Offset, scrollStep.Direction);
                }

                currentAnimation = new ScrollAnimation(scrollAnimation, scrollStep);
                StartScrollAnimation(scrollAnimation);
            }
        }

        /// <summary>
        /// Pass null to stop the animation. Stopping animation only works with HandoffBehavior.SnapshotAndReplace
        /// </summary>
        /// <param name="animation"></param>
        private void StartScrollAnimation(AnimationTimeline animation)
        {
            TranslateTransform trans = this.RenderTransform as TranslateTransform;
            trans.BeginAnimation(TranslateTransform.YProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        private void StopCurrentScrollAnimation()
        {
            StartScrollAnimation(null);
        }

        private void AnimationCompletedHandler(ScrollDirection direction)
        {
            currentAnimation = null;

            // pop out completed animation
            if (scrollStepQueue.Count > 0)
            {
                scrollStepQueue.Dequeue();
            }

            ScheduleView.MonthViewStartDate = ScheduleView.MonthViewStartDate.Plus(NodaTime.Duration.FromDays(7 * -(int)direction));
            this.ResetTranslateTransform();

            if (scrollStepQueue.Count > 0)
            {
                ExecuteNextAnimationInQueue();
            }
            else
            {
                foreach (var child in Children.OfType<UIElement>())
                {
                    (child as IAnimationScrollObservable)?.NotifyScrollAnimationCompleted();
                }
            }            
        }

        public void LineLeft()
        {
        }

        public void LineRight()
        {
        }

        private class ScrollStep
        {
            public ScrollDirection Direction { get; set; }
            public double Offset { get; set; }
            public System.Windows.Duration Duration { get; set; }
        }

        private class ScrollAnimation
        {
            public ScrollAnimation(DoubleAnimation animation, ScrollStep scrollStep)
            {
                Animation = animation;
                ScrollStep = scrollStep;
            }

            public DoubleAnimation Animation { get; }
            public ScrollStep ScrollStep { get; }
        }
    }
}
