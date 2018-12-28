using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Immutable
{
    public class ImmutableHashDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        public static readonly ImmutableHashDictionary<TKey, TValue> Empty
            = new ImmutableHashDictionary<TKey, TValue>();

        public static ImmutableHashDictionary<TKey, TValue> Create(IReadOnlyCollection<KeyValuePair<TKey, TValue>> keyValuePairs)
            => Create(keyValuePairs, keyValuePairs.Count);

        public static ImmutableHashDictionary<TKey, TValue> Create(IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs, int capacity = 0)
        {
            var dictionary = new ImmutableHashDictionary<TKey, TValue>(capacity);

            foreach (var keyValuePair in keyValuePairs)
                dictionary._dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            return dictionary;
        }

        public static ImmutableHashDictionary<TKey, TValue> Create(IReadOnlyCollection<TValue> values, Func<TValue, TKey> keySelector)
            => Create(values, keySelector, values.Count);

        public static ImmutableHashDictionary<TKey, TValue> Create(IEnumerable<TValue> values, Func<TValue, TKey> keySelector, int capacity = 0)
        {
            var dictionary = new ImmutableHashDictionary<TKey, TValue>(capacity);

            foreach (var value in values)
                dictionary._dictionary.Add(keySelector.Invoke(value), value);

            return dictionary;
        }

        private ImmutableHashDictionary(int capacity = 0)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public TValue this[TKey key]
            => _dictionary[key];

        public int Count
            => _dictionary.Count;

        public IEnumerable<TKey> Keys
            => _dictionary.Keys;

        public IEnumerable<TValue> Values
            => _dictionary.Values;

        public bool ContainsKey(TKey key)
            => _dictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => (_dictionary as IEnumerable).GetEnumerator();

        public bool TryGetValue(TKey key, out TValue value)
            => _dictionary.TryGetValue(key, out value);

        public ImmutableHashDictionary<TKey, TValue> Add(TKey key, TValue value)
            => _dictionary.TryGetValue(key, out var existingValue)
                ? EqualityComparer<TValue>.Default.Equals(value, existingValue)
                    ? this
                    : throw new ArgumentException(nameof(key), $"Key {key} is already in the dictionary, with value {existingValue} instead of {value}.")
                : _dictionary
                    .Append(new KeyValuePair<TKey, TValue>(key, value))
                    .ToImmutableHashDictionary(_dictionary.Count + 1);

        public ImmutableHashDictionary<TKey, TValue> AddRange(IReadOnlyCollection<KeyValuePair<TKey, TValue>> keyValuePairs)
            => AddRange(keyValuePairs, keyValuePairs.Count);

        public ImmutableHashDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs, int capacity = 0)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Cannot be negative");

            var dictionary = new ImmutableHashDictionary<TKey, TValue>(_dictionary.Count + capacity);

            foreach (var keyValuePair in keyValuePairs)
            {
                if (_dictionary.TryGetValue(keyValuePair.Key, out var value))
                {
                    if (!EqualityComparer<TValue>.Default.Equals(keyValuePair.Value, value))
                        throw new ArgumentException(nameof(keyValuePairs), $"Key {keyValuePair.Key} is already in the dictionary, with value {value} instead of {keyValuePair.Value}");
                }
                else
                {
                    dictionary._dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            if (!dictionary._dictionary.Any())
                return this;

            foreach (var keyValuePair in _dictionary)
                dictionary._dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            return dictionary;
        }

        public ImmutableHashDictionary<TKey, TValue> AddRange(IReadOnlyCollection<TValue> values, Func<TValue, TKey> keySelector)
            => AddRange(values, keySelector, values.Count);

        public ImmutableHashDictionary<TKey, TValue> AddRange(IEnumerable<TValue> values, Func<TValue, TKey> keySelector, int capacity = 0)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Cannot be negative");

            var dictionary = new ImmutableHashDictionary<TKey, TValue>(_dictionary.Count + capacity);

            foreach (var value in values)
            {
                var key = keySelector.Invoke(value);

                if (_dictionary.TryGetValue(key, out var existingValue))
                {
                    if (!EqualityComparer<TValue>.Default.Equals(value, existingValue))
                        throw new ArgumentException(nameof(keySelector), $"Key {key} is already in the dictionary, with value {existingValue} instead of {value}");
                }
                else
                {
                    dictionary._dictionary.Add(key, value);
                }
            }

            if (!dictionary._dictionary.Any())
                return this;

            foreach (var keyValuePair in _dictionary)
                dictionary._dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            return dictionary;
        }

        public ImmutableHashDictionary<TKey, TValue> Remove(TKey key)
            => _dictionary.ContainsKey(key)
                ? _dictionary
                    .Where(x => !EqualityComparer<TKey>.Default.Equals(x.Key, key))
                    .ToImmutableHashDictionary(_dictionary.Count - 1)
                : this;

        public ImmutableHashDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
        {
            var keysToRemove = new HashSet<TKey>(keys);

            keysToRemove.IntersectWith(_dictionary.Keys);

            return keysToRemove.Any()
                ? _dictionary
                    .Where(x => !keysToRemove.Contains(x.Key))
                    .ToImmutableHashDictionary(_dictionary.Count - keysToRemove.Count)
                : this;
        }

        private readonly Dictionary<TKey, TValue> _dictionary;
    }
}
