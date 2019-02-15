using System.Collections.Generic;
using System.Drawing;
using FSImage.Classes;
using FSImage.Interfaces;

namespace FSImage.ImageStrategies
{
    public class Rgb565Strategy : IImageStrategy<ushort, byte>
    {
        public ConvertedImage<ushort, byte> Convert(Bitmap image)
        {
            var result = new List<ushort>();

            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            {
                var pixelColor = image.GetPixel(x, y);

                var rbyte = pixelColor.R;
                var gbyte = pixelColor.G;
                var bbyte = pixelColor.B;

                var clr = (ushort) (((rbyte >> 3) & 0x1F) << 11);
                clr |= (ushort) (((gbyte >> 2) & 0x3F) << 5);
                clr |= (ushort) (((bbyte >> 3) & 0x1F) << 0);

                result.Add(clr);
            }
            return new ConvertedImage<ushort, byte> {Image = result, Palette = null};
        }
    }
}