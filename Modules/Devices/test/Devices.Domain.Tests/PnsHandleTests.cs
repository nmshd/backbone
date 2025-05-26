using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;

namespace Backbone.Modules.Devices.Domain.Tests;

public class PnsHandleTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_fcm_handle()
    {
        var parseHandleResult = PnsHandle.Parse(PushNotificationPlatform.Fcm,
            "c179FjXDVoUhoSs8LIbNCKpMxWo19sCqzXaRsodK5u1vLZlZaL3gie17RmbvMOCyKogJcZ4GYUYp2xa3f2VONkUBPzwmk8fccX2TBIImatePSUGxYGcfmmpEO1PbxGBH0E6PF5x7kpQdxsxr8TKhtG");
        parseHandleResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Can_create_apns_handle()
    {
        var parseHandleResult = PnsHandle.Parse(PushNotificationPlatform.Apns,
            "f2nnR7S3AK599-Aww43ZZFqhH0mazkA9CkAOg-cCt73VWnKAxEKRt5zWAB-IzB4u9Btt5TF2RvFWWDk2okYkWndFUP6-72wyu70Fkqey6fMv2uFVAiddUd9IU1-D8bJhP0sRjDis4vZfDrLcFEDcng");
        parseHandleResult.IsSuccess.ShouldBeTrue();
    }
}
