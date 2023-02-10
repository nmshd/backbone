using Backbone.Modules.Synchronization.Domain.Entities;

namespace Synchronization.Jobs.SanityCheck.Infrastructure.Reporter
{
    public class LogReporter : IReporter
    {
        private readonly ILogger<LogReporter> _logger;
        private readonly List<DatawalletModificationId> _databaseids;
        private readonly List<string> _blobids;

        public LogReporter(ILogger<LogReporter> logger)
        {
            _logger = logger;

            _databaseids = new List<DatawalletModificationId>();
            _blobids = new List<string>();
        }

        public void Complete()
        {
            foreach (var databaseId in _databaseids)
            {
                _logger.LogError("no blob found for datawallet modification id: {databaseId}", databaseId);
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

        public void ReportOrphanedDatabaseId(DatawalletModificationId id)
        {
            _databaseids.Add(id);
        }
    }
}
