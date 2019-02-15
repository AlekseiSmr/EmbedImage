using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FSImage.Classes;
using FSImage.Enums;
using FSImage.Interfaces;

namespace FSImage.FileSaveStrategies
{
    internal class Image888BigEndianWithPaletteSaver : IFileSaver
    {
        private readonly FileType _fileType;

        public Image888BigEndianWithPaletteSaver(FileType fileType)
        {
            _fileType = fileType;
        }

        public ImageInfo Save(Bitmap bitmap, ConvertedImage<object, object> convertedImage, string fileName)
        {
            switch (_fileType)
            {
                case FileType.TypeC:
                    return SaveCFile(bitmap, convertedImage, fileName);
                default:
                    return null;
            }
        }

        public ImageInfo SaveCFile(Bitmap bitmap, ConvertedImage<object, object> convertedImage, string fileName)
        {
            //Формирование и сохранение файла
            var test = new FileInfo(fileName);
            var name = test.Name;
            var onlyFileName = name.Split('.').FirstOrDefault();
            var ext = test.Extension;
            var dir = AppDomain.CurrentDomain.BaseDirectory + "OUTPUT\\";
            var fName = dir + name.Replace(ext, ".c");

            var result = new ImageInfo
            {
                ImgName = onlyFileName,
                Width = bitmap.Width,
                Height = bitmap.Height
            };

            using (var file = new StreamWriter(fName))
            {
                file.WriteLine("/*Картинка с палитрой со сжатием.*/");
                file.WriteLine("const unsigned char image[] = {");

                //Ширина картинки
                file.WriteLine("0x{0:X2},", (bitmap.Width >> 8) & 0xFF);
                file.WriteLine("0x{0:X2},", (bitmap.Width >> 0) & 0xFF);

                //Высота картинки
                file.WriteLine("0x{0:X2},", (bitmap.Height >> 8) & 0xFF);
                file.WriteLine("0x{0:X2},", (bitmap.Height >> 0) & 0xFF);

                //Размер палитры
                file.WriteLine("0x{0:X2},", (convertedImage.Palette.Count() >> 8) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Palette.Count() >> 0) & 0xFF);

                //Размер битмапа
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 24) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 16) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 8) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 0) & 0xFF);

                //Сохраняем палитру(2 байта цвет + 1 байт прозрачность)
                for (var cnt = 0; cnt < convertedImage.Palette.Count(); cnt++)
                {
                    var temp = convertedImage.Palette.ElementAt(cnt);
                    file.Write("0x{0:X2}, ", (Convert.ToUInt16(temp.Color) >> 8) & 0xFF);
                    file.Write("0x{0:X2}, ", (Convert.ToUInt16(temp.Color) >> 0) & 0xFF);
                    if ((cnt + 1) % 10 == 0)
                        file.WriteLine("");
                }
                file.WriteLine("\n");


                //Сохраняем тело картинки
                file.WriteLine("//Битмап:");
                for (var cnt = 0; cnt < convertedImage.Image.Count(); cnt++)
                {
                    file.Write("0x{0:X2}, ", convertedImage.Image.ElementAt(cnt));
                    if ((cnt + 1) % 10 == 0)
                        file.WriteLine("");
                }
                file.WriteLine("};");
                file.Flush();
            }
            return result;
        }
    }
}