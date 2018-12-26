namespace WorkPump.Common.Messaging
{
    public interface INotificationHandler<in TNotification>
        where TNotification : class, INotification
    {
        void HandleNotification(TNotification notification);
    }
}
