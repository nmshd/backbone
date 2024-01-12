using Backbone.AdminUi.Tests.Integration.Configuration;
using Backbone.AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminUi.Tests.Integration.API;
internal class MessagesApi : BaseApi
{
    public MessagesApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    internal async Task<HttpResponse<List<MessageOverviewDTO>>> GetMessagesWithParticipant(string participant, string type, RequestConfiguration requestConfiguration)
    {
        return await Get<List<MessageOverviewDTO>>($"/Messages?&participant={participant}&type={type}", requestConfiguration);
    }
}
