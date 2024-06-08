using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Common<T>(
            this IEnumerable<T> source,
            IEnumerable<T> sequence,
            IEqualityComparer<T> comparer = null)
        {
            if (sequence == null)
            {
                return Enumerable.Empty<T>();
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            return source.GroupBy(t => t, comparer)
                .Join(
                    sequence.GroupBy(t => t, comparer),
                    g => g.Key,
                    g => g.Key,
                    (lg, rg) => lg.Zip(rg, (l, r) => l),
                    comparer)
                .SelectMany(g => g);
        }
    }
}