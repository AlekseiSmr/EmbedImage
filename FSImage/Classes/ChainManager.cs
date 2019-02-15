using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FSImage.Classes
{
    public class ChainManager
    {
        private readonly List<ImageInfo> _imagesInfos;

        public ChainManager()
        {
            _imagesInfos = new List<ImageInfo>();
        }

        public void Add(ImageInfo info)
        {
            _imagesInfos.Add(info);
        }

        /// <summary>
        ///     Create arrays of image animation informations
        /// </summary>
        public void CreateChain()
        {
            var fileName = AppDomain.CurrentDomain.BaseDirectory + "OUTPUT\\" + "chain.h";

            using (var file = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                file.WriteLine("#ifndef __CHAIN_H");
                file.WriteLine("#define __CHAIN_H");

                file.WriteLine();

                foreach (var name in _imagesInfos)
                {
                    file.WriteLine("#include \"{0}.h\"", name.ImgName);
                }

                file.WriteLine();


                file.WriteLine("const unsigned char* animationChain[] =");
                var fileNames = _imagesInfos.Select(x => "img" + x.ImgName).ToList();
                var fileNameItems = string.Join(",", fileNames.ToArray());
                file.WriteLine("{{{0}}};", fileNameItems);

                file.WriteLine();

                file.WriteLine("const long animationChainWidths[] =");
                var fileWidths = _imagesInfos.Select(x => x.Width.ToString()).ToList();
                var fileWidthsItems = string.Join(",", fileWidths.ToArray());
                file.WriteLine("{{{0}}};", fileWidthsItems);

                file.WriteLine();

                file.WriteLine("const long animationChainHeights[] =");
                var fileHeights = _imagesInfos.Select(x => x.Height.ToString()).ToList();
                var fileHeightsItems = string.Join(",", fileHeights.ToArray());
                file.WriteLine("{{{0}}};", fileHeightsItems);

                file.WriteLine();

                file.WriteLine("#endif");
            }
        }
    }
}