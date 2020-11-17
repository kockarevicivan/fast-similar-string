using GraphSearch.Models;
using System;
using System.Diagnostics;

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
            _stopwatch.Restart();
            
            string input = "Zwingle";

            var results = cluster.Find(input, 10, 20);
            
            _stopwatch.Stop();

            Console.WriteLine("Search time in seconds: " + ((double)_stopwatch.ElapsedTicks / 10000000));
        }
    }
}
