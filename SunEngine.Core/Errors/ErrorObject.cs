using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SunEngine.Core.Errors
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorType
    {
        System = 0,
        Soft = 1
    }

    public class ErrorObject
    {
        public string Code { get; private set; }

        public string Description { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        public ErrorType Type { get; private set; }

        public ErrorObject(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        public ErrorObject(string code, string description, ErrorType type, string message) : this(code, description,
            type)
        {
            Message = message;
        }

        public ErrorObject(string code, string description, ErrorType type, Exception exception) : this(code,
            description, type)
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;
        }
    }
}