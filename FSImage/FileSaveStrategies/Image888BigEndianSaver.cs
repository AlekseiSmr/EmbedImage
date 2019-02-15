using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FSImage.Classes;
using FSImage.Enums;
using FSImage.Interfaces;

namespace FSImage.FileSaveStrategies
{
    internal class Image888BigEndianSaver : IFileSaver
    {
        private readonly FileType _fileType;

        public Image888BigEndianSaver(FileType fileType)
        {
            _fileType = fileType;
        }

        public ImageInfo Save(Bitmap bitmap, ConvertedImage<object, object> convertedImage, string fileName)
        {
            switch (_fileType)
            {
                case FileType.TypeC:
                    return SaveCFile(bitmap, convertedImage, fileName);
                case FileType.TypeBin:
                   return SaveBinFile(bitmap, convertedImage, fileName);
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
                file.WriteLine("/*Картинка без палитры без сжатия.*/");
                file.WriteLine("const unsigned int image[] = {");

                file.WriteLine("0x{0:X4}, //Ширина", bitmap.Width);
                file.WriteLine("0x{0:X4}, //Высота", bitmap.Height);

                file.WriteLine("//Размер битмапа");
                file.WriteLine("0x{0:X4},", (convertedImage.Image.Count() >> 16) & 0xFFFF);
                file.WriteLine("0x{0:X4},", (convertedImage.Image.Count() >> 0) & 0xFFFF);

                file.WriteLine("\n");

                //Сохраняем тело картинки
                for (var cnt = 0; cnt < convertedImage.Image.Count(); cnt++)
                {
                    file.Write("0x{0:X4}, ", (Convert.ToUInt16(convertedImage.Image.ElementAt(cnt)) >> 16) & 0xFFFF);
                    file.Write("0x{0:X4}, ", (Convert.ToUInt16(convertedImage.Image.ElementAt(cnt)) >> 00) & 0xFFFF);
                    if ((cnt + 1) % 10 == 0)
                        file.WriteLine("");
                }
                file.WriteLine("};");
                file.Flush();
            }
            return result;
        }

        private ImageInfo SaveBinFile(Bitmap bitmap, ConvertedImage<object, object> convertedImage, string fileName)
        {
            //Формирование и сохранение файла
            var test = new FileInfo(fileName);
            var name = test.Name;
            var onlyFileName = name.Split('.').FirstOrDefault();
            var ext = test.Extension;
            var dir = AppDomain.CurrentDomain.BaseDirectory + "OUTPUT\\";
            var fName = dir + name.Replace(ext, ".bin");

            var result = new ImageInfo
            {
                ImgName = onlyFileName,
                Width = bitmap.Width,
                Height = bitmap.Height
            };

            using (var file = new BinaryWriter(File.Open(fName, FileMode.Create)))
            {

                //Пишем ширину(2 байта)
                file.Write(bitmap.Width);

                //Пишем высоту(2 байта)
                file.Write(bitmap.Height);

                //Размер битмапа
                var t1 = (ushort) ((convertedImage.Image.Count() >> 16) & 0xFFFF);
                var t2 = (ushort) ((convertedImage.Image.Count() >> 0) & 0xFFFF);
                file.Write(t1);
                file.Write(t2);

                //Сохраняем битмап
                for (var j = 0; j < convertedImage.Image.Count(); j++)
                    file.Write(Convert.ToInt32(convertedImage.Image.ElementAt(j)));

                file.Flush();
            }
            return result;
        }
    }
}