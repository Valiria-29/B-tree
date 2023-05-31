using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BtreeLib;

namespace Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //создадим деревья разной степени
            var comparer = StringComparer.Ordinal;
            var BtreeList = new List<Btree<string>>
            {
                new Btree<string>(5,comparer),
                new Btree<string>(10,comparer),
                new Btree<string>(15,comparer),
                new Btree<string>(20,comparer),
                new Btree<string>(25,comparer),
                new Btree<string>(30,comparer),
            };

            //заполним 100 текстовых файлов с 100_000 именами пользователей в каждом
            TestBtree testBtree = new TestBtree();
            //testBtree.DataGeneration();

            //для каждого дерева из BtreeList проведем тестирование 5 раз и поcчитаем среднее время
            var AllResultsTime = new List<long>();
            var resultMiddleTime = new List<double>();
            long middleTimeResult = 0;
            long OneBtreeTimeResult = 0;
            for (int i = 0; i < BtreeList.Count; i++)
            {
                middleTimeResult = testBtree.FillingAndTimeTestForDifferentBtree(BtreeList[i]);
                AllResultsTime.Add(middleTimeResult);
                OneBtreeTimeResult += middleTimeResult;
                OneBtreeTimeResult /= 5;
                resultMiddleTime.Add(OneBtreeTimeResult);
            }
            using (var sw = new StreamWriter("results.txt"))
            {
                for (int i = 0; i < AllResultsTime.Count; i++)
                {
                    sw.WriteLine(AllResultsTime[i]);
                }
                sw.WriteLine("------------------------------------");
                for (int i = 0; i < resultMiddleTime.Count; i++)
                {
                    sw.WriteLine(resultMiddleTime[i]);
                }
            }
        }
    }
}
