namespace Backbone.Modules.Devices.Application.Identities.Queries.IsIdentityOfUserDeleted;

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
