using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value)
            => Enumerable.Empty<T>()
                .Append(value);
    }
}
