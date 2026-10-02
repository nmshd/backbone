namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("IsIdentityOfUserDeletedResponse")]
public class Response
{
    public Response(bool isDeleted, DateTime? deletionDate)
    {
        IsDeleted = isDeleted;
        DeletionDate = deletionDate;
    }

    public bool IsDeleted { get; set; }
    public DateTime? DeletionDate { get; set; }
}
