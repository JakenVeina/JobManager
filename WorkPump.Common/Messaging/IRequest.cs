namespace WorkPump.Common.Messaging
{
    public interface IRequest<in TResponse>
        where TResponse : class
    { }
}
