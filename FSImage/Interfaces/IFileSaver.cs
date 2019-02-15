using System.Drawing;
using FSImage.Classes;

namespace FSImage.Interfaces
{
    public interface IFileSaver
    {
        ImageInfo Save(Bitmap bitmap, ConvertedImage<object, object> convertedImage, string fileName);
    }
}
