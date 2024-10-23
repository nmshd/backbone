using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        return reason switch
        {
            DbUpdateExceptionReason.DuplicateIndex => ex.Message.Contains("Index") || ex.InnerException != null && ex.InnerException.Message.ToLower().Contains("unique"),
            DbUpdateExceptionReason.UniqueKeyViolation => ex.GetBaseException() is SqlException { Number: 2627 } // SqlServer
                or PostgresException { SqlState: "23505" },
            _ => throw new ArgumentException("The given reason does not exist.")
        };
    }
}
