using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSImage.Classes
{
    public static class ImageHelper
    {
        public static List<byte> RleEncode(this List<byte> input)
        {
            var rleMass = new List<byte>(); //Результат кодирования
            var tempResult = new byte[256]; //Промежуточный результат
            var cnt = 0;
            byte bOld = 0;

            tempResult[0] = 0x00;

            var bNew = input[cnt++];

            while (cnt < input.Count)
            {
                bOld = bNew;
                bNew = input[cnt++];

                //Совпало новое значение со старым
                if (bOld == bNew)
                {
                    // если были разные символы 
                    if (tempResult[0] < 128)
                    {
                        if (tempResult[0] != 0)
                            for (var p = 0; p < tempResult[0] + 1; p++)
                                rleMass.Add(tempResult[p]);
                        tempResult[0] = 128;
                    }

                    tempResult[0]++;
                    if (tempResult[0] == 128 + 127)
                    {
                        rleMass.Add(tempResult[0]);
                        rleMass.Add(bOld);
                        tempResult[0] = 0;
                    }
                }
                //Значения не совпали
                else
                {
                    tempResult[0]++;
                    // если были одинаковые символы 
                    if (tempResult[0] > 128)
                    {
                        rleMass.Add(tempResult[0]);
                        rleMass.Add(bOld);
                        tempResult[0] = 0;
                    }
                    else
                    {
                        tempResult[tempResult[0]] = bOld;
                        if (tempResult[0] > 127)
                        {
                            for (var p = 0; p < tempResult[0] + 1; p++)
                                rleMass.Add(tempResult[p]);
                            tempResult[0] = 0;
                        }
                    }
                }
            }

            if (tempResult[0] > 128)
            {
                if (bNew == bOld) tempResult[0]++;

                rleMass.Add(tempResult[0]);
                rleMass.Add(bOld);

                if (bNew != bOld)
                {
                    tempResult[0] = 1;
                    rleMass.Add(tempResult[0]);
                    rleMass.Add(bNew);
                }
            }
            else
            {
                tempResult[0]++;
                tempResult[tempResult[0]] = bNew;
                for (var p = 0; p < tempResult[0] + 1; p++)
                    rleMass.Add(tempResult[p]);
            }

            return rleMass;
        }
    }
}
