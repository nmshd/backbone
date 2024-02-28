using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices;
public class DevicesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<List<Device>>> ListDevices(PaginationFilter? pagination = null) => await _client.Get<List<Device>>("Devices", null, pagination);

    public async Task<ConsumerApiResponse<List<Device>>> ListDevices(List<string> identities, PaginationFilter? pagination = null) => await _client
        .Request<List<Device>>(HttpMethod.Get, "Devices")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", identities)
        .Execute();

    public async Task<ConsumerApiResponse<RegisterDeviceResponse>> RegisterDevice(RegisterDeviceRequest request) => await _client.Post<RegisterDeviceResponse>("Devices", request);

    public async Task<ConsumerApiResponse<Device>> GetActiveDevice() => await _client.Get<Device>("Devices/Self");

    public async Task<ConsumerApiResponse<EmptyResponse>> ChangePassword(ChangePasswordRequest request) => await _client.Put<EmptyResponse>("Devices/Self/Password", request);

    public async Task<ConsumerApiResponse<EmptyResponse>> DeleteDevice(string id) => await _client.Delete<EmptyResponse>($"Devices/{id}");
}
