using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScheduleView.Wpf.Controls
{
    public class AppointmentItem : Control
    {
        private static readonly Brush fillBrush;
        private static readonly Brush fillBrushHover;
        private static readonly Brush borderBrush;
        private static readonly Pen pen;

        private DpiScale dpiScale;
        private static readonly CultureInfo culture = CultureInfo.GetCultureInfo("en-us");
        private static readonly Typeface segoeTypeface = new Typeface("Segoe UI");
        private static readonly Style style;

        static AppointmentItem()
        {
            BackgroundProperty.OverrideMetadata(typeof(AppointmentItem), new FrameworkPropertyMetadata((Brush)null, FrameworkPropertyMetadataOptions.AffectsRender));

            fillBrushHover = new SolidColorBrush(Colors.White);
            fillBrushHover.Freeze();

            fillBrush = new SolidColorBrush(Colors.Orange);
            fillBrush.Freeze();

            borderBrush = new SolidColorBrush(Colors.Black);
            borderBrush.Freeze();

            pen = new Pen(borderBrush, 1);
            pen.Freeze();

            style = new Style(typeof(AppointmentItem));
            style.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = fillBrush,
            });

            Trigger trigger = new Trigger()
            {
                Property = IsMouseOverProperty,
                Value = true
            };

            trigger.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = Brushes.Red
            });

            style.Triggers.Add(trigger);
        }

        public AppointmentItem()
        {
            Style = style;
        }

        GlyphRun glyphRun;
        FormattedText formattedText;

        private string text = Guid.NewGuid().ToString();
        private Size arrangeBounds;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            dpiScale = VisualTreeHelper.GetDpi(this);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.arrangeBounds = arrangeBounds;

            return base.ArrangeOverride(arrangeBounds);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var roundedArrangeBound = LayoutHelper.RoundLayoutSize(this.arrangeBounds);

            drawingContext.DrawSnappedRectangle(new Rect(0,0, roundedArrangeBound.Width, roundedArrangeBound.Height), pen, Background);

            if (formattedText == null)
            {
                formattedText = new FormattedText(text,
                        culture,
                        this.FlowDirection,
                        segoeTypeface,
                        13,
                        Brushes.Black,
                        dpiScale.PixelsPerDip);
            }

            //drawingContext.DrawText(formattedText, new Point(0, 0));

            //if (glyphRun == null)
            //{
            //    glyphRun = CreateGlyphRun(text, 13, new Point(0, 0), dpiScale.PixelsPerDip);
            //}

            //drawingContext.DrawGlyphRun(borderBrush, glyphRun);
        }

        private static Dictionary<ushort, double> glyphWidths = new Dictionary<ushort, double>();
        private static GlyphTypeface glyphTypeface;
        public static GlyphRun CreateGlyphRun(string text, double size, Point position, double pixelsPerDip)
        {
            if (glyphTypeface == null)
            {
                Typeface typeface = segoeTypeface;
                if (typeface.TryGetGlyphTypeface(out glyphTypeface) == false)
                {
                    throw new InvalidOperationException("No glyphtypeface found");
                }
            }

            ushort[] glyphIndexes = new ushort[text.Length];
            double[] advanceWidths = new double[text.Length];

            var totalWidth = 0d;
            double glyphWidth;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = (ushort)(text[n] - 29);
                glyphIndexes[n] = glyphIndex;

                if (glyphWidths.TryGetValue(glyphIndex, out glyphWidth) == false)
                {
                    glyphWidth = glyphTypeface.AdvanceWidths[glyphIndex] * size;
                    glyphWidths.Add(glyphIndex, glyphWidth);
                }
                advanceWidths[n] = glyphWidth;
                totalWidth += glyphWidth;
            }

            var offsetPosition = new Point(position.X, position.Y);

            GlyphRun glyphRun = new GlyphRun(glyphTypeface, 0, false, size, (float)pixelsPerDip, glyphIndexes, offsetPosition, advanceWidths, null, null, null, null, null, null);

            return glyphRun;
        }
    }
}
