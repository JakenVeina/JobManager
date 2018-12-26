using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void ToEnumerable_Always_EnumeratesValue()
        {
            var value = new object();

            var result = value.ToEnumerable();

            result.Count().ShouldBe(1);
            result.Single().ShouldBeSameAs(value);
        }
    }
}
