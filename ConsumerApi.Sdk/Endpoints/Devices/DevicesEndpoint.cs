using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices;

public class DevicesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<ListDevicesResponse>> ListDevices(PaginationFilter? pagination = null)
        => await _client.Get<ListDevicesResponse>($"api/{API_VERSION}/Devices", null, pagination);

    public async Task<ApiResponse<ListDevicesResponse>> ListDevices(IEnumerable<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<ListDevicesResponse>(HttpMethod.Get, $"api/{API_VERSION}/Devices")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ApiResponse<RegisterDeviceResponse>> RegisterDevice(RegisterDeviceRequest request)
        => await _client.Post<RegisterDeviceResponse>($"api/{API_VERSION}/Devices", request);

    public async Task<ApiResponse<Device>> GetActiveDevice() => await _client.Get<Device>($"api/{API_VERSION}/Devices/Self");

    public async Task<ApiResponse<EmptyResponse>> ChangePassword(ChangePasswordRequest request)
        => await _client.Put<EmptyResponse>($"api/{API_VERSION}/Devices/Self/Password", request);

    public async Task<ApiResponse<EmptyResponse>> DeleteDevice(string id) => await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Devices/{id}");
}
