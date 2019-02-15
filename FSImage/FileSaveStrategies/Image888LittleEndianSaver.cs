using System.Drawing;
using FSImage.Classes;
using FSImage.Enums;
using FSImage.Interfaces;

namespace FSImage.FileSaveStrategies
{
    internal class Image888LittleEndianSaver : IFileSaver
    {
        private readonly FileType _fileType;

        public Image888LittleEndianSaver(FileType fileType)
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
            return null;
        }
    }
}