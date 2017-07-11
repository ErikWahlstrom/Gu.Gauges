﻿namespace Gu.Wpf.Gauges.Tests.TestHelpers
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using NUnit.Framework;
    using PixelFormat = System.Windows.Media.PixelFormat;
    using Size = System.Windows.Size;

    public static class ImageAssert
    {
        public static void AreEqual(string fileName, UIElement tickBar)
        {
            var assembly = typeof(ImageAssert).Assembly;
            var name = assembly.GetManifestResourceNames()
                               .SingleOrDefault(x => x.EndsWith(fileName, ignoreCase: true, culture: CultureInfo.InvariantCulture));
            Assert.NotNull(name, $"Did not find a resource named {fileName}");
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                using (var expected = (Bitmap)Image.FromStream(stream))
                {
                    AreEqual(expected, tickBar);
                }
            }
        }

        public static void AreEqual(Bitmap expected, UIElement actual)
        {
            using (var actualBmp = actual.ToBitmap(expected.Size(), expected.PixelFormat()))
            {
                AreEqual(expected, actualBmp);
            }
        }

        public static void AreEqual(Bitmap expected, Bitmap actual)
        {
            if (expected.Size != actual.Size)
            {
                Assert.Fail("Sizes did not match\r\n" +
                            $"Expected: {expected.Size}\r\n" +
                            $"Actual:   {actual.Size}");
            }

            if (expected.PixelFormat != actual.PixelFormat)
            {
                Assert.Fail("PixelFormats did not match\r\n" +
                            $"Expected: {expected.PixelFormat}\r\n" +
                            $"Actual:   {actual.PixelFormat}");
            }

            for (var x = 0; x < expected.Size.Width; x++)
            {
                for (var y = 0; y < expected.Size.Height; y++)
                {
                    Assert.AreEqual(expected.GetPixel(x, y).Name, actual.GetPixel(x, y).Name);
                }
            }
        }

        /////// <summary>
        /////// https://stackoverflow.com/a/21648083/1069200
        /////// This was only marginally faster.
        /////// </summary>
        /////// <param name="expected"></param>
        /////// <param name="actual"></param>
        /////// <returns></returns>
        ////public static unsafe bool AreEqualUnsafe(Bitmap expected, Bitmap actual)
        ////{
        ////    if (expected.Size != actual.Size)
        ////    {
        ////        Assert.Fail("Sizes did not match\r\n" +
        ////                    $"Expected: {expected.Size}\r\n" +
        ////                    $"Actual:   {actual.Size}");
        ////    }

        ////    if (expected.PixelFormat != actual.PixelFormat)
        ////    {
        ////        Assert.Fail("PixelFormats did not match\r\n" +
        ////                    $"Expected: {expected.PixelFormat}\r\n" +
        ////                    $"Actual:   {actual.PixelFormat}");
        ////    }

        ////    if (expected.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        ////    {
        ////        Assert.Fail("PixelFormat must be System.Drawing.Imaging.PixelFormat.Format32bppArgb");
        ////    }

        ////    var rect = new Rectangle(0, 0, expected.Width, expected.Height);
        ////    var expectedData = expected.LockBits(rect, ImageLockMode.ReadOnly, expected.PixelFormat);
        ////    var actualData = actual.LockBits(rect, ImageLockMode.ReadOnly, expected.PixelFormat);

        ////    var p1 = (int*)expectedData.Scan0;
        ////    var p2 = (int*)actualData.Scan0;
        ////    var byteCount = expected.Height * expectedData.Stride / 4; // only Format32bppArgb

        ////    var result = true;
        ////    for (var i = 0; i < byteCount; ++i)
        ////    {
        ////        if (*p1++ != *p2++)
        ////        {
        ////            result = false;
        ////            break;
        ////        }
        ////    }

        ////    expected.UnlockBits(expectedData);
        ////    actual.UnlockBits(actualData);

        ////    return result;
        ////}

        public static Bitmap ToBitmap(this UIElement element, Size size, PixelFormat pixelFormat)
        {
            return element.ToRenderTargetBitmap(size, pixelFormat)
                          .ToBitmap();
        }

        public static Bitmap ToBitmap(this UIElement element, Size size)
        {
            return element.ToRenderTargetBitmap(size, PixelFormats.Pbgra32)
                          .ToBitmap();
        }

        public static RenderTargetBitmap ToRenderTargetBitmap(this UIElement element, Size size)
        {
            return element.ToRenderTargetBitmap(size, PixelFormats.Pbgra32);
        }

        public static RenderTargetBitmap ToRenderTargetBitmap(this UIElement element, Size size, PixelFormat pixelFormat)
        {
            var result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, pixelFormat);
            element.Measure(size);
            element.Arrange(new Rect(size));
            result.Render(element);
            return result;
        }

        public static Bitmap ToBitmap(this RenderTargetBitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                return new Bitmap(stream);
            }
        }

        public static void SaveImage(this UIElement element, Size size, string fileName)
        {
            SaveImage(element, size, GetPixelFormat(fileName), fileName);
        }

        public static void SaveImage(this UIElement element, Size size, PixelFormat pixelFormat, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                var renderTargetBitmap = element.ToRenderTargetBitmap(size, pixelFormat);
                var encoder = GetEncoder(fileName);
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                encoder.Save(stream);
            }
        }

        private static BitmapEncoder GetEncoder(string fileName)
        {
            if (string.Equals(Path.GetExtension(fileName), ".png", StringComparison.OrdinalIgnoreCase))
            {
                return new PngBitmapEncoder();
            }

            throw new ArgumentException($"Cannot save {Path.GetExtension(fileName)}");
        }

        private static PixelFormat GetPixelFormat(string fileName)
        {
            if (string.Equals(Path.GetExtension(fileName), ".png", StringComparison.OrdinalIgnoreCase))
            {
                return PixelFormats.Pbgra32;
            }

            throw new ArgumentException($"Cannot save {Path.GetExtension(fileName)}");
        }

        private static Size Size(this Image bitmap)
        {
            return new Size(bitmap.Width, bitmap.Height);
        }

        private static PixelFormat PixelFormat(this Image bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormats.Pbgra32;
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Indexed:
                case System.Drawing.Imaging.PixelFormat.Gdi:
                case System.Drawing.Imaging.PixelFormat.Alpha:
                case System.Drawing.Imaging.PixelFormat.PAlpha:
                case System.Drawing.Imaging.PixelFormat.Extended:
                case System.Drawing.Imaging.PixelFormat.Canonical:
                case System.Drawing.Imaging.PixelFormat.Undefined:
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Max:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
