using FSImage.Interfaces;

namespace FSImage.FileSaveStrategies
{
    public class ImageSaver
    {
        private IFileSaver _saver;

        public ImageSaver(IFileSaver saver)
        {
            _saver = saver;
        }

        public void Save()
        {
        }
    }
}