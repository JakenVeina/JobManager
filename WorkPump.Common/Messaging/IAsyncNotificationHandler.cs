using System.Threading;
using System.Threading.Tasks;

namespace WorkPump.Common.Messaging
{
    public interface IAsyncNotificationHandler<in TNotification>
        where TNotification : class, INotification
    {
        Task HandleNotificationAsync(TNotification notification, CancellationToken cancellationToken);
    }
}
