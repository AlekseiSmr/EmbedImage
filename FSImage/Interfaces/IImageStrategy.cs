using System.Drawing;
using FSImage.Classes;

namespace FSImage.Interfaces
{
    public interface IImageStrategy<T, TP>
    {
        ConvertedImage<T, TP> Convert(Bitmap image);
    }
}