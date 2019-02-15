using System.Collections.Generic;
using System.Drawing;
using FSImage.Interfaces;
using System.Linq;
using FSImage.Classes;
using FSImage.Factory;

namespace FSImage.ImageStrategies
{
    /// <summary>
    ///     Менеджер стратегий преобрахования картинки
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmbeddedImageConverter<T,TP> : IConverter
    {
        private readonly IImageStrategy<T, TP> _strategy;

        /// <summary>
        ///     .ctor
        /// </summary>
        /// <param name="strategy"></param>
        public EmbeddedImageConverter(IImageStrategy<T, TP> strategy)
        {
            _strategy = strategy;
        }

        /// <summary>
        ///     Конвертирует картинку в заданный формат
        /// </summary>
        public ConvertedImage<object,object> Convert(Bitmap bitmap)
        {
            var result = _strategy.Convert(bitmap);
            return new ConvertedImage<object, object>
            {
                Image = result.Image.Cast<object>(),
                Palette = result.Palette == null ? null : result.Palette.Select(x => new Palette<object> {Color = x})
            };
        }
    }
}