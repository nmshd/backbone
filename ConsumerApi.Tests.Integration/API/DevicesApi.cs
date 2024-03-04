using Backbone.ConsumerApi.Tests.Integration.Models;

namespace Backbone.ConsumerApi.Tests.Integration.API;

internal class DevicesApi : BaseApi
{
    public DevicesApi(HttpClientFactory factory) : base(factory) { }

    public async Task<HttpResponse<ListDevicesResponse>> ListDevices(RequestConfiguration requestConfiguration)
    {
        return await Get<ListDevicesResponse>("/Devices", requestConfiguration);
    }

    public async Task<HttpResponse<RegisterDeviceResponse>> RegisterDevice(RequestConfiguration requestConfiguration)
    {
        return await Post<RegisterDeviceResponse>("/Devices", requestConfiguration);
    }

    internal async Task<HttpResponse> DeleteDevice(RequestConfiguration requestConfiguration, string id)
    {
        return await Delete($"/Devices/{id}", requestConfiguration);
    }
}
