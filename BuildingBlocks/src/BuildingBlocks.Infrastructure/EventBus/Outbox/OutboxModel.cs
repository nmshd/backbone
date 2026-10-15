using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public static class OutboxModel
{
    public static void Configure(ModelBuilder builder, string? provider)
    {
        var message = builder.Entity<OutboxMessage>();
        message.ToTable("OutboxMessages");
        message.HasKey(x => x.Id);
        message.Property(x => x.Id).ValueGeneratedNever();
        var sequence = message.Property(x => x.Sequence).ValueGeneratedOnAdd();
        if (provider == "Microsoft.EntityFrameworkCore.SqlServer")
            sequence.UseIdentityColumn();
        else if (provider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            sequence.UseIdentityByDefaultColumn();
        if (provider == "Microsoft.EntityFrameworkCore.Sqlite")
            sequence.ValueGeneratedNever();
        message.Property(x => x.EventId).HasMaxLength(128);
        message.Property(x => x.EventName).HasMaxLength(200);
        message.Property(x => x.EventType).HasMaxLength(500);
        message.Property(x => x.Stream).HasMaxLength(64).IsUnicode(false);
        message.Property(x => x.CorrelationId).HasMaxLength(256);
        message.Property(x => x.TraceParent).HasMaxLength(128);
        message.Property(x => x.TraceState).HasMaxLength(512);
        message.Property(x => x.LastError).HasMaxLength(500);
        message.HasIndex(x => new { x.NextAttemptAt, x.Sequence })
            .HasFilter("\"PublishedAt\" IS NULL AND \"FailedAt\" IS NULL");
        message.HasIndex(x => new { x.SaveId, x.Stream, x.Sequence });
        message.HasIndex(x => new { x.Stream, x.Sequence }).HasFilter("\"PublishedAt\" IS NULL");
        var inbox = builder.Entity<InboxMessage>();
        inbox.ToTable("InboxMessages");
        inbox.HasKey(x => new { x.EventId, x.Handler });
        inbox.Property(x => x.EventId).HasMaxLength(128);
        inbox.Property(x => x.Handler).HasMaxLength(300);
        inbox.HasIndex(x => x.ProcessedAt);
    }
}
