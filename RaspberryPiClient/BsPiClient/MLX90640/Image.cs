using System;
using System.Drawing;
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
            var isPresenceDetected = redPixels > 50;
            Console.WriteLine(isPresenceDetected ? "Presence detected, Number of red pixels: " + redPixels : "No presence detected, Number of red pixels: " + redPixels);
            return isPresenceDetected;
        }

        private static bool IsRedPixel(Pixel pixel)
        {
            var color = Color.FromArgb(0,pixel.Red, pixel.Green, pixel.Blue);
            return color.GetHue() < 60;
        }
    }
}
