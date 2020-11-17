namespace GraphSearch.Helpers
{
    internal static class NumberHelper
    {
        internal static double ToPercents(double value, double maxValue)
        {
            return (value * 100) / maxValue;
        }
    }
}
