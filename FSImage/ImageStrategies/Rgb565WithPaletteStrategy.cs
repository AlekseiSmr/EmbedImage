﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FSImage.Classes;
using FSImage.Interfaces;

namespace FSImage
{
    public class Rgb565WithPaletteStrategy : IImageStrategy<byte, ushort>
    {
        /// <summary>
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public ConvertedImage<byte, ushort> Convert(Bitmap image)
        {
            var palette = new List<Palette<ushort>>();

            var bitmap = new byte[image.Width * image.Height];

            //Пробегание всей картинки
            //Построчно горизонтально
            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            {
                var pixelColor = image.GetPixel(x, y);

                //Получение цвета по составляющим
                var rbyte = pixelColor.R;
                var gbyte = pixelColor.G;
                var bbyte = pixelColor.B;

                //Составление палитры
                var clr565 = (ushort) (((rbyte >> 3) & 0x1F) << 11);
                clr565 |= (ushort) ((ushort) ((gbyte >> 2) & 0x3F) << 5);
                clr565 |= (ushort) (((bbyte >> 3) & 0x1F) << 0);

                var find = 0;
                var pos = 0;
                for (var q = 0; q < palette.Count; q++)
                {
                    var temp = palette[q];
                    if (temp.Color == clr565)
                    {
                        find = 1;
                        pos = q;
                        break;
                    }
                }

                if (find == 0)
                    palette.Add(new Palette<ushort> {Color = clr565});

                //Составление тела картинки
                if (find == 0)
                    bitmap[x + y * image.Height] = (byte) palette.Count;
                else
                    bitmap[x + y * image.Height] = (byte) pos;
            }

            var result = new ConvertedImage<byte, ushort>
            {
                Image = bitmap.ToList(),
                Palette = palette
            };

            return result;
        }
    }
}