using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Files.Application.Tests;

public static class TestData
{
    public static class IdentityAddresses
    {
        public static readonly IdentityAddress ADDRESS_1 = IdentityAddress.Parse("id17RDEphijMPFGLbhqLWWgJfatBANMruC8f");
        public static readonly IdentityAddress ADDRESS_2 = IdentityAddress.Parse("id1HwY1TuyVBp3CmY3h18yTt1CKyu5qwB9wj");
        public static readonly IdentityAddress ADDRESS_3 = IdentityAddress.Parse("id1LMp4k1XwxZ3WFXdAn9y12tv1ofe5so4kM");
        public static readonly IdentityAddress ADDRESS_4 = IdentityAddress.Parse("id1McegXycvRoiJppS2LG25phn3jNveckFUL");
        public static readonly IdentityAddress ADDRESS_5 = IdentityAddress.Parse("id193k6K5cJr94WJEWYb6Kei8zp5CGPyrQLS");
        public static readonly IdentityAddress ADDRESS_6 = IdentityAddress.Parse("id1Gda4aTXiBX9Pyc8UnmLaG44cX46umjnea");
        public static readonly IdentityAddress ADDRESS_7 = IdentityAddress.Parse("id1NjGvLfWPrQ34PXWRBNiTfXv9DFiDQHExx");
    }

    public static class Devices
    {
        public static readonly DeviceId DEVICE_1 = DeviceId.Parse("DVC1");
        public static readonly DeviceId DEVICE_2 = DeviceId.Parse("DVC2");
        public static readonly DeviceId DEVICE_3 = DeviceId.Parse("DVC3");
        public static readonly DeviceId DEVICE_4 = DeviceId.Parse("DVC4");
        public static readonly DeviceId DEVICE_5 = DeviceId.Parse("DVC5");
        public static readonly DeviceId DEVICE_6 = DeviceId.Parse("DVC6");
        public static readonly DeviceId DEVICE_7 = DeviceId.Parse("DVC7");
        public static readonly DeviceId DEVICE_8 = DeviceId.Parse("DVC8");
        public static readonly DeviceId DEVICE_9 = DeviceId.Parse("DVC9");
        public static readonly DeviceId DEVICE_10 = DeviceId.Parse("DVC10");
    }

    public static class Payloads
    {
        public static readonly byte[] EMPTY = [];
        public static readonly byte[] PAYLOAD_1 = [1, 1, 1];
        public static readonly byte[] PAYLOAD_2 = [2, 2, 2];
        public static readonly byte[] PAYLOAD_3 = [3, 3, 3];
    }
}
