namespace BsPiClient.MLX90640
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    internal static class Mlx90640Helper
    {
        private const int Height = 24;
        private const int Width = 32;

        public static void CreateImages(string path)
        {
            CreateRawData(path);

            var filePaths = Directory.GetFiles(path, "*_modified.txt");
            foreach (var filePath in filePaths)
            {
                var pixels = CreatePixelArray(filePath);
                WriteBmp(filePath, pixels);
            }
        }

        public static Image GetLatestImage(string path)
        {
            CreateRawData(path);

            var filePaths = Directory.GetFiles(path, "*.temp.txt");
            Array.Sort(filePaths);
            return new Image(CreatePixelArray(Path.Combine(Path.GetDirectoryName(filePaths[0]), Path.GetFileNameWithoutExtension(filePaths[0]) + "_modified.txt")));
        }

        public static void CleanData(string path)
        {
            try
            {
                DeleteFiles(path);
                DeleteFiles(Path.Combine(path, "output"));
                Debug.WriteLine("Cleaned all data");

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clean data: " + ex);
            }
        }

        private static void DeleteFiles(string path)
        {
            var filePaths = Directory.GetFiles(path);
            foreach (var filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }

        private static void CreateRawData(string path)
        {
            var filePaths = Directory.GetFiles(path, "*.temp.txt");
            foreach (var filePath in filePaths)
            {
                if (filePath == null)
                    continue;

                if (Path.GetFileNameWithoutExtension(filePath).EndsWith("_modified"))
                    continue;

                using (var outFile = new StreamWriter(Path.Combine(path, Path.GetFileNameWithoutExtension(filePath) + "_modified.txt")))
                {
                    var fileLines = File.ReadAllLines(filePath);
                    foreach (var fileLine in fileLines)
                    {
                        if (string.IsNullOrWhiteSpace(fileLine))
                            continue;
                        var line = fileLine.Split();
                        foreach (var pixel in line)
                        {
                            if (string.IsNullOrWhiteSpace(pixel))
                                continue;
                            outFile.WriteLine(pixel);
                        }
                    }
                }
            }
        }

        private static Pixel[,] CreatePixelArray(string path)
        {
            var pixels = new Pixel[24, 32];
            var lines = File.ReadAllLines(path);
            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    var line = lines[y * Width + x];
                    var tmperatures = line.Split(',');
                    foreach (var tmperature in tmperatures)
                    {
                        var val = Math.Abs((int.Parse(tmperature) - 150) / (double)(350 - 150));
                        if (val < 0.5)
                        {
                            var green = (int)Math.Floor(val * 2 * 255);
                            if (green > 255)
                                green = 255;
                            if (green < 0)
                                green = 0;

                            var blue = (int)Math.Floor((1 - val * 2) * 255);
                            if (blue > 255)
                                blue = 255;
                            if (blue < 0)
                                blue = 0;

                            pixels[y, x] = new Pixel(0, green, blue);
                        }
                        else if (val >= 0.5)
                        {
                            var red = (int)Math.Floor((val - 0.5) * 2 * 255);
                            if (red > 255)
                                red = 255;
                            if (red < 0)
                                red = 0;

                            var green = (int)Math.Floor((1 - (val - 0.5) * 2) * 255);
                            if (green > 255)
                                green = 255;
                            if (green < 0)
                                green = 0;

                            pixels[y, x] = new Pixel(red, green, 0);
                        }
                    }
                }

            return pixels;
        }

        private static void WriteBmp(string path, Pixel[,] pixels)
        {
            var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);

            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(0, pixels[y, x].Red, pixels[y, x].Green, pixels[y, x].Blue));
                }

            bitmap.Save(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".bmp"));
        }
    }
}
