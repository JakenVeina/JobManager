using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkPump.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test.Messaging
{
    [TestFixture]
    public class MessengerTests
    {
        #region Test Context

        private static (AutoMocker autoMocker, Messenger uut) BuildTestContext()
        {
            var autoMocker = new AutoMocker();

            var uut = autoMocker.CreateInstance<Messenger>();

            return (autoMocker, uut);
        }

        #endregion Test Context

        #region Dummies

        public class DummyNotification : INotification { }

        public class DummyRequest : IRequest<object> { }

        #endregion Dummies

        #region Constructor() Tests

        [Test]
        public void Constructor_Always_ServiceProviderIsGiven()
        {
            var serviceProvider = new Mock<IServiceProvider>().Object;

            var result = new Messenger(serviceProvider);

            result.ServiceProvider.ShouldBeSameAs(serviceProvider);
        }

        #endregion Constructor() Tests

        #region PublishNotificationAsync() Tests

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(1, 1)]
        public async Task PublishNotificationAsync_Always_InvokesRegisteredHandlers(int handlerCount, int asyncHandlerCount)
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockHandlers = Enumerable.Repeat(0, handlerCount)
                .Select(x => new Mock<INotificationHandler<DummyNotification>>())
                .ToArray();

            var mockAsyncHandlers = Enumerable.Repeat(0, asyncHandlerCount)
                .Select(x => new Mock<IAsyncNotificationHandler<DummyNotification>>())
                .ToArray();

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<DummyNotification>>)))
                .Returns(mockHandlers.Select(x => x.Object));

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncNotificationHandler<DummyNotification>>)))
                .Returns(mockAsyncHandlers.Select(x => x.Object));

            var notification = new DummyNotification();
            var cancellationToken = new CancellationTokenSource().Token;

            await uut.PublishNotificationAsync(notification, cancellationToken);

            mockHandlers.EachShould(handler => handler
                .ShouldHaveReceived(x => x.HandleNotification(notification)));

            mockAsyncHandlers.EachShould(handler => handler
                .ShouldHaveReceived(x => x.HandleNotificationAsync(notification, cancellationToken)));
        }

        #endregion PublishNotificationAsync() Tests

        #region PublishRequestAsync() Tests

        [Test]
        public async Task PublishRequestAsync_NoHandlersRegistered_ThrowsException()
        {
            (var autoMocker, var uut) = BuildTestContext();

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IRequestHandler<DummyRequest, object>>());

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IAsyncRequestHandler<DummyRequest, object>>());

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            await Should.ThrowAsync<InvalidOperationException>(() => uut.PublishRequestAsync(request, cancellationToken));
        }

        [TestCase(2, 0)]
        [TestCase(0, 2)]
        [TestCase(1, 1)]
        public async Task PublishRequestAsync_MultipleHandlersRegistered_ThrowsException(int handlerCount, int asyncHandlerCount)
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockHandlers = Enumerable.Repeat(0, handlerCount)
                .Select(x => new Mock<IRequestHandler<DummyRequest, object>>())
                .ToArray();

            var mockAsyncHandlers = Enumerable.Repeat(0, asyncHandlerCount)
                .Select(x => new Mock<IAsyncRequestHandler<DummyRequest, object>>())
                .ToArray();

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(mockHandlers.Select(x => x.Object));

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(mockAsyncHandlers.Select(x => x.Object));

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            await Should.ThrowAsync<InvalidOperationException>(() => uut.PublishRequestAsync(request, cancellationToken));

            mockHandlers.EachShould(mockHandler => mockHandler
                .ShouldNotHaveReceived(x => x.HandleRequest(It.IsAny<DummyRequest>())));

            mockAsyncHandlers.EachShould(mockAsyncHandler => mockAsyncHandler
                .ShouldNotHaveReceived(x => x.HandleRequestAsync(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>())));
        }

        [Test]
        public async Task PublishRequestAsync_SingleHandlerRegistered_ReturnsHandlerResponse()
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockHandler = new Mock<IRequestHandler<DummyRequest, object>>();

            var response = new object();
            mockHandler
                .Setup(x => x.HandleRequest(It.IsAny<DummyRequest>()))
                .Returns(response);

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(mockHandler.Object.ToEnumerable());

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IAsyncRequestHandler<DummyRequest, object>>());

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            var result = await uut.PublishRequestAsync(request, cancellationToken);

            result.ShouldBeSameAs(response);
            
            mockHandler.ShouldHaveReceived(x => x.HandleRequest(request));
        }

        [Test]
        public async Task PublishRequestAsync_SingleAsyncHandlerRegistered_ReturnsHandlerResponse()
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockAsyncHandler = new Mock<IAsyncRequestHandler<DummyRequest, object>>();

            var response = new object();
            mockAsyncHandler
                .Setup(x => x.HandleRequestAsync(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IRequestHandler<DummyRequest, object>>());

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(mockAsyncHandler.Object.ToEnumerable());

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            var result = await uut.PublishRequestAsync(request, cancellationToken);

            result.ShouldBeSameAs(response);

            mockAsyncHandler.ShouldHaveReceived(x => x.HandleRequestAsync(request, cancellationToken));
        }

        #endregion PublishRequestAsync() Tests

        #region TryPublishRequestAsync() Tests

        [Test]
        public async Task TryPublishRequestAsync_NoHandlersRegistered_ReturnsNull()
        {
            (var autoMocker, var uut) = BuildTestContext();

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IRequestHandler<DummyRequest, object>>());

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IAsyncRequestHandler<DummyRequest, object>>());

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            var response = await uut.TryPublishRequestAsync(request, cancellationToken);

            response.ShouldBeNull();
        }

        [TestCase(2, 0)]
        [TestCase(0, 2)]
        [TestCase(1, 1)]
        public async Task TryPublishRequestAsync_MultipleHandlersRegistered_ThrowsException(int handlerCount, int asyncHandlerCount)
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockHandlers = Enumerable.Repeat(0, handlerCount)
                .Select(x => new Mock<IRequestHandler<DummyRequest, object>>())
                .ToArray();

            var mockAsyncHandlers = Enumerable.Repeat(0, asyncHandlerCount)
                .Select(x => new Mock<IAsyncRequestHandler<DummyRequest, object>>())
                .ToArray();

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(mockHandlers.Select(x => x.Object));

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(mockAsyncHandlers.Select(x => x.Object));

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            await Should.ThrowAsync<InvalidOperationException>(() => uut.TryPublishRequestAsync(request, cancellationToken));

            mockHandlers.EachShould(mockHandler => mockHandler
                .ShouldNotHaveReceived(x => x.HandleRequest(It.IsAny<DummyRequest>())));

            mockAsyncHandlers.EachShould(mockAsyncHandler => mockAsyncHandler
                .ShouldNotHaveReceived(x => x.HandleRequestAsync(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>())));
        }

        [Test]
        public async Task TryPublishRequestAsync_SingleHandlerRegistered_ReturnsHandlerResponse()
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockHandler = new Mock<IRequestHandler<DummyRequest, object>>();

            var response = new object();
            mockHandler
                .Setup(x => x.HandleRequest(It.IsAny<DummyRequest>()))
                .Returns(response);

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(mockHandler.Object.ToEnumerable());

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IAsyncRequestHandler<DummyRequest, object>>());

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            var result = await uut.TryPublishRequestAsync(request, cancellationToken);

            result.ShouldBeSameAs(response);

            mockHandler.ShouldHaveReceived(x => x.HandleRequest(request));
        }

        [Test]
        public async Task TryPublishRequestAsync_SingleAsyncHandlerRegistered_ReturnsHandlerResponse()
        {
            (var autoMocker, var uut) = BuildTestContext();

            var mockAsyncHandler = new Mock<IAsyncRequestHandler<DummyRequest, object>>();

            var response = new object();
            mockAsyncHandler
                .Setup(x => x.HandleRequestAsync(It.IsAny<DummyRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IRequestHandler<DummyRequest, object>>)))
                .Returns(Enumerable.Empty<IRequestHandler<DummyRequest, object>>());

            autoMocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(IEnumerable<IAsyncRequestHandler<DummyRequest, object>>)))
                .Returns(mockAsyncHandler.Object.ToEnumerable());

            var request = new DummyRequest();
            var cancellationToken = new CancellationTokenSource().Token;

            var result = await uut.TryPublishRequestAsync(request, cancellationToken);

            result.ShouldBeSameAs(response);

            mockAsyncHandler.ShouldHaveReceived(x => x.HandleRequestAsync(request, cancellationToken));
        }

        #endregion TryPublishRequestAsync() Tests
    }
}
