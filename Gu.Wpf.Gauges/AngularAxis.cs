﻿namespace Gu.Wpf.Gauges
{
    using System.Windows;

    public class AngularAxis : Axis
    {
        public static readonly DependencyProperty StartProperty = AngularGauge.StartProperty.AddOwner(
            typeof(AngularAxis),
            new FrameworkPropertyMetadata(
                Defaults.StartAngle,
                FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty EndProperty = AngularGauge.EndProperty.AddOwner(
            typeof(AngularAxis),
            new FrameworkPropertyMetadata(
                Defaults.EndAngle,
                FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty TextOrientationProperty = AngularGauge.TextOrientationProperty.AddOwner(
            typeof(AngularAxis),
            new FrameworkPropertyMetadata(
                Defaults.TextOrientation,
                FrameworkPropertyMetadataOptions.Inherits));

        static AngularAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AngularAxis), new FrameworkPropertyMetadata(typeof(AngularAxis)));
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

        /// <summary>
        /// Gets or sets the <see cref="T:Gu.Wpf.Gauges.TextOrientation" />
        /// Default is Tangential
        /// </summary>
        public TextOrientation TextOrientation
        {
            get => (TextOrientation)this.GetValue(TextOrientationProperty);
            set => this.SetValue(TextOrientationProperty, value);
        }
    }
}