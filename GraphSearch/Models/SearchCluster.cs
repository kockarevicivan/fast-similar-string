using GraphSearch.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            List<SearchResult> results = new List<SearchResult>();

            char[] inputCharacters = input.ToCharArray();

            foreach (char[] book in _books) {
                double matchCoefficient = GetMatchCoefficient(inputCharacters, book, lengthTollerancePercentage);

                if (matchCoefficient != 0)
                    results.Add(new SearchResult { Word = new string(book), MatchCoefficient = matchCoefficient });
            }

            return results
                .OrderBy(e => GetLevenshteinDistance(e.Word, input))
                .Take(takeAmount)
                .ToList();
        }


        private double GetMatchCoefficient(char[] input, char[] reference, double lengthTollerancePercentage)
        {
            /// TODO: This method only reduces the set by checking for same letters and length tollerance. This should have a better implementation.
            int lengthDifference = Math.Abs(input.Length - reference.Length);
            double lengthDifferencePercentage = NumberHelper.ToPercents(lengthDifference, reference.Length);

            return lengthDifferencePercentage <= lengthTollerancePercentage ? input.Intersect(reference).Count() : 0;
        }

        private int GetLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
                return string.IsNullOrEmpty(t) ? 0 : t.Length;

            if (string.IsNullOrEmpty(t))
                return s.Length;

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
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
