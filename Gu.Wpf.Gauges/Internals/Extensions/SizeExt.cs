﻿namespace Gu.Wpf.Gauges
{
    using System;
    using System.Windows;

    internal static class SizeExt
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="size"></param>
        /// <param name="vertical"></param>
        /// <param name="horizontal"></param>
        /// <returns>A vector from topleft to the specified point</returns>
        internal static Vector Offset(this Size size, Vertical vertical, Horizontal horizontal)
        {
            var x = 0.0;
            var y = 0.0;
            switch (horizontal)
            {
                case Horizontal.Left:
                    x = 0;
                    break;
                case Horizontal.Center:
                    x = size.Width / 2;
                    break;
                case Horizontal.Right:
                    x = size.Width;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(horizontal));
            }

            switch (vertical)
            {
                case Vertical.Top:
                    y = 0;
                    break;
                case Vertical.Mid:
                    y = size.Height / 2;
                    break;
                case Vertical.Bottom:
                    y = size.Height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(vertical));
            }

            return new Vector(-x, -y);
        }

        internal static bool IsInfinity(this Size size)
        {
            return double.IsInfinity(size.Width) ||
                   double.IsInfinity(size.Height);
        }

        internal static bool IsNanOrEmpty(this Size size)
        {
            return double.IsNaN(size.Width) ||
                    double.IsNaN(size.Height) ||
                    size.IsEmpty;
        }

        internal static Point MidPoint(this Size size)
        {
            return new Point(size.Width / 2, size.Height / 2);
        }

        internal static Size Deflate(this Size size, Thickness padding)
        {
            return new Size(
                Math.Max(0.0, size.Width - padding.Left - padding.Right),
                Math.Max(0.0, size.Height - padding.Top - padding.Bottom));
        }
    }
}