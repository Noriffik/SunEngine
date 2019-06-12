using System;

namespace SunEngine.Core.Exceptions.Database.Connection
{
    public class SunDatabaseConnectionException : SunDatabaseException 
    {
        public SunDatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}