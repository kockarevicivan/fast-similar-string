using GraphSearch.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GraphSearch.Models
{
    public class SearchCluster
    {
        private string _filePath;
        private char[][] _books;


        public SearchCluster(string filePath)
        {
            _filePath = filePath;
            _books = File.ReadAllLines(filePath).Select(e => e.ToCharArray()).ToArray();
        }


        public List<SearchResult> Find(
            string input,
            int takeAmount = 50,
            double lengthTollerancePercentage = 100)
        {
            char[] inputCharacters = input.ToCharArray();
            ConcurrentBag<SearchResult> results = new ConcurrentBag<SearchResult>();

            Parallel.ForEach(_books, (book) => {
                if (PassesComparisonCriteria(inputCharacters, book, lengthTollerancePercentage))
                {
                    results.Add(new SearchResult
                    {
                        Word = new string(book),
                        LevenshteinDistance = GetLevenshteinDistance(book, inputCharacters)
                    });
                }
            });

            return results
                .OrderBy(e => e.LevenshteinDistance)
                .Take(takeAmount)
                .ToList();
        }


        private bool PassesComparisonCriteria(char[] input, char[] reference, double toleratedLengthDifferencePercentage)
        {
            // Calculate difference in lengths of input and compared string and check if under allowed limit percentage.
            double lengthDifferencePercentage = NumberHelper.ToPercents(
                Math.Abs(input.Length - reference.Length),
                reference.Length
            );

            if (lengthDifferencePercentage > toleratedLengthDifferencePercentage)
                return false;

            return true;
        }

        private int GetLevenshteinDistance(char[] input, char[] comparedTo)
        {
            if (input == null || input.Length == 0)
                return (comparedTo == null || comparedTo.Length == 0) ? 0 : comparedTo.Length;

            if (comparedTo == null || comparedTo.Length == 0)
                return input.Length;

            int n = input.Length;
            int m = comparedTo.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (comparedTo[j - 1] == input[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[n, m];
        }
    }
}
