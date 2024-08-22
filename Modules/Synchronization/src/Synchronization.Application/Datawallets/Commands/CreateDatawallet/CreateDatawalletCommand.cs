using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.CreateDatawallet;

public class CreateDatawalletCommand : IRequest<CreateDatawalletResponse>
{
    public CreateDatawalletCommand(ushort datawalletVersion)
    {
        DatawalletVersion = datawalletVersion;
    }

    public ushort DatawalletVersion { get; set; }
}
