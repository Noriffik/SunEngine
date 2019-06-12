namespace SunEngine.Core.Services.Request.Concrete
{
    public class ByIdRequest<T> : BaseRequest
    {
        public T Id { get; set; }
    }
}