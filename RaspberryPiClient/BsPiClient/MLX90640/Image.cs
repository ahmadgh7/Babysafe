using System;
using System.Linq;

namespace BsPiClient.MLX90640
{
    internal class Image
    {
        private const int Height = 24;
        private const int Width = 32;
        private readonly Pixel[,] _pixels;

        public Image(Pixel[,] pixels)
        {
            if (pixels.GetLength(0) != Height || pixels.GetLength(1) != Width)
                throw new ArgumentException(nameof(pixels));

            _pixels = pixels;
        }

        public bool IsPresenceDetected()
        {
            var redPixels = _pixels.Cast<Pixel>().Count(IsRedPixel);
            return redPixels > 50;
        }

        private static bool IsRedPixel(Pixel pixel)
        {
            return pixel.Red > 127;
        }
    }
}
