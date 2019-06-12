namespace SunEngine.Core.Services.Request
{
    public class ByModelRequest<T> : BaseRequest
    {
        public T Model { get; set; }
    }
}