using System.Collections.Generic;

namespace SunEngine.Core.Services.Response
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; } = true;
        public IList<string> Errors { get; set; }
    }
}