namespace SunEngine.Core.Services.Request.Concrete
{
    public class ByModelRequest<T> : BaseRequest
    {
        public T Model { get; set; }
    }
}