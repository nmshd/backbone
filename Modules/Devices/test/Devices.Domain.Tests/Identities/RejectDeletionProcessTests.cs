// using Backbone.BuildingBlocks.Domain.Exceptions;
// using Backbone.DevelopmentKit.Identity.ValueObjects;
// using Backbone.Modules.Devices.Domain.Aggregates.Tier;
// using Backbone.Modules.Devices.Domain.Entities.Identities;
// using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
// using Backbone.Tooling;
//
// namespace Backbone.Modules.Devices.Domain.Tests.Identities;
//
// public class RejectDeletionProcessTests : AbstractTestsBase
// {
//     [Fact]
//     public void Reject_deletion_process_waiting_for_approval()
//     {
//         // Arrange
//         SystemTime.Set(DateTime.Parse("2020-01-01"));
//         var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
//         identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
//         var deviceId = identity.Devices[0].Id;
//
//         // Act
//         identity.RejectDeletionProcess(identity.DeletionProcesses[0].Id, deviceId);
//
//         // Assert
//         var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Rejected)!;
//         AssertAuditLogEntryWasCreated(deletionProcess);
//     }
//
//     [Fact]
//     public void Throws_when_device_not_owned_by_identity()
//     {
//         // Arrange
//         var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
//
//         // Act
//         var acting = () => identity.RejectDeletionProcess(identity.DeletionProcesses[0].Id, DeviceId.Parse("DVC"));
//
//         // Assert
//         var exception = acting.Should().Throw<DomainException>().Which;
//
//         exception.Code.Should().Be("error.platform.recordNotFound");
//         exception.Message.Should().Contain("Device");
//     }
//
//     [Fact]
//     public void Throws_when_deletion_process_does_not_exist()
//     {
//         // Arrange
//         var identity = CreateIdentity();
//         identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
//         var deviceId = identity.Devices[0].Id;
//         var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;
//
//         // Act
//         var acting = () => identity.RejectDeletionProcess(deletionProcessId, deviceId);
//
//         // Assert
//         var exception = acting.Should().Throw<DomainException>().Which;
//
//         exception.Code.Should().Be("error.platform.recordNotFound");
//         exception.Message.Should().Contain("IdentityDeletionProcess");
//     }
//
//     [Fact]
//     public void Throws_when_deletion_process_is_not_in_status_waiting_for_approval()
//     {
//         // Arrange
//         var identity = CreateIdentity();
//         identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
//         var deviceId = identity.Devices[0].Id;
//         var deletionProcess = identity.StartDeletionProcessAsOwner(deviceId);
//
//         // Act
//         var acting = () => identity.RejectDeletionProcess(deletionProcess.Id, deviceId);
//
//         // Assert
//         var exception = acting.Should().Throw<DomainException>().Which;
//
//         exception.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
//         exception.Message.Should().Contain("WaitingForApproval");
//     }
//
//     private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
//     {
//         deletionProcess.AuditLog.Should().HaveCount(2);
//
//         var auditLogEntry = deletionProcess.AuditLog[1];
//         auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
//         auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
//         auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
//         auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
//         auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Rejected);
//     }
//
//     private static Identity CreateIdentity()
//     {
//         var address = IdentityAddress.Create([], "prod.enmeshed.eu");
//         return new Identity("", address, [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE);
//     }
//
//     private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
//     {
//         var identity = CreateIdentity();
//         Hasher.SetHasher(new DummyHasher([1, 2, 3]));
//         identity.StartDeletionProcessAsSupport();
//         return identity;
//     }
// }


