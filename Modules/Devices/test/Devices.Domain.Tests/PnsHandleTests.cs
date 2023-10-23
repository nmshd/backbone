using Backbone.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Devices.Domain.Aggregates.PushNotifications.Handles;
using FluentAssertions;
using Xunit;

namespace Backbone.Devices.Domain.Tests;
public class PnsHandleTests
{
    [Fact]
    public void Can_create_fcm_handle()
    {
        var parseHandleResult = PnsHandle.Parse("c179FjXDVoUhoSs8LIbNCKpMxWo19sCqzXaRsodK5u1vLZlZaL3gie17RmbvMOCyKogJcZ4GYUYp2xa3f2VONkUBPzwmk8fccX2TBIImatePSUGxYGcfmmpEO1PbxGBH0E6PF5x7kpQdxsxr8TKhtG", PushNotificationPlatform.Fcm);
        parseHandleResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Can_create_apns_handle()
    {
        var parseHandleResult = PnsHandle.Parse("f2nnR7S3AK599-Aww43ZZFqhH0mazkA9CkAOg-cCt73VWnKAxEKRt5zWAB-IzB4u9Btt5TF2RvFWWDk2okYkWndFUP6-72wyu70Fkqey6fMv2uFVAiddUd9IU1-D8bJhP0sRjDis4vZfDrLcFEDcng", PushNotificationPlatform.Apns);
        parseHandleResult.IsSuccess.Should().BeTrue();
    }
}
