using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using FSImage.Classes;
using FSImage.Enums;
using FSImage.Factory;

namespace FSImage
{
    internal class TaskManager
    {
        private int _extPos;
        public int ExtTekPos;
        public int ShOffset;

        public int TekPos;


        /// <summary>
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static List<string> GetAllBmpFiles(string directory)
        {
            var fName = new List<string>();
            var png = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
            var bmp = Directory.GetFiles(directory, "*.bmp", SearchOption.AllDirectories);
            var jpg = Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories);
            var gif = Directory.GetFiles(directory, "*.gif", SearchOption.AllDirectories);

            for (var i = 0; i < png.Length; i++) fName.Add(png[i]);
            for (var i = 0; i < bmp.Length; i++) fName.Add(bmp[i]);
            for (var i = 0; i < jpg.Length; i++) fName.Add(jpg[i]);
            for (var i = 0; i < gif.Length; i++) fName.Add(gif[i]);

            return fName;
        }

        /// <summary>
        /// </summary>
        private static List<string> GetAllBinFiles(string directory)
        {
            var fName = new List<string>();
            var bin = Directory.GetFiles(directory, "*.bin", SearchOption.AllDirectories);

            for (var i = 0; i < bin.Length; i++)
                fName.Add(bin[i]);

            return fName;
        }

        /// <summary>
        /// </summary>
        /// <param name="iRecSize"></param>
        /// <returns></returns>
        private string ExtendedAddressHex(int iRecSize)
        {
            string sAddRecord = null;
            const byte size = 2;
            const byte mode = 4;
            const short addr = 0;
            var crc = 0;

            if (ExtTekPos * iRecSize + ShOffset >= 0xFFFF)
            {
                ExtTekPos = 0;
                _extPos++;

                crc += size;
                crc += addr;
                crc += mode;
                crc += _extPos;
                crc = (0 - crc) & 0xFF;

                var sTemp = string.Format(":{0:X2}", size);
                sAddRecord += sTemp;
                sTemp = string.Format("{0:X4}", addr);
                sAddRecord += sTemp;
                sTemp = string.Format("{0:X2}", mode);
                sAddRecord += sTemp;
                sTemp = string.Format("{0:X4}", _extPos);
                sAddRecord += sTemp;
                sTemp = string.Format("{0:X2}", crc);
                sAddRecord += sTemp;
                return sAddRecord;
            }

            return null;
        }


        /// <summary>
        /// </summary>
        /// <param name="myArray"></param>
        /// <param name="size"></param>
        /// <param name="recsize"></param>
        /// <returns></returns>
        private string SaveOneStringHex(List<byte> myArray, byte size, byte recsize)
        {
            const byte type = 0;

            var sOneRecord = "";
            var crc = 0;
            var data = new byte[size];

            crc += size;
            var addr = (short) (TekPos * recsize + ShOffset);
            crc += (addr >> 8) & 0xFF;
            crc += addr & 0xFF;

            crc += type;

            for (var k = 0; k < size; k++)
            {
                data[k] = myArray[TekPos * recsize + k];
                crc += data[k];
            }

            crc = (0 - crc) & 0xFF;

            var sTemp = string.Format(":{0:X2}", size);
            sOneRecord += sTemp;
            sTemp = string.Format("{0:X4}", addr);
            sOneRecord += sTemp;
            sTemp = string.Format("{0:X2}", type);
            sOneRecord += sTemp;

            for (var k = 0; k < size; k++)
            {
                sTemp = string.Format("{0:X2}", data[k]);
                sOneRecord += sTemp;
            }

            sTemp = string.Format("{0:X2}", crc);
            sOneRecord += sTemp;

            return sOneRecord;
        }


        /// <summary>
        /// </summary>
        /// <param name="myArray"></param>
        /// <param name="offset"></param>
        /// <param name="iRecSize"></param>
        private void SaveToHex(List<byte> myArray, long offset, int iRecSize)
        {
            var sFirstRecord = ":02000004";
            string sTemp;
            byte crc = 6;

            TekPos = 0;
            ExtTekPos = 0;

            long lArrSize = myArray.Count;

            var path = AppDomain.CurrentDomain.BaseDirectory + "OUTPUT\\" + "myhex.hex";
            var file = new StreamWriter(path);

            ShOffset = (int) (offset & 0xFFFF);
            _extPos = (int) ((offset >> 16) & 0xFFFF);
            sTemp = string.Format("{0:X4}", _extPos);

            sFirstRecord += sTemp;

            crc += (byte) ((_extPos >> 8) & 0xFF);
            crc += (byte) (_extPos & 0xFF);
            crc = (byte) (0 - crc);
            sTemp = string.Format("{0:X2}", crc);

            sFirstRecord += sTemp;

            file.WriteLine(sFirstRecord);

            var lFullStr = lArrSize / iRecSize;
            var iLastByte = lArrSize - lFullStr * iRecSize;


            for (long i = 0; i < lFullStr; i++)
            {
                var s1 = ExtendedAddressHex(0x20);
                var s2 = SaveOneStringHex(myArray, (byte) iRecSize, 0x20);
                if (s1 != null) file.WriteLine(s1);
                file.WriteLine(s2);

                TekPos++;
                ExtTekPos++;
            }

            var s3 = ExtendedAddressHex(0x20);
            if (s3 != null) file.WriteLine(s3);
            file.WriteLine(SaveOneStringHex(myArray, (byte) iLastByte, 0x20));
            file.WriteLine(":00000001FF");

            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Файл HEX создан!");
            Console.ForegroundColor = prevColor;
            file.Close();
        }


        /// <summary>
        ///     Собирает файловую систему
        /// </summary>
        public void ConvertToFS(int iOrient, int iEnd, int iForm, int iByte)
        {
            var id = 0;

            var body = new List<byte>(); //Тело файловой системы
            var header = new List<byte>(); //Заголовок файловой системы
            var fs = new List<byte>(); //Вся файловая система

            //Читаем все файлы bin в папке input для составления файловой системы
            var binFiles = GetAllBinFiles(AppDomain.CurrentDomain.BaseDirectory + "INPUT");
            var fileCnt = binFiles.Count;

            //Количество файлов
            switch (iEnd)
            {
                case 1:
                {
                    header.Add((byte) ((fileCnt >> 8) & 0xFF));
                    header.Add((byte) ((fileCnt >> 0) & 0xFF));
                }
                    break;

                case 2:
                {
                    header.Add((byte) ((fileCnt >> 0) & 0xFF));
                    header.Add((byte) ((fileCnt >> 8) & 0xFF));
                }
                    break;
            }

            long totalOffset = 0;
            //Поочередно открываем все файлы заданий
            for (var i = 0; i < fileCnt; i++)
            {
                try
                {
                    using (var taskFile = new BinaryReader(File.Open(binFiles[i], FileMode.Open), Encoding.ASCII))
                    {
                        Console.WriteLine("Файл открыт:" + binFiles[i] + "!");

                        //Смещение до файла -> (шапка + размер файла)
                        long offset = body.Count;
                        totalOffset = offset + 2 + fileCnt * 8;

                        while (taskFile.PeekChar() != -1)
                            body.Add(taskFile.ReadByte());
                    }
                }
                catch
                {
                    var prevColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Файл отсутствует! {0} Завершение программы...", binFiles[i]);
                    Console.ForegroundColor = prevColor;
                    Environment.Exit(0);
                }

                //Размер текущего файла в байтах
                var myFileInfo = new FileInfo(binFiles[i]);
                var fileByteSize = myFileInfo.Length;

                //ID
                switch (iEnd)
                {
                    case 1:
                    {
                        header.Add((byte) ((id >> 8) & 0xFF));
                        header.Add((byte) ((id >> 0) & 0xFF));
                        id++;

                        //Размер файла
                        //Header.Add((byte)((fileByteSize >> 24) & 0xFF));
                        header.Add((byte) ((fileByteSize >> 16) & 0xFF));
                        header.Add((byte) ((fileByteSize >> 8) & 0xFF));
                        header.Add((byte) ((fileByteSize >> 0) & 0xFF));

                        //Сдвиг
                        //Header.Add((byte)((TotalOffset >> 24) & 0xFF));
                        header.Add((byte) ((totalOffset >> 16) & 0xFF));
                        header.Add((byte) ((totalOffset >> 8) & 0xFF));
                        header.Add((byte) ((totalOffset >> 0) & 0xFF));
                    }
                        break;

                    case 2:
                    {
                        header.Add((byte) ((id >> 0) & 0xFF));
                        header.Add((byte) ((id >> 8) & 0xFF));
                        id++;

                        //Размер файла
                        header.Add((byte) ((fileByteSize >> 0) & 0xFF));
                        header.Add((byte) ((fileByteSize >> 8) & 0xFF));
                        header.Add((byte) ((fileByteSize >> 16) & 0xFF));
                        //Header.Add((byte)((fileByteSize >> 24) & 0xFF));

                        //Сдвиг
                        header.Add((byte) ((totalOffset >> 0) & 0xFF));
                        header.Add((byte) ((totalOffset >> 8) & 0xFF));
                        header.Add((byte) ((totalOffset >> 16) & 0xFF));
                        //Header.Add((byte)((TotalOffset >> 24) & 0xFF));
                    }
                        break;
                }
            }

            fs.AddRange(header);
            fs.AddRange(body);
            body.Clear();

            //Сохраняем файл файловой системы 
            var dir = AppDomain.CurrentDomain.BaseDirectory + "OUTPUT\\";
            var fName = dir + "fs.bin";
            var binFile = new BinaryWriter(File.Open(fName, FileMode.Create));

            binFile.Write(fs.ToArray());

            binFile.Flush();
            binFile.Close();

            //Сохраняем заголовочный файл
            fName = dir + "header.h";
            var hFile = new StreamWriter(fName);
            hFile.WriteLine("typedef enum _FLASH_ID");
            hFile.WriteLine("{");

            for (var i = 0; i < fileCnt; i++)
                hFile.WriteLine("    Bitmap" + i + " = " + i + ",");

            hFile.WriteLine("}_FLASH_ID;");
            hFile.Flush();
            hFile.Close();

            SaveToHex(fs, 0, 0x20);
        }


        /// <summary>
        ///     Process color images
        /// </summary>
        public void Color_task(Endian iEnd, FileType iForm, ImageFormat iClrForm, bool isChain)
        {
            //Определение родительской директории + "INPUT"
            //Поиск всех файлов картинок там
            //Определение их количества
            var directory = AppDomain.CurrentDomain.BaseDirectory + "INPUT";
            var bmpFiles = GetAllBmpFiles(directory);
            var bmpCount = bmpFiles.Count;

            var chainManager = new ChainManager();

            for (var i = 0; i < bmpCount; i++)
                try
                {
                    var image = new Bitmap(bmpFiles[i], true);
                    Console.WriteLine("Image open! " + bmpFiles[i]);

                    var factory = new ImageFactory(iClrForm);
                    var converter = factory.GetConverter();
                    var rawImage = converter.Convert(image);

                    var saverFactory = new ImageSaverFactory(iForm, iEnd, iClrForm);
                    var saver = saverFactory.Select();
                    var info = saver.Save(image, rawImage, bmpFiles[i]);
                    if (info != null)
                        chainManager.Add(info);
                }
                catch (Exception ex)
                {
                    var prevColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Can`t open image! " + bmpFiles[i]);
                    Console.WriteLine("Can`t open image! " + ex.Message);
                    Console.ForegroundColor = prevColor;
                    Console.ReadKey();
                    Environment.Exit(0);
                }

            if (isChain)
                chainManager.CreateChain();
        }
    }
}