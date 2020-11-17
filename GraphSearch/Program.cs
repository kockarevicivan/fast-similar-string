using GraphSearch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GraphSearch
{
    class Program
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            // Loading
            _stopwatch.Start();

            SearchCluster cluster = new SearchCluster("words.txt");

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
