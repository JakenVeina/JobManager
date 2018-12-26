namespace WorkPump.Common.Messaging
{
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TResponse : class
    {
        TResponse HandleRequest(TRequest request);
    }
}
