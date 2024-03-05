using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database;

public class FilesDbContext : AbstractDbContextBase, IFilesDbContext
{
    public FilesDbContext() { }

    public FilesDbContext(DbContextOptions<FilesDbContext> options) : base(options) { }

    public FilesDbContext(DbContextOptions<FilesDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider) { }

    public DbSet<File> FileMetadata { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<FileId>().AreUnicode(false).AreFixedLength().HaveMaxLength(FileId.MAX_LENGTH).HaveConversion<FileIdEntityFrameworkValueConverter>();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    base.OnConfiguring(optionsBuilder);
    //    optionsBuilder.UseSqlServer();
    //}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(FilesDbContext).Assembly);
    }
}
