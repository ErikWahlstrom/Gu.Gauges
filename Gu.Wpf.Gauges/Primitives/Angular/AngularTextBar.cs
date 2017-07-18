namespace Gu.Wpf.Gauges
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    public class AngularTextBar : TextTickBar
    {
        public static readonly DependencyProperty TextOrientationProperty = AngularGauge.TextOrientationProperty.AddOwner(
            typeof(AngularTextBar),
            new FrameworkPropertyMetadata(
                Defaults.TextOrientation,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty TextPositionProperty = DependencyProperty.Register(
            nameof(TextPosition),
            typeof(AngularTextPosition),
            typeof(AngularTextBar),
            new FrameworkPropertyMetadata(
                AngularTextPosition.Default,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
                OnTextPositionChanged,
                CoerceTextPosition));

        public static readonly DependencyProperty StartProperty = AngularGauge.StartProperty.AddOwner(
            typeof(AngularTextBar),
            new FrameworkPropertyMetadata(
                Defaults.StartAngle,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty EndProperty = AngularGauge.EndProperty.AddOwner(
            typeof(AngularTextBar),
            new FrameworkPropertyMetadata(
                Defaults.EndAngle,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        private ArcInfo arc;

        /// <summary>
        /// Gets or sets the <see cref="T:Gu.Wpf.Gauges.TextOrientation" />
        /// Default is Tangential
        /// </summary>
        public TextOrientation TextOrientation
        {
            get => (TextOrientation)this.GetValue(TextOrientationProperty);
            set => this.SetValue(TextOrientationProperty, value);
        }

        /// <summary>
        /// Controls how each tick is arranged.
        /// </summary>
        public AngularTextPosition TextPosition
        {
            get => (AngularTextPosition)this.GetValue(TextPositionProperty);
            set => this.SetValue(TextPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the start angle of the arc.
        /// Degrees clockwise from the y axis.
        /// The default is -140
        /// </summary>
        public double Start
        {
            get => (double)this.GetValue(StartProperty);
            set => this.SetValue(StartProperty, value);
        }

        /// <summary>
        /// Gets or sets the end angle of the arc.
        /// Degrees clockwise from the y axis.
        /// The default is 140
        /// </summary>
        public double End
        {
            get => (double)this.GetValue(EndProperty);
            set => this.SetValue(EndProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var rect = default(Rect);
            if (this.AllTexts != null)
            {
                foreach (var tickText in this.AllTexts)
                {
                    rect.Union(new Rect(tickText.Geometry.Bounds.Size));
                }
            }

            return rect.Size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.arc = ArcInfo.Fit(finalSize, this.Start, this.End);
            if (this.AllTexts != null)
            {
                foreach (var tickText in this.AllTexts)
                {
                    tickText.TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, 0.0d);
                    tickText.TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, 0.0d);
                    switch (this.TextOrientation)
                    {
                        case TextOrientation.Tangential:
                            tickText.Transform = new RotateTransform(this.Angle(tickText.Value) + 90);
                            break;
                        case TextOrientation.TangentialFlipped:
                            tickText.Transform = new RotateTransform(this.Angle(tickText.Value) - 90);
                            break;
                        case TextOrientation.RadialOut:
                            tickText.Transform = new RotateTransform(this.Angle(tickText.Value));
                            break;
                        case TextOrientation.RadialIn:
                            tickText.Transform = new RotateTransform(this.Angle(tickText.Value) - 180);
                            break;
                        case TextOrientation.UseTransform:
                            tickText.Transform = this.TextTransform;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var pos = this.PixelPosition(tickText);
                    tickText.TranslateTransform.SetCurrentValue(TranslateTransform.XProperty, pos.X);
                    tickText.TranslateTransform.SetCurrentValue(TranslateTransform.YProperty, pos.Y);
                }
            }

            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.Foreground == null ||
                this.AllTexts == null ||
                this.AllTexts.Count == 0)
            {
                return;
            }

            foreach (var tickText in this.AllTexts)
            {
                dc.DrawGeometry(this.Foreground, null, tickText.Geometry);
            }
        }

        protected virtual double Angle(double value)
        {
            var linear = Interpolate.Linear(this.Minimum, this.Maximum, value)
                                    .Clamp(0, 1);
            return linear.Interpolate(this.Start, this.End, this.IsDirectionReversed);
        }

        protected virtual Point PixelPosition(double value)
        {
            return this.arc.GetPoint(this.Angle(value));
        }

        protected virtual Point PixelPosition(TickText tickText)
        {
            return this.PixelPosition(tickText.Value);
        }

        protected virtual TickText CreateTickText(double value)
        {
            return new TickText(
                value,
                this.StringFormat ?? string.Empty,
                this.TypeFace,
                this.FontSize,
                this.Foreground,
                this.TextOrientation == TextOrientation.UseTransform ? this.TextTransform : null);
        }

        protected override void UpdateTexts()
        {
            if (this.AllTicks == null || this.AllTicks.Count == 0)
            {
                this.AllTexts = null;
                return;
            }

            this.AllTexts = this.AllTicks.Select(this.CreateTickText).ToArray();
        }

        private static void OnTextPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBar = (AngularTextBar)d;
            if (e.OldValue is AngularTextPosition oldValue)
            {
                oldValue.ArrangeDirty -= textBar.OnTextPositionArrange;
            }

            if (e.NewValue is AngularTextPosition newValue)
            {
                newValue.ArrangeDirty += textBar.OnTextPositionArrange;
            }
        }

        private static object CoerceTextPosition(DependencyObject d, object basevalue)
        {
            return basevalue ?? LinearTextPosition.Default;
        }

        private void OnTextPositionArrange(object sender, EventArgs e)
        {
            this.InvalidateArrange();
        }
    }
}