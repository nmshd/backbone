using Enmeshed.DevelopmentKit.Identity.ValueObjects;

#pragma warning disable CS8618 // turn off nullable checks
namespace Enmeshed.DevelopmentKit.Identity.Entities
{
    public class Identity
    {
        public IdentityAddress Address { get; set; }
        public byte[] PublicKey { get; set; }
        public IdentityType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Device> Devices { get; set; }
    }

    public enum IdentityType
    {
        Normal = 0,
        Test = 10,
        End2EndTest = 20
    }

    public static class IdentityTypeExtensions
    {
        public static string IntValueAsString(this IdentityType userType)
        {
            return ((int) userType).ToString();
        }
    }
}