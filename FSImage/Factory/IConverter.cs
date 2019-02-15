using System.Drawing;
using FSImage.Classes;

namespace FSImage.Factory
{
    public interface IConverter
    {
        ConvertedImage<object, object> Convert(Bitmap bitmap);
    }
}