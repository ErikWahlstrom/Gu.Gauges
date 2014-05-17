﻿namespace Gauges
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    [TemplatePart(Name = IndicatorTemplateName, Type = typeof(FrameworkElement))]
    public class Gauge : RangeBase
    {
        private FrameworkElement _indicator;
        private readonly TranslateTransform _indicatorTransform = new TranslateTransform();
        private const string IndicatorTemplateName = "PART_Indicator";

        static Gauge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gauge), new FrameworkPropertyMetadata(typeof(Gauge)));
        }

        public ObservableCollection<GaugeLabel> Lables { get; set; }

        /// <summary>
        /// Called when a template is applied to a <see cref="T:System.Windows.Controls.ProgressBar"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (Template == null)
            {
                return;
            }
            base.OnApplyTemplate();
            this._indicator = this.GetTemplateChild(IndicatorTemplateName) as FrameworkElement;
            if (_indicator != null)
            {
                _indicator.RenderTransform = _indicatorTransform;
                _indicator.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// Updates the current position of the <see cref="T:System.Windows.Controls.ProgressBar"/> when the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property changes.
        /// </summary>
        /// <param name="oldMinimum">Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param><param name="newMinimum">New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            this.SetIndicatorPos();
        }

        /// <summary>
        /// Updates the current position of the <see cref="T:System.Windows.Controls.ProgressBar"/> when the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property changes.
        /// </summary>
        /// <param name="oldMaximum">Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param><param name="newMaximum">New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            this.SetIndicatorPos();
        }

        /// <summary>
        /// Updates the current position of the <see cref="T:System.Windows.Controls.ProgressBar"/> when the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value"/> property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value"/> property.</param><param name="newValue">New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value"/> property.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.SetIndicatorPos();
        }

        private void SetIndicatorPos()
        {
            if (this._indicator == null)
                return;
            double minimum = this.Minimum;
            double maximum = this.Maximum;
            double num = this.Value;
            double d = (num - minimum) / Math.Abs(maximum - minimum);

            _indicatorTransform.SetCurrentValue(TranslateTransform.XProperty, this.ActualWidth * (d - 0.5));
        }
    }
}
