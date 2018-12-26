using Moq;
using System;
using System.Linq.Expressions;

namespace Shouldly
{
    public static class MoqAssertions
    {
        public static void ShouldHaveReceived<T>(this Mock<T> mock, Expression<Action<T>> expression)
            where T : class
            => mock.Verify(expression);

        public static void ShouldNotHaveReceived<T>(this Mock<T> mock, Expression<Action<T>> expression)
            where T : class
            => mock.Verify(expression, Times.Never);
    }
}
