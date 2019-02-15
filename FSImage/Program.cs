using System;
using FSImage.Enums;

namespace FSImage
{
    internal class Program
    {
        //Входные параметры
        private static string _sWorkType = "";
        private static string _sEndian = "";
        private static string _sOutputf = "";
        private static string _sClrForm = "";
        private static bool IsChain { get; set; }

        private static readonly string Error = "Ошибка аргумента! Используйте команду -?";

        private static readonly string usageString =
            "Usage: create <arguments>\n\n" +
            "  -w monohrome        Work with monohrome image.\n" +
            "  -w color            Work with color image.\n" +
            "  -w f_image          Work with flash image.\n\n" +
            "  -o vert             Vertical orientation of byte.\n" +
            "  -o horiz            Horizontal orientation of byte.\n\n" +
            "  -e big              Big_Endian.\n" +
            "  -e little           Little_Endian.\n\n" +
            "  -f c                Output \"C\" massive files.\n" +
            "  -f b                Output binary files.\n\n" +
            "  -b 1                Image size 1 byte.\n" +
            "  -b 2                Image size 2 byte.\n\n" +
            "  -cf RGB_565         Image format.\n" +
            "  -cf BGR_565         Image format.\n" +
            "  -cf RGB_888         Image format.\n" +
            "  -cf BGR_888         Image format.\n" +
            "  -cf RGB_565_PAL     Image format.\n" +
            "  -cf RGB_565_PAL_RLE Image format.\n" +
            "  -cf ARGB_4444       Image format.\n" +
            "  -h  0x------        Hex offset.\n";

        /// <summary>
        /// </summary>
        public static void Color_task()
        {
            var taskClass = new TaskManager();
            var iEnd = Endian.Little;
            var iForm = FileType.TypeC;
            var iClrForm = ImageFormat.Rgb565;

            //Определяем Endian
            if (_sEndian == "big")
            {
                iEnd = Endian.Big;
            }
            else if (_sEndian == "little")
            {
                iEnd = Endian.Little;
            }
            else
            {
                Console.WriteLine(Error);
                Environment.Exit(0);
            }

            //Определяем Выходной формат
            if (_sOutputf == "c")
            {
                iForm = FileType.TypeC;
            }
            else if (_sOutputf == "bin")
            {
                iForm = FileType.TypeBin;
            }
            else
            {
                Console.WriteLine(Error);
                Environment.Exit(0);
            }

            //Определяем формат картинки после конвертации
            if (_sClrForm == "RGB_565")
            {
                iClrForm = ImageFormat.Rgb565;
            }
            else if (_sClrForm == "BGR_565")
            {
                iClrForm = ImageFormat.Bgr565;
            }
            else if (_sClrForm == "RGB_888")
            {
                iClrForm = ImageFormat.Rgb888;
            }
            else if (_sClrForm == "BGR_888")
            {
                iClrForm = ImageFormat.Bgr888;
            }
            else if (_sClrForm == "RGB_565_PAL")
            {
                iClrForm = ImageFormat.Rgb565Pal;
            }
            else if (_sClrForm == "RGB_565_PAL_RLE")
            {
                iClrForm = ImageFormat.Rgb565PalRle;
            }
            else if(_sClrForm == "ARGB_4444")
            {
                iClrForm = ImageFormat.Argb4444;
            }
            else
            {
                Console.WriteLine(Error);
                Environment.Exit(0);
            }
            taskClass.Color_task(iEnd, iForm, iClrForm, IsChain);
        }


        /// <summary>
        /// </summary>
        public static void FS_task()
        {
            var taskClass = new TaskManager();
            const int iOrient = 0;
            var iEnd = 0;
            var iForm = 0;
            const int iByte = 0;

            //Определяем Выходной формат
            if (_sOutputf == "c")
            {
                iForm = 1;
            }
            else if (_sOutputf == "bin")
            {
                iForm = 2;
            }
            else
            {
                Console.WriteLine(Error);
                Environment.Exit(0);
            }

            //Определяем Endian
            if (_sEndian == "big")
            {
                iEnd = 1;
            }
            else if (_sEndian == "little")
            {
                iEnd = 2;
            }
            else
            {
                Console.WriteLine(Error);
                Environment.Exit(0);
            }

            taskClass.ConvertToFS(iOrient, iEnd, iForm, iByte);
        }


        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            //Нет параметров - выход из приложения
            if (args.Length == 0)
            {
                Console.WriteLine("Не переданы аргументы! Используйте команду -? Завершение программы...");
                Environment.Exit(0);
            }

            //Проверка входных аргументов
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-?")
                {
                    Console.Write(usageString);
                    Environment.Exit(0);
                }

                //Забираем аргументы
                if (args[i] == "-w") _sWorkType = args[i + 1];
                if (args[i] == "-e") _sEndian = args[i + 1];
                if (args[i] == "-f") _sOutputf = args[i + 1];
                if (args[i] == "-cf") _sClrForm = args[i + 1];
                if (args[i] == "--chain") IsChain = true;
            }

            if (_sWorkType == "color")
            {
                Color_task();
            }
            else if (_sWorkType == "f_image")
            {
                FS_task();
            }
            else
            {
                Console.WriteLine(usageString);
                Environment.Exit(0);
            }
        }
    }
}