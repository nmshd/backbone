using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.MessageDistributor;

public interface IMessageDistributor
{
    public void Distribute(IList<PoolEntry> pools);
}

public class MessageDistributorTools
{
    protected internal static void CalculateSentAndReceivedMessages(IList<PoolEntry> pools, PoolFileConfiguration poolsConfiguration)
    {
        var messagesSentByConnectorRatio = poolsConfiguration.MessagesSentByConnectorRatio;
        var messagesSentByAppRatio = 1 - messagesSentByConnectorRatio;

        foreach (var pool in pools.Where(p => p.TotalNumberOfMessages > 0))
        {
            if (pool.IsApp())
            {
                pool.NumberOfSentMessages = Convert.ToUInt32(decimal.Ceiling(pool.TotalNumberOfMessages * messagesSentByAppRatio));
            }
            else if (pool.IsConnector())
            {
                pool.NumberOfSentMessages = Convert.ToUInt32(decimal.Ceiling(pool.TotalNumberOfMessages * messagesSentByConnectorRatio));
            }
            else
            {
                throw new Exception("Pools that are neither app nor connector cannot send messages.");
            }
            pool.NumberOfReceivedMessages = pool.TotalNumberOfMessages - pool.NumberOfSentMessages;

            if (pool.NumberOfReceivedMessages == 0 || pool.NumberOfSentMessages == 0)
            {
                throw new Exception(
                    $"The resulting number of sent/received messages for pool {pool.Name} is zero. Please use a higher number and/or adjust the ratio. Otherwise, the number of messages will not match.");
            }
        }
    }
}
