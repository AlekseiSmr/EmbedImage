using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FSImage.Classes;
using FSImage.Enums;
using FSImage.Interfaces;

namespace FSImage.FileSaveStrategies
{
    internal class Image565LittleEndianSaver : IFileSaver
    {
        private readonly FileType _fileType;

        public Image565LittleEndianSaver(FileType fileType)
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
                file.WriteLine("const unsigned char image[] = {");

                file.WriteLine("0x{0:X2}, //Ширина", (bitmap.Width >> 8) & 0xFF);
                file.WriteLine("0x{0:X2}, //Ширина", (bitmap.Width >> 0) & 0xFF);

                file.WriteLine("0x{0:X2}, //Высота", (bitmap.Height >> 8) & 0xFF);
                file.WriteLine("0x{0:X2}, //Высота", (bitmap.Height >> 0) & 0xFF);

                file.WriteLine("//Размер битмапа");
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 24) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 16) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 8) & 0xFF);
                file.WriteLine("0x{0:X2},", (convertedImage.Image.Count() >> 0) & 0xFF);
                file.WriteLine("\n");

                //Сохраняем тело картинки
                for (var cnt = 0; cnt < convertedImage.Image.Count(); cnt++)
                {
                    file.Write("0x{0:X2}, ", (Convert.ToUInt16(convertedImage.Image.ElementAt(cnt)) >> 8) & 0xFF);
                    file.Write("0x{0:X2}, ", (Convert.ToUInt16(convertedImage.Image.ElementAt(cnt)) >> 0) & 0xFF);
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
                byte b1;
                byte b2;
                byte b3;

                byte b4;

                //Пишем ширину(2 байта)
                b1 = (byte) ((bitmap.Width >> 8) & 0xFF);
                b2 = (byte) ((bitmap.Width >> 0) & 0xFF);
                file.Write(b2);
                file.Write(b1);

                //Пишем высоту(2 байта)
                b1 = (byte) ((bitmap.Height >> 8) & 0xFF);
                b2 = (byte) ((bitmap.Height >> 0) & 0xFF);
                file.Write(b2);
                file.Write(b1);

                //Размер битмапа
                b1 = (byte) ((convertedImage.Image.Count() >> 24) & 0xFF);
                b2 = (byte) ((convertedImage.Image.Count() >> 16) & 0xFF);
                b3 = (byte) ((convertedImage.Image.Count() >> 8) & 0xFF);
                b4 = (byte) ((convertedImage.Image.Count() >> 0) & 0xFF);

                file.Write(b4);
                file.Write(b3);
                file.Write(b2);
                file.Write(b1);

                //Сохраняем битмап
                for (var j = 0; j < convertedImage.Image.Count(); j++)
                    file.Write(Convert.ToByte(convertedImage.Image.ElementAt(j)));

                file.Flush();
            }
            return result;
        }
    }
}