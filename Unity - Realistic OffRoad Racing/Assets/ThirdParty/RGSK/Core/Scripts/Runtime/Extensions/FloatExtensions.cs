namespace RGSK.Extensions
{
    public static class FloatExtensions
    {
        public static bool IsBetween(this float f, float lower, float upper)
        {
            return lower < f && f < upper;
        }
    } 
}
