using System;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Services
{
    public interface IDbExceptionParserProvider
    {
        /// <summary>
        ///  Parse exception raised by DB provider
        /// </summary>
        /// <param name="e">exception object</param>
        /// <returns>Error message extracted from the exception</returns>
        string Parse(Exception e);

        /// <summary>
        /// try to parse the exception and if it is a known one, it will parse the error and raise the new one
        /// </summary>
        /// <param name="ex"></param>
        void ParseAndRaise(Exception ex);
    }
}