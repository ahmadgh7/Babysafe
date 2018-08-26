namespace BsPiClient.MLX90640
{
    internal class Pixel
    {
        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }

        public Pixel(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}