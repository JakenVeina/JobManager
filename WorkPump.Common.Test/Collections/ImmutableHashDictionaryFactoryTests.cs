using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test.Collections
{
    [TestFixture]
    public class ImmutableHashDictionaryFactoryTests
    {
        #region Create() Tests

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.KeyValuePairsTestCases))]
        public void Create_CapacityIsNotGiven_ResultSetEqualsKeyValuePairs(string keyValuePairString)
        {
            var keyValuePairs = ImmutableHashDictionaryTests.ParseKeyValuePairsString(keyValuePairString);

            var result = ImmutableHashDictionary.Create(keyValuePairs);

            result.ShouldBeSet(keyValuePairs);
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.KeyValuePairsWithInvalidCapacityTestCases))]
        public void Create_CapacityIsInvalid_ThrowsException(string keyValuePairString, int capacity)
        {
            var keyValuePairs = ImmutableHashDictionaryTests.ParseKeyValuePairsString(keyValuePairString);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = ImmutableHashDictionary.Create(keyValuePairs, capacity);
            });
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.KeyValuePairsWithValidCapacityTestCases))]
        public void Create_CapacityIsValid_ResultSetEqualsKeyValuePairs(string keyValuePairString, int capacity)
        {
            var keyValuePairs = ImmutableHashDictionaryTests.ParseKeyValuePairsString(keyValuePairString);

            var result = ImmutableHashDictionary.Create(keyValuePairs, capacity);

            result.ShouldBeSet(keyValuePairs);
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.ValuesWithKeySelectorTestCases))]
        public void Create_CapacityIsNotGiven_ResultSetEqualsValuesWithKeySelector(int[] values, Expression<Func<int, int>> keySelectorExpression)
        {
            var keySelector = keySelectorExpression.Compile();

            var result = ImmutableHashDictionary.Create(values, keySelector);

            result.ShouldBeSet(values
                .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value)));
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.ValuesWithKeySelectorAndInvalidCapacityTestCases))]
        public void Create_CapacityIsInvalid_ThrowsException(int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = ImmutableHashDictionary.Create(values, keySelector, capacity);
            });
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.ValuesWithKeySelectorAndValidCapacityTestCases))]
        public void Create_CapacityIsValid_ResultSetEqualsValuesWithKeySelector(int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();

            var result = ImmutableHashDictionary.Create(values, keySelector, capacity);

            result.ShouldBeSet(values
                .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value)));
        }

        #endregion Create() Tests

        #region ToImmutableHashDictionary() Tests

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.KeyValuePairsTestCases))]
        public void ToImmutableHashDictionary_CapacityIsNotGiven_ResultSetEqualsKeyValuePairs(string keyValuePairString)
        {
            var keyValuePairs = ImmutableHashDictionaryTests.ParseKeyValuePairsString(keyValuePairString);

            var result = keyValuePairs.ToImmutableHashDictionary();

            result.ShouldBeSet(keyValuePairs);
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.KeyValuePairsWithInvalidCapacityTestCases))]
        public void ToImmutableHashDictionary_CapacityIsInvalid_ThrowsException(string keyValuePairString, int capacity)
        {
            var keyValuePairs = ImmutableHashDictionaryTests.ParseKeyValuePairsString(keyValuePairString);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = keyValuePairs.ToImmutableHashDictionary(capacity);
            });
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.KeyValuePairsWithValidCapacityTestCases))]
        public void ToImmutableHashDictionary_CapacityIsValid_ResultSetEqualsKeyValuePairs(string keyValuePairString, int capacity)
        {
            var keyValuePairs = ImmutableHashDictionaryTests.ParseKeyValuePairsString(keyValuePairString);

            var result = keyValuePairs.ToImmutableHashDictionary(capacity);

            result.ShouldBeSet(keyValuePairs);
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.ValuesWithKeySelectorTestCases))]
        public void ToImmutableHashDictionary_CapacityIsNotGiven_ResultSetEqualsValuesWithKeySelector(int[] values, Expression<Func<int, int>> keySelectorExpression)
        {
            var keySelector = keySelectorExpression.Compile();

            var result = values.ToImmutableHashDictionary(keySelector);

            result.ShouldBeSet(values
                .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value)));
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.ValuesWithKeySelectorAndInvalidCapacityTestCases))]
        public void ToImmutableHashDictionary_CapacityIsInvalid_ThrowsException(int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var result = values.ToImmutableHashDictionary(keySelector, capacity);
            });
        }

        [TestCaseSource(typeof(ImmutableHashDictionaryTests), nameof(ImmutableHashDictionaryTests.ValuesWithKeySelectorAndValidCapacityTestCases))]
        public void ToImmutableHashDictionary_CapacityIsValid_ResultSetEqualsValuesWithKeySelector(int[] values, Expression<Func<int, int>> keySelectorExpression, int capacity)
        {
            var keySelector = keySelectorExpression.Compile();

            var result = values.ToImmutableHashDictionary(keySelector, capacity);

            result.ShouldBeSet(values
                .Select(value => new KeyValuePair<int, int>(keySelector.Invoke(value), value)));
        }

        #endregion ToImmutableHashDictionary() Tests
    }
}
