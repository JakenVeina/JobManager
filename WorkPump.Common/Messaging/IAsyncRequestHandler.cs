using System.Threading;
using System.Threading.Tasks;

namespace WorkPump.Common.Messaging
{
    public interface IAsyncRequestHandler<in TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TResponse : class
    {
        Task<TResponse> HandleRequestAsync(TRequest request, CancellationToken cancellationToken);
    }
}
