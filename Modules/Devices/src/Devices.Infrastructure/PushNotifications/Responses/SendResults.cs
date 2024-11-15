namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

public class SendResults
{
    private readonly List<SendResult> _failures = [];
    private readonly List<SendResult> _successes = [];

    public SendResults(IEnumerable<SendResult> results)
    {
        foreach (var result in results)
        {
            if (result.IsSuccess)
                _successes.Add(result);
            else
                _failures.Add(result);
        }
    }

    public IEnumerable<SendResult> Failures => _failures;

    public IEnumerable<SendResult> Successes => _successes;
}
