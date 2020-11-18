namespace GraphSearch.Models
{
    public class SearchResult
    {
        public char[] Word { get; set; }
        public double LevenshteinDistance { get; set; }
    }
}
