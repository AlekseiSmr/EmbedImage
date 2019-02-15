using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSImage.Classes
{
    public class ConvertedImage<T, Pal>
    {
        public IEnumerable<T> Image { get; set; }
        public IEnumerable<Palette<Pal>> Palette { get; set; }
    }
}
