using FSImage.Enums;
using FSImage.ImageStrategies;

namespace FSImage.Factory
{
    public class ImageFactory
    {
        private readonly ImageFormat _imageFormat;

        public ImageFactory(ImageFormat imageFormat)
        {
            _imageFormat = imageFormat;
        }

        public IConverter GetConverter()
        {
            switch (_imageFormat)
            {
                case ImageFormat.Rgb565Pal:
                    return new EmbeddedImageConverter<byte, ushort>(new Rgb565WithPaletteStrategy());
                case ImageFormat.Rgb565:
                    return new EmbeddedImageConverter<ushort, byte>(new Rgb565Strategy());
                case ImageFormat.Bgr565:
                    return new EmbeddedImageConverter<ushort, byte>(new Bgr565Strategy());
                case ImageFormat.Rgb565PalRle:
                    return new EmbeddedImageConverter<byte, ushort>(new Rgb565WithPaletteRleStrategy());
                case ImageFormat.Rgb888:
                    return new EmbeddedImageConverter<int, byte>(new Rgb888Strategy());
                case ImageFormat.Rgb888Pal:
                    return new EmbeddedImageConverter<byte, int>(new Rgb888WithPaletteStrategy());
                case ImageFormat.Bgr888:
                    return new EmbeddedImageConverter<int, byte>(new Bgr888Strategy());
                case ImageFormat.Argb4444:
                    return new EmbeddedImageConverter<ushort, byte>(new Argb4444Strategy());
                default:
                    return new EmbeddedImageConverter<byte, ushort>(new Rgb565WithPaletteStrategy());
            }
        }
    }
}