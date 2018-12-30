using Moq;
using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test
{
    [TestFixture]
    public class EntityBaseTests
    {
        [TestCaseSource(typeof(TestCases), nameof(TestCases.UInt64))]
        public void Constructor_Always_IdIsGiven(ulong id)
        {
            var result = new Mock<EntityBase<ulong>>(id).Object;

            result.Id.ShouldBe(id);
        }
    }
}
