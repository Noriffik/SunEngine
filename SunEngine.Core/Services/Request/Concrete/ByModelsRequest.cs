using System.Collections.Generic;

namespace SunEngine.Core.Services.Request.Concrete
{
    public class ByModelsRequest<T> : BaseRequest
    {
        public List<T> Models { get; set; }
    }
}