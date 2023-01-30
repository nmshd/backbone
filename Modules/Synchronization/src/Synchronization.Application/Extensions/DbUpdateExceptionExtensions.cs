using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public enum DbUpdateExceptionReason
{
    DuplicateIndex
}

public static class DbUpdateExceptionExtensions
{
    public static bool HasReason(this DbUpdateException ex, DbUpdateExceptionReason reason)
    {
        if (reason == DbUpdateExceptionReason.DuplicateIndex)
            return ex.Message.Contains("Index") || ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE");

        throw new ArgumentException("The given reason does not exist.");
    }
}
