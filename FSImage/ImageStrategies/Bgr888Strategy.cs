using System.Collections.Generic;
using System.Drawing;
using FSImage.Classes;
using FSImage.Interfaces;

namespace FSImage.ImageStrategies
{
    public class Bgr888Strategy : IImageStrategy<int, byte>
    {
        public ConvertedImage<int, byte> Convert(Bitmap image)
        {
            var result = new List<int>();
            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            {
                var pixelColor = image.GetPixel(x, y);

                var rbyte = pixelColor.R;
                var gbyte = pixelColor.G;
                var bbyte = pixelColor.B;

                var clr = bbyte << 24;
                clr |= gbyte << 16;
                clr |= rbyte << 0;

                result.Add(clr);
            }
            return new ConvertedImage<int, byte> {Image = result, Palette = null};
        }
    }
}