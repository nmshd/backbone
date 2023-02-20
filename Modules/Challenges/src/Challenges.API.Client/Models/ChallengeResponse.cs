using System.Net;

namespace Challenges.API.Client.Models;
public class ChallengeResponse
{
    public ChallengeResult Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ReasonPhrase { get; set; }
    public bool IsSuccessStatusCode { get; set; }
}
