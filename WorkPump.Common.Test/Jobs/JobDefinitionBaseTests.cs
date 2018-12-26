using WorkPump.Common.Jobs;

using Moq;
using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test.Jobs
{
    [TestFixture]
    public class JobDefinitionBaseTests
    {
        #region Constructor() Tests

        [TestCaseSource(typeof(TestCases), nameof(TestCases.UInt64))]
        public void Constructor_Always_IdIsGiven(ulong id)
        {
            var result = new Mock<JobDefinitionBase>(id).Object;

            result.Id.ShouldBe(id);
        }

        #endregion Constructor() Tests
    }
}
