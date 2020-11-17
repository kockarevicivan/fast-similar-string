namespace GraphSearch.Models
{
    public class SearchResult
    {
        public string Word { get; set; }
        public double LevenshteinDistance { get; set; }
    }
}
