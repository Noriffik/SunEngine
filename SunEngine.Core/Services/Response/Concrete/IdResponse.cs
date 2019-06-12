namespace SunEngine.Core.Services.Response.Concrete
{
    public class IdResponse<T> : BaseResponse
    {
        public T Id { get; set; }
    }
}