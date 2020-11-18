using GraphSearch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GraphSearch
{
    class Program
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            //string[] lines = File.ReadAllLines("words.txt");

            //StringBuilder sb = new StringBuilder();

            //for (int number = 1; number < 4; number++)
            //    for (int i = 0; i < lines.Length - 3; i++)
            //        sb.AppendLine(lines[i] + " " + lines[i + 1] + " " + lines[i + 2] + " " + number);


            //File.AppendAllText("books.txt", sb.ToString());

            // Loading
            _stopwatch.Start();

            SearchCluster cluster = new SearchCluster("books.txt");

            _stopwatch.Stop();

            Console.WriteLine("Insertion time in seconds: " + ((double)_stopwatch.ElapsedTicks / 10000000));



            // Searching            
            string input = "Zwingle";
            List<SearchResult> results = new List<SearchResult>();

            _stopwatch.Restart();

            for (int i = 0; i < 1; i++)
                results = cluster.Find(input, 10, 100);

            _stopwatch.Stop();

            Console.WriteLine("Search time in seconds: " + ((double)_stopwatch.ElapsedTicks / 10000000));
        }
    }
}
