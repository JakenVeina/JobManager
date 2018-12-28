using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test.Collections
{
    [TestFixture]
    public class ImmutableHashDictionaryTests
    {
        #region Test Context

        public static ImmutableHashDictionary<int, int> BuildUUT(string uutString)
            => ImmutableHashDictionary<int, int>.Create(ParseKeyValuePairsString(uutString));

        public static IReadOnlyCollection<KeyValuePair<int, int>> ParseKeyValuePairsString(string keyValuePairsString)
            => keyValuePairsString
                .Replace(" ", "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { "=>" }, StringSplitOptions.None))
                .Select(x => new KeyValuePair<int, int>(int.Parse(x[0]), int.Parse(x[1])))
                .ToArray();

        #endregion Test Context

        #region Test Data

        public static readonly string[] KeyValuePairsTestCases
            = new[]
            {
                "",
                "1=>2",
                "1=>2, 2=>3",
                "1=>2, 2=>3, 3=>4"
            };

        public static readonly object[][] KeyValuePairsWithInvalidCapacityTestCases
            = new[]
            {
                new object[] { "",                 -1 },
                new object[] { "1=>2",             -2 },
                new object[] { "1=>2, 2=>3",       -3 },
                new object[] { "1=>2, 2=>3, 3=>4", -4 }
            };

        public static readonly object[][] KeyValuePairsWithValidCapacityTestCases
            = new[]
            {
                new object[] { "",                 0 },
                new object[] { "1=>2",             1 },
                new object[] { "1=>2, 2=>3",       2 },
                new object[] { "1=>2, 2=>3, 3=>4", 3 },
                new object[] { "1=>2, 2=>3, 3=>4", 2 },
                new object[] { "1=>2, 2=>3, 3=>4", 1 },
                new object[] { "1=>2, 2=>3, 3=>4", 0 }
            };

        public static readonly object[][] ValuesWithKeySelectorTestCases
            = new[]
            {
                new object[] { new int[] { },         (Expression<Func<int, int>>)(value => value + 1) },
                new object[] { new int[] { 1 },       (Expression<Func<int, int>>)(value => value + 2) },
                new object[] { new int[] { 1, 2 },    (Expression<Func<int, int>>)(value => value + 3) },
                new object[] { new int[] { 1, 2, 3 }, (Expression<Func<int, int>>)(value => value + 4) }
            };

        public static readonly object[][] ValuesWithKeySelectorAndInvalidCapacityTestCases
            = new[]
            {
                new object[] { new int[] { },         (Expression<Func<int, int>>)(value => value + 1), -1 },
                new object[] { new int[] { 1 },       (Expression<Func<int, int>>)(value => value + 2), -2 },
                new object[] { new int[] { 1, 2 },    (Expression<Func<int, int>>)(value => value + 3), -3 },
                new object[] { new int[] { 1, 2, 3 }, (Expression<Func<int, int>>)(value => value + 4), -4 }
            };

        public static readonly object[][] ValuesWithKeySelectorAndValidCapacityTestCases
            = new[]
            {
                new object[] { new int[] { },         (Expression<Func<int, int>>)(value => value + 1), 0 },
                new object[] { new int[] { 1 },       (Expression<Func<int, int>>)(value => value + 2), 1 },
                new object[] { new int[] { 1, 2 },    (Expression<Func<int, int>>)(value => value + 3), 2 },
                new object[] { new int[] { 1, 2, 3 }, (Expression<Func<int, int>>)(value => value + 4), 3 },
                new object[] { new int[] { 1, 2, 3 }, (Expression<Func<int, int>>)(value => value + 5), 2 },
                new object[] { new int[] { 1, 2, 3 }, (Expression<Func<int, int>>)(value => value + 6), 1 },
                new object[] { new int[] { 1, 2, 3 }, (Expression<Func<int, int>>)(value => value + 7), 0 }
            };

        public static readonly object[][] ValidKeyWithValueTestCases
            = new[]
            {
                new object[] { "1=>2",             1, 2 },
                new object[] { "1=>2, 2=>3",       1, 2 },
                new object[] { "1=>2, 2=>3",       2, 3 },
                new object[] { "1=>2, 2=>3, 3=>4", 1, 2 },
                new object[] { "1=>2, 2=>3, 3=>4", 2, 3 },
                new object[] { "1=>2, 2=>3, 3=>4", 3, 4 }
            };

        public static readonly object[][] ValidKeyTestCases
            = new[]
            {
                new object[] { "1=>2",             1, },
                new object[] { "1=>2, 2=>3",       1, },
                new object[] { "1=>2, 2=>3",       2, },
                new object[] { "1=>2, 2=>3, 3=>4", 1, },
                new object[] { "1=>2, 2=>3, 3=>4", 2, },
                new object[] { "1=>2, 2=>3, 3=>4", 3, }
            };

        public static readonly object[][] InvalidKeyTestCases
            = new[]
            {
                new object[] { "",                 1 },
                new object[] { "1=>2",             2 },
                new object[] { "1=>2, 2=>3",       3 },
                new object[] { "1=>2, 2=>3, 3=>4", 4 }
            };

        public static object[][] ExistingKeyWithValueTestCases
            => ValidKeyWithValueTestCases;

        public static readonly object[][] ExistingKeyWithDifferentValueTestCases
            = new[]
            {
                new object[] { "1=>2",             1, 3 },
                new object[] { "1=>2, 2=>3",       1, 3 },
                new object[] { "1=>2, 2=>3",       2, 4 },
                new object[] { "1=>2, 2=>3, 3=>4", 1, 3 },
                new object[] { "1=>2, 2=>3, 3=>4", 2, 4 },
                new object[] { "1=>2, 2=>3, 3=>4", 3, 5 }
            };

        public static readonly object[][] NewKeyWithValueTestCases
            = new[]
            {
                new object[] { "",           1, 2 },
                new object[] { "",           2, 3 },
                new object[] { "",           3, 4 },
                new object[] { "1=>2",       2, 3 },
                new object[] { "1=>2",       3, 4 },
                new object[] { "1=>2, 2=>3", 3, 4 }
            };

        public static readonly object[][] ExistingKeyValuePairsTestCases
            = new[]
            {
                new object[] { "1=>2",             "1=>2" },
                new object[] { "1=>2, 2=>3",       "1=>2" },
                new object[] { "1=>2, 2=>3",       "2=>3" },
                new object[] { "1=>2, 2=>3",       "1=>2, 2=>3" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>2" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>2, 2=>3" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>2, 2=>3, 3=>4" }
            };

        public static readonly object[][] InvalidKeyValuePairsTestCases
            = new[]
            {
                new object[] { "1=>2",             "1=>3" },
                new object[] { "1=>2, 2=>3",       "1=>3" },
                new object[] { "1=>2, 2=>3",       "2=>4" },
                new object[] { "1=>2, 2=>3",       "1=>3, 2=>3" },
                new object[] { "1=>2, 2=>3",       "1=>2, 2=>4" },
                new object[] { "1=>2, 2=>3",       "1=>3, 2=>4" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>3" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>3, 2=>3" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>2, 2=>4" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>3, 2=>4" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>3, 2=>3, 3=>4" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>2, 2=>4, 3=>4" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>2, 2=>3, 3=>5" },
                new object[] { "1=>2, 2=>3, 3=>4", "1=>3, 2=>4, 3=>5" }
            };

        public static readonly object[][] NewKeyValuePairsTestCases
            = new[]
            {
                new object[] { "",                 "1=>2" },
                new object[] { "",                 "2=>3" },
                new object[] { "",                 "3=>4" },
                new object[] { "",                 "1=>2, 2=>3" },
                new object[] { "",                 "1=>2, 3=>4" },
                new object[] { "",                 "2=>3, 3=>4" },
                new object[] { "",                 "1=>2, 2=>3, 3=>4" },
                new object[] { "1=>2",             "2=>3" },
                new object[] { "1=>2",             "3=>4" },
                new object[] { "1=>2",             "1=>2, 2=>3" },
                new object[] { "1=>2",             "1=>2, 3=>4" },
                new object[] { "1=>2",             "2=>3, 3=>4" },
                new object[] { "1=>2",             "1=>2, 2=>3, 3=>4" },
                new object[] { "1=>2, 2=>3",       "3=>4" },
                new object[] { "1=>2, 2=>3",       "1=>2, 3=>4" },
                new object[] { "1=>2, 2=>3",       "2=>3, 3=>4" },
                new object[] { "1=>2, 2=>3",       "1=>2, 2=>3, 3=>4" }
            };

        public static readonly object[][] ValidKeyValuePairsWithInvalidCapacityTestCases
            = ExistingKeyValuePairsTestCases
                .Select((x, i) => x.Append(-(i+1)).ToArray())
                .ToArray();

        public static readonly object[][] ExistingKeyValuePairsWithValidCapacityTestCases
            = ExistingKeyValuePairsTestCases
                .Select((x, i) => x.Append(i).ToArray())
                .ToArray();

        public static readonly object[][] InvalidKeyValuePairsWithValidCapacityTestCases
            = InvalidKeyValuePairsTestCases
                .Select((x, i) => x.Append(i).ToArray())
                .ToArray();

        public static readonly object[][] NewKeyValuePairsWithValidCapacityTestCases
            = NewKeyValuePairsTestCases
                .Select((x, i) => x.Append(i).ToArray())
                .ToArray();

        public static readonly object[][] ExistingValuesWithKeySelectorTestCases
            = new[]
            {
                new object[] { "1=>2",             new[] { 2 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",       new[] { 2 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",       new[] { 3 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",       new[] { 2, 3 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 3 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 3, 4 }, (Expression<Func<int, int>>)(value => value - 1) }
            };

        public static readonly object[][] InvalidValuesWithKeySelectorTestCases
            = new[]
            {
                new object[] { "1=>2",             new[] { 3 },       (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3",       new[] { 3 },       (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3",       new[] { 4 },       (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3",       new[] { 2, 3 },    (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3",       new[] { 2, 4 },    (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3",       new[] { 3, 4 },    (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 3 },       (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 3 },    (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 4 },    (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 3, 4 },    (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 3, 4 }, (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 3, 5 }, (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 4, 5 }, (Expression<Func<int, int>>)(value => value - 2) },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 3, 4, 5 }, (Expression<Func<int, int>>)(value => value - 2) }
            };

        public static readonly object[][] NewValuesWithKeySelectorTestCases
            = new[]
            {
                new object[] { "",             new[] { 2 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "",             new[] { 3 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "",             new[] { 4 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "",             new[] { 2, 3 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "",             new[] { 2, 4 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "",             new[] { 3, 4 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "",             new[] { 2, 3, 4 }, (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2",         new[] { 3 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2",         new[] { 4 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2",         new[] { 2, 3 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2",         new[] { 2, 4 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2",         new[] { 3, 4 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2",         new[] { 2, 3, 4 }, (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",   new[] { 4 },       (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",   new[] { 2, 4 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",   new[] { 3, 4 },    (Expression<Func<int, int>>)(value => value - 1) },
                new object[] { "1=>2, 2=>3",   new[] { 2, 3, 4 }, (Expression<Func<int, int>>)(value => value - 1) }
            };

        public static readonly object[][] ValidValuesWithKeySelectorAndInvalidCapacityTestCases
            = ExistingValuesWithKeySelectorTestCases
                .Select((x, i) => x.Append(-(i + 1)).ToArray())
                .ToArray();

        public static readonly object[][] ExistingValuesWithKeySelectorAndValidCapacityTestCases
            = ExistingValuesWithKeySelectorTestCases
                .Select((x, i) => x.Append(i).ToArray())
                .ToArray();

        public static readonly object[][] InvalidValuesWithKeySelectorAndValidCapacityTestCases
            = InvalidValuesWithKeySelectorTestCases
                .Select((x, i) => x.Append(i).ToArray())
                .ToArray();

        public static readonly object[][] NewValuesWithKeySelectorAndValidCapacityTestCases
            = NewValuesWithKeySelectorTestCases
                .Select((x, i) => x.Append(i).ToArray())
                .ToArray();

        public static readonly object[][] ValidKeysTestCases
            = new[]
            {
                new object[] { "1=>2",             new[] { 1 }, },
                new object[] { "1=>2, 2=>3",       new[] { 1 }, },
                new object[] { "1=>2, 2=>3",       new[] { 2 }, },
                new object[] { "1=>2, 2=>3",       new[] { 1, 2 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 1 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 3 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 1, 2 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 1, 3 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 2, 3 }, },
                new object[] { "1=>2, 2=>3, 3=>4", new[] { 1, 2, 3 }, }
            };

        public static readonly object[][] InvalidKeysTestCases
            = new[]
            {
                new object[] { "",           new[] { 1 } },
                new object[] { "",           new[] { 2 } },
                new object[] { "",           new[] { 3 } },
                new object[] { "",           new[] { 1, 2 } },
                new object[] { "",           new[] { 1, 3 } },
                new object[] { "",           new[] { 2, 3 } },
                new object[] { "",           new[] { 1, 2, 3 } },
                new object[] { "1=>2",       new[] { 2 } },
                new object[] { "1=>2",       new[] { 3 } },
                new object[] { "1=>2",       new[] { 2, 3 } },
                new object[] { "1=>2, 2=>3", new[] { 3 } },
            };

        public static readonly object[][] SomeValidKeysTestCases
            = new[]
            {
                new object[] { "1=>2",             new[] { 1, 2 } },
                new object[] { "1=>2",             new[] { 1, 3 } },
                new object[] { "1=>2",             new[] { 1, 2, 3 } },
                new object[] { "1=>2, 2=>3",       new[] { 1, 3 } },
                new object[] { "1=>2, 2=>3",       new[] { 2, 3 } },
                new object[] { "1=>2, 2=>3",       new[] { 1, 2, 3 } },
            };

        #endregion Test Data

        #region Empty Tests

        [Test]
        public void Empty_Always_IsEmpty()
        {
            ImmutableHashDictionary<int, int>.Empty.ShouldBeEmpty();
        }

        #endregion Empty Tests

        #region Create() Tests

        [TestCaseSource(nameof(KeyValuePairsTestCases))]
        public void Create_CapacityIsNotGiven_ResultSetEqualsKeyValuePairs(string keyValuePairString)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairString);

            var result = ImmutableHashDictionary<int, int>.Create(keyValuePairs);

            result.ShouldBeSet(keyValuePairs);
        }

        [TestCaseSource(nameof(KeyValuePairsWithInvalidCapacityTestCases))]
        public void Create_CapacityIsInvalid_ThrowsException(string keyValuePairString, int capacity)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairString);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = ImmutableHashDictionary<int, int>.Create(keyValuePairs, capacity);
            });
        }

        [TestCaseSource(nameof(KeyValuePairsWithValidCapacityTestCases))]
        public void Create_CapacityIsValid_ResultSetEqualsKeyValuePairs(string keyValuePairString, int capacity)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairString);

            var result = ImmutableHashDictionary<int, int>.Create(keyValuePairs, capacity);

            result.ShouldBeSet(keyValuePairs);
        }

        [TestCaseSource(nameof(ValuesWithKeySelectorTestCases))]
        public void Create_CapacityIsNotGiven_ResultSetEqualsValuesWithKeySelector(int[] values, Expression<Func<int, int>> keySelectorExpression)
        {
            var keySelector = keySelectorExpression.Compile();

            var result = ImmutableHashDictionary<int, int>.Create(values, keySelector);

            result.ShouldBeSet(values
                .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value)));
        }

        [TestCaseSource(nameof(ValuesWithKeySelectorAndInvalidCapacityTestCases))]
        public void Create_CapacityIsInvalid_ThrowsException(int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = ImmutableHashDictionary<int, int>.Create(values, keySelector, capacity);
            });
        }

        [TestCaseSource(nameof(ValuesWithKeySelectorAndValidCapacityTestCases))]
        public void Create_CapacityIsValid_ResultSetEqualsValuesWithKeySelector(int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();

            var result = ImmutableHashDictionary<int, int>.Create(values, keySelector, capacity);

            result.ShouldBeSet(values
                .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value)));
        }

        #endregion Create() Tests

        #region this[key] Tests

        [TestCaseSource(nameof(ValidKeyWithValueTestCases))]
        public void ThisKey_KeyExists_ReturnsValueForKey(string uutString, int key, int value)
        {
            var uut = BuildUUT(uutString);

            var result = uut[key];

            result.ShouldBe(value);
        }

        [TestCaseSource(nameof(InvalidKeyTestCases))]
        public void ThisKey_KeyDoesNotExist_ThrowsException(string uutString, int key)
        {
            var uut = BuildUUT(uutString);

            Should.Throw<KeyNotFoundException>(() =>
            {
                var result = uut[key];
            });
        }

        #endregion this[key] Tests

        #region Count Tests

        [TestCaseSource(nameof(KeyValuePairsTestCases))]
        public void Count_Always_ReturnsKeyValuePairsCount(string keyValuePairsString)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var uut = ImmutableHashDictionary<int, int>.Create(keyValuePairs);

            var result = uut.Count;

            result.ShouldBe(keyValuePairs.Count());
        }

        #endregion Count Tests

        #region Keys Tests

        [TestCaseSource(nameof(KeyValuePairsTestCases))]
        public void Keys_Always_SetEqualsKeyValuePairsKeys(string keyValuePairsString)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var uut = ImmutableHashDictionary<int, int>.Create(keyValuePairs);

            var result = uut.Keys;

            result.ShouldBeSet(keyValuePairs
                .Select(pair => pair.Key));
        }

        #endregion Keys Tests

        #region Values Tests

        [TestCaseSource(nameof(KeyValuePairsTestCases))]
        public void Values_Always_SetEqualsKeyValuePairsValues(string keyValuePairsString)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var uut = ImmutableHashDictionary<int, int>.Create(keyValuePairs);

            var result = uut.Values;

            result.ShouldBeSet(keyValuePairs
                .Select(pair => pair.Value));
        }

        #endregion Values Tests

        #region ContainsKey()

        [TestCaseSource(nameof(ValidKeyTestCases))]
        public void ContainsKey_KeyExists_ReturnsTrue(string uutString, int key)
        {
            var uut = BuildUUT(uutString);

            var result = uut.ContainsKey(key);

            result.ShouldBeTrue();
        }

        [TestCaseSource(nameof(InvalidKeyTestCases))]
        public void ContainsKey_KeyDoesNotExist_ThrowsException(string uutString, int key)
        {
            var uut = BuildUUT(uutString);

            var result = uut.ContainsKey(key);

            result.ShouldBeFalse();
        }

        #endregion ContainsKey()

        #region GetEnumerator() Tests

        [TestCaseSource(nameof(KeyValuePairsTestCases))]
        public void GetEnumerator_Generic_EnumerationMatchesKeyValuePairs(string keyValuePairsString)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var uut = ImmutableHashDictionary<int, int>.Create(keyValuePairs);

            uut.ShouldBeSet(keyValuePairs);
        }

        #endregion GetEnumerator() Tests

        #region IEnumerable.GetEnumerator() Tests

        [TestCaseSource(nameof(KeyValuePairsTestCases))]
        public void GetEnumerator_NonGeneric_EnumerationMatchesKeyValuePairs(string keyValuePairsString)
        {
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var uut = ImmutableHashDictionary<int, int>.Create(keyValuePairs);

            (uut as IEnumerable).Cast<object>().ShouldBeSet(keyValuePairs.Cast<object>());
        }

        #endregion IEnumerable.GetEnumerator() Tests

        #region TryGetValue() Tests

        [TestCaseSource(nameof(ValidKeyWithValueTestCases))]
        public void TryGetValue_KeyExists_ReturnsTrueAndValue(string uutString, int key, int expectedValue)
        {
            var uut = BuildUUT(uutString);

            var result = uut.TryGetValue(key, out var value);

            result.ShouldBeTrue();
            value.ShouldBe(expectedValue);
        }

        [TestCaseSource(nameof(InvalidKeyTestCases))]
        public void TryGetValue_KeyDoesNotExist_ReturnsFalse(string uutString, int key)
        {
            var uut = BuildUUT(uutString);

            var result = uut.TryGetValue(key, out var value);

            result.ShouldBeFalse();
        }

        #endregion TryGetValue() Tests

        #region Add() Tests

        [TestCaseSource(nameof(ExistingKeyWithValueTestCases))]
        public void Add_KeyValuePairExists_ReturnsSelf(string uutString, int key, int value)
        {
            var uut = BuildUUT(uutString);

            var result = uut.Add(key, value);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(ExistingKeyWithDifferentValueTestCases))]
        public void Add_KeyExistsWithDifferentValue_ThrowsException(string uutString, int key, int value)
        {
            var uut = BuildUUT(uutString);

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.Add(key, value);
            });
        }

        [TestCaseSource(nameof(NewKeyWithValueTestCases))]
        public void Add_KeyDoesNotExist_AddsKeyValuePairToResult(string uutString, int key, int value)
        {
            var uut = BuildUUT(uutString);

            var result = uut.Add(key, value);

            result.ShouldBeSet(uut.Append(new KeyValuePair<int, int>(key, value)));
        }

        #endregion Add() Tests

        #region AddRange() Tests

        [TestCaseSource(nameof(ExistingKeyValuePairsTestCases))]
        public void AddRange_AllKeyValuePairsExist_ReturnsSelf(string uutString, string keyValuePairsString)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var result = uut.AddRange(keyValuePairs);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(InvalidKeyValuePairsTestCases))]
        public void AddRange_SomeKeyValuePairsAreInvalid_ThrowsException(string uutString, string keyValuePairsString)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.AddRange(keyValuePairs);
            });
        }

        [TestCaseSource(nameof(NewKeyValuePairsTestCases))]
        public void AddRange_SomeKeyValuePairsDoNotExist_AddsNewKeyValuePairsToResult(string uutString, string keyValuePairsString)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var result = uut.AddRange(keyValuePairs);

            result.ShouldBeSet(uut.Concat(
                keyValuePairs.Where(pair => !uut.ContainsKey(pair.Key))));
        }

        [TestCaseSource(nameof(ValidKeyValuePairsWithInvalidCapacityTestCases))]
        public void AddRange_CapacityIsInvalid_ThrowsException(string uutString, string keyValuePairsString, int capacity)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = uut.AddRange(keyValuePairs, capacity);
            });
        }

        [TestCaseSource(nameof(ExistingKeyValuePairsWithValidCapacityTestCases))]
        public void AddRange_AllKeyValuePairsExist_ReturnsSelf(string uutString, string keyValuePairsString, int capacity)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var result = uut.AddRange(keyValuePairs, capacity);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(InvalidKeyValuePairsWithValidCapacityTestCases))]
        public void AddRange_SomeKeyValuePairsAreInvalid_ThrowsException(string uutString, string keyValuePairsString, int capacity)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.AddRange(keyValuePairs, capacity);
            });
        }

        [TestCaseSource(nameof(NewKeyValuePairsWithValidCapacityTestCases))]
        public void AddRange_SomeKeyValuePairsDoNotExist_AddsNewKeyValuePairsToResult(string uutString, string keyValuePairsString, int capacity)
        {
            var uut = BuildUUT(uutString);
            var keyValuePairs = ParseKeyValuePairsString(keyValuePairsString);

            var result = uut.AddRange(keyValuePairs, capacity);

            result.ShouldBeSet(uut.Concat(
                keyValuePairs.Where(pair => !uut.ContainsKey(pair.Key))));
        }

        [TestCaseSource(nameof(ExistingValuesWithKeySelectorTestCases))]
        public void AddRange_AllValuesWithSelectedKeyExist_ReturnsSelf(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            var result = uut.AddRange(values, keySelector);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(InvalidValuesWithKeySelectorTestCases))]
        public void AddRange_SomeValuesWithSelectedKeyAreInvalid_ThrowsException(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.AddRange(values, keySelector);
            });
        }

        [TestCaseSource(nameof(NewValuesWithKeySelectorTestCases))]
        public void AddRange_SomeValuesWithSelectedKeyDoNotExist_AddsNewKeyValuePairsToResult(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            var result = uut.AddRange(values, keySelector);

            result.ShouldBeSet(uut.Concat(
                values
                    .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value))
                    .Where(pair => !uut.ContainsKey(pair.Key))));
        }

        [TestCaseSource(nameof(ValidValuesWithKeySelectorAndInvalidCapacityTestCases))]
        public void AddRange_CapacityIsInvalid_ThrowsException(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = uut.AddRange(values, keySelector, capacity);
            });
        }

        [TestCaseSource(nameof(ExistingValuesWithKeySelectorAndValidCapacityTestCases))]
        public void AddRange_AllValuesWithSelectedKeyExist_ReturnsSelf(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            var result = uut.AddRange(values, keySelector, capacity);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(InvalidValuesWithKeySelectorAndValidCapacityTestCases))]
        public void AddRange_SomeValuesWithSelectedKeyAreInvalid_ThrowsException(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.AddRange(values, keySelector, capacity);
            });
        }

        [TestCaseSource(nameof(NewValuesWithKeySelectorAndValidCapacityTestCases))]
        public void AddRange_SomeValuesWithSelectedKeyDoNotExist_AddsNewKeyValuePairsToResult(string uutString, int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();
            var uut = BuildUUT(uutString);

            var result = uut.AddRange(values, keySelector, capacity);

            result.ShouldBeSet(uut.Concat(
                values
                    .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value))
                    .Where(pair => !uut.ContainsKey(pair.Key))));
        }

        #endregion AddRange() Tests

        #region Remove() Tests

        [TestCaseSource(nameof(InvalidKeyTestCases))]
        public void Remove_KeyDoesNotExist_ReturnsSelf(string uutString, int key)
        {
            var uut = BuildUUT(uutString);

            var result = uut.Remove(key);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(ValidKeyTestCases))]
        public void Remove_KeyExists_RemovesKeyValuePairFromResult(string uutString, int key)
        {
            var uut = BuildUUT(uutString);

            var result = uut.Remove(key);

            result.ShouldBeSet(uut.Where(pair => pair.Key != key));
        }

        #endregion Remove() Tests

        #region RemoveRange() Tests

        [TestCaseSource(nameof(InvalidKeysTestCases))]
        public void RemoveRange_AllKeysDoNotExist_ReturnsSelf(string uutString, int[] keys)
        {
            var uut = BuildUUT(uutString);

            var result = uut.RemoveRange(keys);

            result.ShouldBeSameAs(uut);
        }

        [TestCaseSource(nameof(ValidKeysTestCases))]
        [TestCaseSource(nameof(SomeValidKeysTestCases))]
        public void RemoveRange_SomeKeysExists_RemovesKeyValuePairFromResult(string uutString, int[] keys)
        {
            var uut = BuildUUT(uutString);

            var result = uut.RemoveRange(keys);

            result.ShouldBeSet(uut.Where(pair => !keys.Contains(pair.Key)));
        }

        #endregion RemoveRange() Tests
    }
}
