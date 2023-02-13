using Backbone.Modules.Messages.Domain.Ids;

namespace Messages.Jobs.SanityCheck.Infrastructure.Reporter
{
    public class LogReporter : IReporter
    {
        private readonly ILogger<LogReporter> _logger;
        private readonly List<MessageId> _databaseIds;
        private readonly List<string> _blobIds;

        public LogReporter(ILogger<LogReporter> logger)
        {
            _logger = logger;

            _databaseIds = new List<MessageId>();
            _blobIds = new List<string>();
        }

        public void Complete()
        {
            foreach (var databaseId in _databaseIds)
            {
                _logger.LogError("no blob found for message id: {databaseId}", databaseId);
            }

            foreach (var blobId in _blobIds)
            {
                _logger.LogError("no database entry found for blob id: {blobId}", blobId);
            }
        }

        public void ReportOrphanedBlobId(string id)
        {
            _blobIds.Add(id);
        }

        public void ReportOrphanedDatabaseId(MessageId id)
        {
            _databaseIds.Add(id);
        }
    }
}
