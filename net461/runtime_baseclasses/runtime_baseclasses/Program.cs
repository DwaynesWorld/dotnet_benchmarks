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
            Console.WriteLine(".Net Core/.Net Framework Performance Test");
            Console.WriteLine("");
            RunTest();
        }

        static void RunTest()
        {
            Lists();
            IEnumerables();
            Queues();
            SortedSets();
            SortedSetsMinMax();
            Linq_OrderBySkipFirst();
            Linq_SelectToList();
            DateTime_ToString();
            Deserialization();
        }


        static void Lists()
        {
            Console.WriteLine("Collections - List");
            Console.WriteLine("Performance: 1.4x, Both highly optimized");

            var times = new List<Double>();

            for (int it = 0; it < COUNT; it++)
            {
                var l = new List<int>();
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 100_000_000; i++)
                {
                    l.Add(i);
                    l.RemoveAt(0);
                }

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Average: {times.Average()}");
            Console.WriteLine("");
        }

        static void IEnumerables()
        {
            Console.WriteLine("Collections - IEnumerable");
            Console.WriteLine("Performance: 29.4x");

            var times = new List<Double>();

            IEnumerable<int> zeroToTen = Enumerable.Range(0, 10);
            IEnumerable<int> result = zeroToTen;

            for (int i = 0; i < 10_000; i++)
            {
                result = result.Concat(zeroToTen);
            }

            for (int it = 0; it < COUNT; it++)
            {
                var sw = Stopwatch.StartNew();

                foreach (int i in result) { }

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Average: {times.Average()}");
            Console.WriteLine("");
        }

        static void Queues()
        {
            Console.WriteLine("Collections - Queue");
            Console.WriteLine("Performance: 1.7x");

            var times = new List<Double>();

            for (int it = 0; it < COUNT; it++)
            {
                var q = new Queue<int>();
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 100_000_000; i++)
                {
                    q.Enqueue(i);
                    q.Dequeue();
                }

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Average: {times.Average()}");
            Console.WriteLine("");
        }

        static void SortedSets()
        {
            Console.WriteLine("Collections - SortedSet");
            Console.WriteLine("Performance: 592x, Bad original algorithm for handling duplicates ( O(N^2) )");

            var sw = Stopwatch.StartNew();

            var ss = new SortedSet<int>(Enumerable.Repeat(42, 1_000_000));

            sw.Stop();

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            Console.WriteLine("");
        }

        static void SortedSetsMinMax()
        {
            Console.WriteLine("Collections - SortedSets Min & Max");
            Console.WriteLine("Performance: 13.2x");

            var times = new List<Double>();

            var s = new SortedSet<int>();
            for (int n = 0; n < 100_000; n++)
            {
                s.Add(n);
            }

            for (int it = 0; it < COUNT; it++)
            {

                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 10_000_000; i++)
                {
                    var result = s.Min;
                }

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Average: {times.Average()}");
            Console.WriteLine("");
        }

        static void Linq_OrderBySkipFirst()
        {
            Console.WriteLine("Linq - OrderBy, Skip, First");
            Console.WriteLine("Performance: 7.8x");

            var times = new List<Double>();
            IEnumerable<int> tenMillionToZero = Enumerable.Range(0, 10_000_000).Reverse();

            for (int it = 0; it < COUNT; it++)
            {
                var sw = Stopwatch.StartNew();

                int fifth = tenMillionToZero.OrderBy(i => i).Skip(4).First();

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Average: {times.Average()}");
            Console.WriteLine("");
        }

        static void Linq_SelectToList()
        {
            Console.WriteLine("Linq - Select, ToList");
            Console.WriteLine("Performance: 4x");

            var times = new List<Double>();
            IEnumerable<int> zeroToTenMillion = Enumerable.Range(0, 10_000_000).ToArray();

            for (int it = 0; it < COUNT; it++)
            {
                var sw = Stopwatch.StartNew();

                zeroToTenMillion.Select(i => i).ToList();

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Average: {times.Average()}");
            Console.WriteLine("");
        }

        static void DateTime_ToString()
        {
            Console.WriteLine("String Manipulation - DateTime ToString");
            Console.WriteLine("Performance: 3.2x, Memory: 10x less memory garbage collections");

            var times = new List<Double>();
            var gcs = new List<int>();
            var dt = DateTime.Now;

            for (int it = 0; it < COUNT; it++)
            {

                int gen0 = GC.CollectionCount(0);
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < 2_000_000; i++)
                {
                    dt.ToString("o");
                    dt.ToString("r");
                }

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
                gcs.Add(GC.CollectionCount(0) - gen0);
                Console.WriteLine($"Elapsed: {sw.Elapsed.TotalMilliseconds} GC Count: {GC.CollectionCount(0) - gen0}");
            }

            Console.WriteLine($"Average Elapsed: {times.Average()}, Average GC Count: {gcs.Average()}");
            Console.WriteLine("");
        }

        static void Deserialization()
        {
            Console.WriteLine("Serialization - Binary Deserialization");
            Console.WriteLine("Performance: 12x");

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
            Console.WriteLine("");
        }

        [Serializable]
        private class Book
        {
            public string Name;
            public string Id;
        }
    }
}
