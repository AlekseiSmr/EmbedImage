using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FSImage.Classes;
using FSImage.Enums;
using FSImage.Interfaces;

namespace FSImage.FileSaveStrategies
{
    internal class Image888LittleEndianWithPaletteSaver : IFileSaver
    {
        private readonly FileType _fileType;

        public Image888LittleEndianWithPaletteSaver(FileType fileType)
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
            return null;
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

                //Размер палитры
                file.Write(convertedImage.Palette.Count());

                //Размер битмапа
                var t1 = (ushort) ((convertedImage.Image.Count() >> 16) & 0xFFFF);
                var t2 = (ushort) ((convertedImage.Image.Count() >> 0) & 0xFFFF);
                file.Write(t1);
                file.Write(t2);

                //Сохраняем палитру
                for (var j = 0; j < convertedImage.Palette.Count(); j++)
                {
                    var temp = convertedImage.Palette.ElementAt(j);
                    file.Write((int) temp.Color);
                }

                //Сохраняем битмап
                for (var j = 0; j < convertedImage.Image.Count(); j++)
                    file.Write(Convert.ToInt32(convertedImage.Image.ElementAt(j)));

                file.Flush();
            }
            return result;
        }
    }
}