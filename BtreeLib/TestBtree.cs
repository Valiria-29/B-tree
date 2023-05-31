using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BtreeLib
{
    public class TestBtree
    {
        public TestBtree() { }

        public void DataGeneration()//создание и заполнение 100 файлов с именами пользователей
        {
            Random rd = new Random();
            for (int i = 0; i < 100; i++)
            {
                using (StreamWriter sw = new StreamWriter($"TestData\\file{i}.txt"))
                {
                    char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
                    var stringArray = new string[100_000];
                    for (int j = 0; j < 100_000; j++)
                    {
                        var sb = new StringBuilder();
                        for (int k = 0; k < rd.Next(2, 200); k++)
                        {
                            sb.Append(letters[rd.Next(0, letters.Length - 1)]);
                        }
                        stringArray[j] = sb.ToString();
                    }
                    sw.WriteLine(string.Join("\n", stringArray));
                }
            }
        }

        public long FillingAndTimeTestForDifferentBtree(Btree<string> btree)
        {
            //создаем таймер и запускаем
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            sw.Start();

            //массив для имен, которые будем искать и удалять(каждый 100_000)
            var containcAndDeleteNameArray = new string[100];
            //читаем все файлы по очереди
            for (int i = 0; i < 100; i++)
            {
                using (StreamReader sr = new StreamReader($"TestData\\file{i}.txt"))
                {
                    string currentName = null;
                    //читаем очередную строку
                    for (int j = 0; j < 100_000; j++)
                    {
                        currentName = sr.ReadLine().Split('\n').ToString();
                        //добавляем ее в дерево
                        btree.Add(currentName);
                    }
                    //последнее имя добавим в containcAndDeleteNameArray
                    containcAndDeleteNameArray[i] = currentName;
                }
            }
            //для каждого имени из containcAndDeleteNameArray проверим, содержится ли это имя в дереве и удалим его
            for (int i = 0; i < containcAndDeleteNameArray.Length; i++)
            {
                if (btree.Contains(containcAndDeleteNameArray[i]))
                {
                    btree.Delete(containcAndDeleteNameArray[i]);
                }
            }
            sw.Stop();
            var resultTime = sw.ElapsedMilliseconds;
            return resultTime;
        }
    }
}
