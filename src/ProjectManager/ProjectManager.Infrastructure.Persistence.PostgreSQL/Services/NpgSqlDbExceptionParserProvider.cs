using System;
using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Services
{
    /// <summary>
    /// Parse errors that comes from databases
    /// Actually it parses only unique violation
    /// </summary>
    public class NpgSqlDbExceptionParserProvider : DbExceptionParser, IDbExceptionParserProvider
    {
        public const string MySqlViolationOfUniqueIndex = "23505";
        private readonly MessageTemplatesConfig _messageTemplates;

        public NpgSqlDbExceptionParserProvider(MessageTemplatesConfig messageTemplates)
        {
            _messageTemplates = messageTemplates;
        }

        public override string Parse(Exception e)
        {
            var dbUpdateEx = e as DbUpdateException;

            if (dbUpdateEx?.InnerException is PostgresException mySqlEx)
            {
                //This is a DbUpdateException on a SQL database

                if (mySqlEx.SqlState == MySqlViolationOfUniqueIndex)
                {
                    //We have an error we can process
                    var valError = ParseUniquenessError(mySqlEx.Message, _messageTemplates.UniqueErrorTemplate, _messageTemplates.CombinationUniqueErrorTemplate);
                    if (valError != null)
                    {
                        return valError;
                    }
                }
            }
            //TODO: add other types of exception we can handle
            //otherwise exception wasn't handled, so return null
            return null;
        }

        public override void ParseAndRaise(Exception ex)
        {
            var dbUpdateEx = ex as DbUpdateException;

            if (dbUpdateEx?.InnerException is PostgresException mySqlEx)
            {
                //This is a DbUpdateException on a SQL database

                if (mySqlEx.SqlState == MySqlViolationOfUniqueIndex)
                {
                    //We have an error we can process
                    var valError = ParseUniquenessError(mySqlEx.Message, _messageTemplates.UniqueErrorTemplate, _messageTemplates.CombinationUniqueErrorTemplate);
                    if (valError != null)
                    {
                        throw new DomainException(valError);
                    }
                }
            }
            throw ex;
        }
    }

    public class MessageTemplatesConfig
    {
        public string UniqueErrorTemplate { get; set; }
        public string CombinationUniqueErrorTemplate { get; set; }
    }
}