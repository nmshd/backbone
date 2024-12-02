using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.Application;

public static class ApplicationErrors
{
    public static class Authentication
    {
        public static ApplicationError InvalidOAuthRequest(string reason = "")
        {
            var formattedReason = reason.IsNullOrEmpty() ? "" : $" ({reason})";
            return new ApplicationError("error.platform.validation.authentication.invalidOAuthRequest", string.IsNullOrEmpty(reason) ? $"The OAuth request is invalid{formattedReason}." : reason);
        }
    }

    public static class Devices
    {
        public static ApplicationError RegistrationFailed(string message = "")
        {
            return new ApplicationError("error.platform.validation.device.registrationFailed", string.IsNullOrEmpty(message) ? "The registration of the device failed." : message);
        }

        public static ApplicationError AddressAlreadyExists()
        {
            return new ApplicationError("error.platform.validation.device.addressAlreadyExists",
                "The address derived from the given public key already exists. Try again with a different public key.");
        }

        public static ApplicationError InvalidSignature()
        {
            return new ApplicationError("error.platform.validation.device.invalidSignature", "The given signature is not valid.");
        }

        public static ApplicationError ChallengeHasExpired()
        {
            return new ApplicationError("error.platform.validation.device.challengeHasExpired", "The given challenge has expired. Obtain a new challenge and try again.");
        }

        public static ApplicationError ChangePasswordFailed(string message = "")
        {
            return new ApplicationError("error.platform.validation.device.changePasswordFailed", string.IsNullOrEmpty(message) ? "Changing the password of the device failed." : message);
        }

        public static ApplicationError ClientIdAlreadyExists()
        {
            return new ApplicationError("error.platform.validation.device.clientIdAlreadyExists", "A client with the given client id already exists. Try a different client id.");
        }

        public static ApplicationError TierNameAlreadyExists()
        {
            return new ApplicationError("error.platform.validation.device.tierNameAlreadyExists", "A tier with the given tier name already exists. Try a different tier name.");
        }

        public static ApplicationError InvalidTierIdOrDoesNotExist()
        {
            return new ApplicationError("error.platform.validation.device.tierIdInvalidOrDoesNotExist", "The passed tier ID is not valid or the tier does not exist.");
        }

        public static ApplicationError ClientReachedIdentitiesLimit()
        {
            return new ApplicationError("error.platform.validation.device.clientReachedIdentitiesLimit",
                "The client's Identity limit has been reached. A new Identity cannot be created with this client.");
        }

        public static ApplicationError BackupDeviceAlreadyExists()
        {
            return new ApplicationError("error.platform.validation.device.onlyOneBackupDeviceCanExist",
                "Only one backup device can be created per identity.");
        }
    }
}
