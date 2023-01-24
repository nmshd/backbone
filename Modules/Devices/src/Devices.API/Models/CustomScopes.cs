namespace Devices.API.Models;

public static class CustomScopes
{
    public static class Apis
    {
        public const string CHALLENGES = "challenges";
        public const string DEVICES = "devices";
        public const string SYNCHRONIZATION = "synchronization";
        public const string MESSAGES = "messages";
        public const string TOKENS = "tokens";
        public const string RELATIONSHIPS = "relationships";
        public const string FILES = "files";
    }

    public static class IdentityResources
    {
        public const string IDENTITY_INFORMATION = "identity_information";
        public const string DEVICE_INFORMATION = "device_information";
    }
}
