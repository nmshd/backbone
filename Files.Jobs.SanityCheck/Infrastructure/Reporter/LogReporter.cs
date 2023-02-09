using Backbone.Modules.Files.Domain.Entities;

namespace Files.Jobs.SanityCheck.Infrastructure.Reporter
{
    public class LogReporter : IReporter
    {
        private readonly ILogger<LogReporter> _logger;
        private readonly List<FileId> _databaseids;
        private readonly List<string> _blobids;

        public LogReporter(ILogger<LogReporter> logger)
        {
            _logger = logger;

            _databaseids = new List<FileId>();
            _blobids = new List<string>();
        }

        public void Complete()
        {
            foreach (var databaseId in _databaseids)
            {
                _logger.LogError("no blob found for file id: {databaseId}", databaseId);
            }

            foreach (var blobId in _blobids)
            {
                _logger.LogError("no database entry found for blob id: {blobId}", blobId);
            }
        }

        public void ReportOrphanedBlobId(string id)
        {
            _blobids.Add(id);
        }

        public void ReportOrphanedDatabaseId(FileId id)
        {
            _databaseids.Add(id);
        }
    }
}
