using FSImage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSImage.Classes;
using System.Drawing;

namespace FSImage.ImageStrategies
{
    public class Argb4444Strategy : IImageStrategy<ushort, byte>
    {
        public ConvertedImage<ushort, byte> Convert(Bitmap image)
        {
            var result = new List<ushort>();

            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var pixelColor = image.GetPixel(x, y);

                    var alpha = pixelColor.A & 0x0F;
                    var rbyte = pixelColor.R & 0x0F;
                    var gbyte = pixelColor.G & 0x0F;
                    var bbyte = pixelColor.B & 0x0F;

                    ushort color;
                    color = (ushort)(gbyte << 12); // green
                    color |= (ushort)(bbyte << 8); //blue
                    color |= (ushort)(alpha << 4); //alpha
                    color |= (ushort)(rbyte); //red

                    result.Add(color);
                }
            }

            return new ConvertedImage<ushort, byte> { Image = result, Palette = null };
        }
    }
}
