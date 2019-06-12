using System.Collections.Generic;

namespace SunEngine.Core.Services.Response.Concrete
{
    public class ModelsResponse<T> : BaseResponse
    {
        public IList<T> Models { get; set; }
    }
}