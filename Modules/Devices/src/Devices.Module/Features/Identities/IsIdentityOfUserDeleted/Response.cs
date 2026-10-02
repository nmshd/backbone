namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

public class IsIdentityOfUserDeletedResponse
{
    public IsIdentityOfUserDeletedResponse(bool isDeleted, DateTime? deletionDate)
    {
        IsDeleted = isDeleted;
        DeletionDate = deletionDate;
    }

    public bool IsDeleted { get; set; }
    public DateTime? DeletionDate { get; set; }
}
