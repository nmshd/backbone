using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Application.Extensions;

public enum DbUpdateExceptionReason
{
    DuplicateIndex,
    UniqueKeyViolation
}

public static class DbUpdateExceptionExtensions
{
    public static bool HasReason(this DbUpdateException ex, DbUpdateExceptionReason reason)
    {
        switch (reason)
        {
            case DbUpdateExceptionReason.DuplicateIndex:
                return ex.Message.Contains("Index") || ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE");
            case DbUpdateExceptionReason.UniqueKeyViolation:
                return ex.GetBaseException() is SqlException { Number: 2627 } // SqlServer
                    or Npgsql.PostgresException { SqlState: "23505" };
            default:
                throw new ArgumentException("The given reason does not exist.");
        }
    }
}
