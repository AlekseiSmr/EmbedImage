using FSImage.Enums;
using FSImage.FileSaveStrategies;
using FSImage.Interfaces;

namespace FSImage.Factory
{
    public class ImageSaverFactory
    {
        private readonly Endian _endian;
        private readonly FileType _fileType;
        private readonly ImageFormat _fomat;

        public ImageSaverFactory(FileType fileType, Endian endian, ImageFormat fomat)
        {
            _fileType = fileType;
            _endian = endian;
            _fomat = fomat;
        }

        public IFileSaver Select()
        {
            switch (_fomat)
            {
                case ImageFormat.Rgb565PalRle:
                case ImageFormat.Rgb565Pal:
                    switch (_endian)
                    {
                        case Endian.Big:
                            return new Rgb565BigEndianWithPaletteSaver(_fileType);
                        case Endian.Little:
                            return new Rgb565LittleEndianWithPaletteSaver(_fileType);
                        default:
                            return null;
                    }

                case ImageFormat.Bgr565:
                case ImageFormat.Rgb565:
                    switch (_endian)
                    {
                        case Endian.Big:
                            return new Image565BigEndianSaver(_fileType);
                        case Endian.Little:
                            return new Image565LittleEndianSaver(_fileType);
                        default:
                            return null;
                    }
                default:
                    return new Image565LittleEndianSaver(_fileType);
            }
        }
    }
}