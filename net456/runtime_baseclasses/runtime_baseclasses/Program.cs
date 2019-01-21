using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace runtime_baseclasses
{
    class Program
    {
        public static readonly int COUNT = 5;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Choose a performance Test:");
                Console.WriteLine("(1) Collections - List");
                Console.WriteLine("(2) Collections - IEnumerable");
                Console.WriteLine("(3) Collections - Queue");
                Console.WriteLine("(4) Collections - SortedSet");
                Console.WriteLine("(5) Collections - SortedSets Min & Max");
                Console.WriteLine("(6) Linq - OrderBy, Skip, First");
                Console.WriteLine("(7) Linq - Select, ToList");
                Console.WriteLine("(8) String Manipulation - DateTime ToString");
                Console.WriteLine("(9) Serialization - Binary Deserialization ");
                Console.WriteLine("(10) Serialization - Binary ");
                Console.WriteLine("");
                Console.WriteLine("");

                string input = Console.ReadLine();
                int selection = -1;
                bool valid = int.TryParse(input, out selection);

                if (!valid)
                {
                    Console.WriteLine("Please enter a valid number.");
                }
                else
                {
                    RunTest(selection);
                }

                Console.WriteLine("");
            }
        }

        static void RunTest(int testNumber)
        {
            switch (testNumber)
            {
                case 1:
                    Lists();
                    break;
                case 2:
                    IEnumerables();
                    break;
                case 3:
                    Queues();
                    break;
                case 4:
                    SortedSets();
                    break;
                case 5:
                    SortedSetsMinMax();
                    break;
                case 6:
                    Linq_OrderBySkipFirst();
                    break;
                case 7:
                    Linq_SelectToList();
                    break;
                case 8:
                    DateTime_ToString();
                    break;
                case 9:
                    Deserialization();
                    break;
                case 10:
                    Deserialization();
                    break;
                default:
                    break;
            }
        }

        // Both highly optimized
        static void Lists()
        {
            for (int it = 0; it < COUNT; it++)
            {
                var l = new List<int>();
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < 100_000_000; i++)
                {
                    l.Add(i);
                    l.RemoveAt(0);
                }
                Console.WriteLine(sw.Elapsed);
            }
        }

        // ~30x improve
        static void IEnumerables()
        {
            for (int it = 0; it < COUNT; it++)
            {
                IEnumerable<int> zeroToTen = Enumerable.Range(0, 10);
                IEnumerable<int> result = zeroToTen;

                for (int i = 0; i < 10_000; i++)
                {
                    result = result.Concat(zeroToTen);
                }

                var sw = Stopwatch.StartNew();
                foreach (int i in result) { }
                Console.WriteLine(sw.Elapsed);
            }
        }

        // ~2x increase in throughput
        static void Queues()
        {
            for (int it = 0; it < COUNT; it++)
            {
                var q = new Queue<int>();
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 100_000_000; i++)
                {
                    q.Enqueue(i);
                    q.Dequeue();
                }

                Console.WriteLine(sw.Elapsed);
            }
        }

        // ~600x when handling dups
        // original O(N^2) algorithm for handling duplicates
        static void SortedSets()
        {
            for (int it = 0; it < COUNT; it++)
            {
                var sw = Stopwatch.StartNew();
                var ss = new SortedSet<int>(Enumerable.Repeat(42, 4_000_000));
                Console.WriteLine(sw.Elapsed);
            }
        }

        // ~15x
        static void SortedSetsMinMax()
        {
            for (int it = 0; it < COUNT; it++)
            {
                var s = new SortedSet<int>();
                for (int n = 0; n < 100_000; n++)
                {
                    s.Add(n);
                }

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < 10_000_000; i++)
                {
                    var result = s.Min;
                }

                Console.WriteLine(sw.Elapsed);
            }
        }


        // ~8x
        static void Linq_OrderBySkipFirst()
        {
            IEnumerable<int> tenMillionToZero = Enumerable.Range(0, 10_000_000).Reverse();

            for (int it = 0; it < COUNT; it++)
            {
                var sw = Stopwatch.StartNew();
                int fifth = tenMillionToZero.OrderBy(i => i).Skip(4).First();
                Console.WriteLine(sw.Elapsed);
            }
        }

        // ~4x
        static void Linq_SelectToList()
        {
            IEnumerable<int> zeroToTenMillion = Enumerable.Range(0, 10_000_000).ToArray();

            for (int it = 0; it < COUNT; it++)
            {
                var sw = Stopwatch.StartNew();
                zeroToTenMillion.Select(i => i).ToList();
                Console.WriteLine(sw.Elapsed);
            }
        }

        // ~3x , ~10x less memory
        static void DateTime_ToString()
        {
            var dt = DateTime.Now;

            for (int it = 0; it < COUNT; it++)
            {
                var sw = new Stopwatch();
                int gen0 = GC.CollectionCount(0);
                sw.Start();

                for (int i = 0; i < 2_000_000; i++)
                {
                    dt.ToString("o");
                    dt.ToString("r");
                }

                Console.WriteLine($"Elapsed={sw.Elapsed} Gen0={GC.CollectionCount(0) - gen0}");
            }
        }

        // ~12x
        static void Deserialization()
        {
            var books = new List<Book>();
            for (int i = 0; i < 1_000_000; i++)
            {
                string id = i.ToString();
                books.Add(new Book { Name = id, Id = id });
            }

            var formatter = new BinaryFormatter();
            var mem = new MemoryStream();
            formatter.Serialize(mem, books);

            mem.Position = 0;

            var sw = Stopwatch.StartNew();
            formatter.Deserialize(mem);
            sw.Stop();

            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }

        [Serializable]
        private class Book
        {
            public string Name;
            public string Id;
        }
    }
}
