using System;
using System.Collections.Generic;
using System.Linq;

namespace Shouldly
{
    public static class EnumerableAssertions
    {
        public static void EachShould<T>(this IEnumerable<T> items, Action<T> assertion)
            => items.ShouldSatisfyAllConditions(
                items.Select(item => new Action(() => assertion.Invoke(item)))
                    .ToArray());

        public static void ShouldBeSet<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
            => actual.ShouldBe(expected, ignoreOrder: true);

        public static void ShouldBeSequence<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
            => actual.ShouldBe(expected, ignoreOrder: false);
    }
}
