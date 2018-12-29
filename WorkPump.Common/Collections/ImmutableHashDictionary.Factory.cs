using System.Collections.Generic;

namespace System.Collections.Immutable
{
    public static class ImmutableHashDictionary
    {
        public static ImmutableHashDictionary<TKey, TValue> Create<TKey, TValue>(IReadOnlyCollection<KeyValuePair<TKey, TValue>> keyValuePairs)
            => ImmutableHashDictionary<TKey, TValue>.Create(keyValuePairs);

        public static ImmutableHashDictionary<TKey, TValue> Create<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs, int capacity = 0)
            => ImmutableHashDictionary<TKey, TValue>.Create(keyValuePairs, capacity);

        public static ImmutableHashDictionary<TKey, TValue> Create<TKey, TValue>(IReadOnlyCollection<TValue> values, Func<TValue, TKey> keySelector)
            => ImmutableHashDictionary<TKey, TValue>.Create(values, keySelector);

        public static ImmutableHashDictionary<TKey, TValue> Create<TKey, TValue>(IEnumerable<TValue> values, Func<TValue, TKey> keySelector, int capacity = 0)
            => ImmutableHashDictionary<TKey, TValue>.Create(values, keySelector, capacity);

        public static ImmutableHashDictionary<TKey, TValue> ToImmutableHashDictionary<TKey, TValue>(this IReadOnlyCollection<KeyValuePair<TKey, TValue>> keyValuePairs)
            => Create(keyValuePairs);

        public static ImmutableHashDictionary<TKey, TValue> ToImmutableHashDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs, int capacity = 0)
            => Create(keyValuePairs, capacity);

        public static ImmutableHashDictionary<TKey, TValue> ToImmutableHashDictionary<TKey, TValue>(this IReadOnlyCollection<TValue> values, Func<TValue, TKey> keySelector)
            => Create(values, keySelector);

        public static ImmutableHashDictionary<TKey, TValue> ToImmutableHashDictionary<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector, int capacity = 0)
            => Create(values, keySelector, capacity);
    }
}
