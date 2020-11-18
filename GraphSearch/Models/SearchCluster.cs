using GraphSearch.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GraphSearch.Models
{
    public class SearchCluster
    {
        private string _filePath;
        private ClusterInsert[] _books;
        private const string _alphabet = "abcdefghijklmnopqrstuvwxyz1234567890";

        public SearchCluster(string filePath)
        {
            _filePath = filePath;
            _books = File.ReadAllLines(filePath).Select(e => {
                char[] bookAsCharArray = e.ToCharArray();

                return new ClusterInsert
                {
                    Word = bookAsCharArray,
                    Index = GetVector(bookAsCharArray)
                };
            }).ToArray();
        }


        public List<SearchResult> Find(
            string input,
            int takeAmount = 50,
            double lengthTollerancePercentage = 100)
        {
            char[] inputCharacters = input.ToCharArray();
            int[] inputVector = GetVector(inputCharacters);

            ConcurrentBag<SearchResult> results = new ConcurrentBag<SearchResult>();

            int processorChunkSize = _books.Length / Environment.ProcessorCount;

            /// TODO: Last elements are sometimes skipped.
            //Parallel.For(0, Environment.ProcessorCount, i => {
                for(int i = 0; i < Environment.ProcessorCount; i++)
                for (int j = i * processorChunkSize; j < (i + 1) * processorChunkSize; j++)
                {
                    char[] book = _books[j].Word;

                    if (PassesComparisonCriteria(inputCharacters, book, lengthTollerancePercentage, inputVector))
                    {
                        results.Add(new SearchResult
                        {
                            Word = book,
                            LevenshteinDistance = GetLevenshteinDistance(book, inputCharacters)
                        });
                    }
                }
            //});

            return results
                .OrderBy(e => e.LevenshteinDistance)
                .Take(takeAmount)
                .ToList();
        }

        private int GetVectorDistance(int[] vector1, int[] vector2)
        {
            int result = 0;

            for (int i = 0; i < _alphabet.Length; i++)
                result += Math.Abs(vector1[i] - vector2[i]);

            return result;
        }

        private int[] GetVector(char[] input)
        {
            int[] result = new int[_alphabet.Length];

            for (int i = 0; i < _alphabet.Length; i++)
                result[i] = input.Count(c => c == _alphabet[i]);

            return result;
        }

        private bool PassesComparisonCriteria(char[] input, char[] reference, double toleratedLengthDifferencePercentage, int[] inputVector)
        {
            // Calculate difference in lengths of input and compared string and check if under allowed limit percentage.
            double lengthDifferencePercentage = NumberHelper.ToPercents(
                Math.Abs(input.Length - reference.Length),
                reference.Length
            );

            if (lengthDifferencePercentage > toleratedLengthDifferencePercentage)
                return false;

            int[] referenceVector = GetVector(reference);

            if (GetVectorDistance(inputVector, referenceVector) > reference.Length)
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
