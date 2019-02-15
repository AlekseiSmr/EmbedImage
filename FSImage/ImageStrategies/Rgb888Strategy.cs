using System.Collections.Generic;
using System.Drawing;
using FSImage.Classes;
using FSImage.Interfaces;

namespace FSImage.ImageStrategies
{
    public class Rgb888Strategy : IImageStrategy<int, byte>
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

                var clr = 0;
                clr |= rbyte << 16;
                clr |= gbyte << 8;
                clr |= bbyte << 0;

                result.Add(clr);
            }
            return new ConvertedImage<int, byte> {Image = result, Palette = null};
        }
    }
}