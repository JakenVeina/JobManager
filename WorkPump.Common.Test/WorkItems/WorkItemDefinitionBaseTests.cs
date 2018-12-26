using WorkPump.Common.WorkItems;

using Moq;
using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test.WorkItems
{
    [TestFixture]
    public class WorkItemDefinitionBaseTests
    {
        #region Constructor() Tests

        [TestCaseSource(typeof(TestCases), nameof(TestCases.UInt64))]
        public void Constructor_Always_IdIsGiven(ulong id)
        {
            var result = new Mock<WorkItemDefinitionBase>(id).Object;

            result.Id.ShouldBe(id);
        }

        #endregion Constructor() Tests
    }
}
