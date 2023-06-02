namespace Backbone.Modules.Devices.Application.Clients.DTOs;

public class ClientDTO
{
    public ClientDTO(string? clientId, string? displayName)
    {
        ClientId = clientId;
        DisplayName = displayName;
    }
    public string? ClientId { get; set; }

    public string? DisplayName { get; set; }
}
