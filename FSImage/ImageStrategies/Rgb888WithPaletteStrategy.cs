using System.Collections.Generic;
using System.Drawing;
using FSImage.Classes;
using FSImage.Interfaces;

namespace FSImage.ImageStrategies
{
    internal class Rgb888WithPaletteStrategy : IImageStrategy<byte, int>
    {
        public ConvertedImage<byte, int> Convert(Bitmap image)
        {
            var pallete888 = new List<Palette<int>>();
            var pal888 = new List<int>();

            var bitmap = new byte[image.Width * image.Height];

            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            {
                var pixelColor = image.GetPixel(x, y);

                //Получение цвета по составляющим
                var rbyte = pixelColor.R;
                var gbyte = pixelColor.G;
                var bbyte = pixelColor.B;

                //Составление палитры
                var clr888 = rbyte << 24;
                clr888 |= gbyte << 16;
                clr888 |= bbyte << 8;

                pal888.Add(clr888);

                var find = 0;
                var pos = 0;
                for (var q = 0; q < pal888.Count; q++)
                    if (pal888[q] == clr888)
                    {
                        find = 1;
                        pos = q;
                        break;
                    }

                if (find == 0)
                    pallete888.Add(new Palette<int> {Color = clr888});

                //Составление тела картинки
                if (find == 0)
                    bitmap[x + y * image.Height] = (byte) pallete888.Count;
                else
                    bitmap[x + y * image.Height] = (byte) pos;
            }

            return new ConvertedImage<byte, int> {Image = bitmap, Palette = pallete888};
        }
    }
}