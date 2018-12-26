using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace WorkPump.Common.Messaging
{
    public interface IMessenger
    {
        Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : class, INotification;

        Task<TResponse> PublishRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse : class;

        Task<TResponse?> TryPublishRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse : class;
    }

    public class Messenger : IMessenger
    {
        public Messenger(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
            where TNotification : class, INotification
        {
            foreach (var handler in ServiceProvider.GetServices<INotificationHandler<TNotification>>())
                handler.HandleNotification(notification);

            foreach (var handler in ServiceProvider.GetServices<IAsyncNotificationHandler<TNotification>>())
                await handler.HandleNotificationAsync(notification, cancellationToken);
        }

        public async Task<TResponse> PublishRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
            where TResponse : class
        {
            var result = await TryPublishRequestAsync(request, cancellationToken);

            if (result is null)
                throw new InvalidOperationException($"No handler is registered for {nameof(IRequest<TResponse>)} type {request.GetType().FullName}.");

            return result;
        }

        public Task<TResponse?> TryPublishRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
                where TResponse : class
            => (Task<TResponse?>)GetType()
                .GetMethod(nameof(DoPublishRequestAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(request.GetType(), typeof(TResponse))
                .Invoke(this, new object[] { request, cancellationToken });

        internal protected IServiceProvider ServiceProvider { get; }

        private async Task<TResponse?> DoPublishRequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
            where TResponse : class
        {
            var handlers = ServiceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().Cast<object>()
                .Concat(ServiceProvider.GetServices<IAsyncRequestHandler<TRequest, TResponse>>().Cast<object>());

            var handlerEnumerator = handlers.GetEnumerator();

            if (!handlerEnumerator.MoveNext())
                return null;

            var handler = handlerEnumerator.Current;

            if (handlerEnumerator.MoveNext())
                throw new InvalidOperationException($"Multiple handlers are registered for {nameof(IRequest<TResponse>)} type {typeof(TRequest).FullName}. Only one handler may be registered for a given request type.");

            if (handler is IRequestHandler<TRequest, TResponse> requestHandler)
                return requestHandler.HandleRequest(request);

            return await (handler as IAsyncRequestHandler<TRequest, TResponse>)!.HandleRequestAsync(request, cancellationToken);
        }
    }
}
