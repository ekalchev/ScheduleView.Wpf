using NodaTime;
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
        private ScrollDirection scrollDirectionAnimation = ScrollDirection.None;

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

        public double ExtentWidth => extendSize.Width;

        public double ExtentHeight => extendSize.Height;

        public double ViewportWidth => 100;

        public double ViewportHeight => 100;

        public double HorizontalOffset { get; private set; }

        public double VerticalOffset { get; private set; }

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
            VerticalOffset = offset;

            ScheduleView.MonthViewStartDate = ScheduleView.MonthViewStartDate.Plus(NodaTime.Duration.FromDays(7));

            var scrollAnimation = new DoubleAnimation(0, scrollOffset * (int)direction, new System.Windows.Duration(TimeSpan.FromMilliseconds(150)));
            scrollAnimation.Completed += Animation_Completed;
            
            // if we are chaging the direction of scroll, remove all queued animations and stop current anim
            if(scrollDirectionAnimation != direction)
            {
                animationQueue.Clear();
                StopCurrentScrollAnimation();
            }

            animationQueue.Enqueue(scrollAnimation);

            ExecuteNextAnimationInQueue();
        }

        private Queue<DoubleAnimation> animationQueue = new Queue<DoubleAnimation>();

        private void ExecuteNextAnimationInQueue()
        {
            if (scrollDirectionAnimation == ScrollDirection.None && animationQueue.Count > 0)
            {
                var animation = animationQueue.Dequeue();
                scrollDirectionAnimation = DoubleUtil.GreaterThan(animation.To.Value, 0) ? ScrollDirection.Down : ScrollDirection.Up;

                foreach (var child in Children.OfType<UIElement>())
                {
                    (child as IAnimationScrollObservable)?.NotifyScrollAnimationStarted(animation.To.Value, scrollDirectionAnimation);
                }

                StartScrollAnimation(animation);
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

        private void Animation_Completed(object sender, EventArgs e)
        {
            scrollDirectionAnimation = ScrollDirection.None;

            if (animationQueue.Count > 0)
            {
                ExecuteNextAnimationInQueue();
            }
            else
            {
                foreach (var child in Children.OfType<UIElement>())
                {
                    (child as IAnimationScrollObservable)?.NotifyScrollAnimationCompleted();
                }

                this.ResetTranslateTransform();
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
