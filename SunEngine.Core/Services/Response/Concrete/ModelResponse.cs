namespace SunEngine.Core.Services.Response.Concrete
{
    public class ModelResponse<T> : BaseResponse
    {
        public T Model { get; set; }
    }
}