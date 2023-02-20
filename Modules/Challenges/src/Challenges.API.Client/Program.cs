using System.Net;
using Challenges.API.Client.API;
using Challenges.API.Client.Models;

AuthenticationRequest userCredentials = null;

var credentials = new NetworkCredential("test", "test");
var handler = new HttpClientHandler { Credentials = credentials };
var challengesClient = new ChallengesClient(new HttpClient(handler));

await InitMenu();

async Task InitMenu()
{
    while (true)
    {
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("1. Create a new challenge");
        Console.WriteLine("2. Get an existing challenge");
        Console.WriteLine("3. Quit");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                var createWithAuth = AskForAuthentication();
                Console.WriteLine("Creating Challenge...");
                var challengePostResponse = createWithAuth
                    ? await challengesClient.CreateChallenge(GetUserCredentials())
                    : await challengesClient.CreateChallenge(null);

                DisplayResult(challengePostResponse);
                break;

            case "2":
                var getWithAuth = AskForAuthentication();
                Console.WriteLine("Enter the challenge ID:");
                var challengeId = Console.ReadLine();
                Console.WriteLine("Fetching Challenge...");
                var challengeGetResponse = getWithAuth
                    ? await challengesClient.GetChallengeById(challengeId, GetUserCredentials())
                    : await challengesClient.GetChallengeById(challengeId, null);

                DisplayResult(challengeGetResponse);
                break;

            case "3":
                return;

            default:
                Console.WriteLine("Invalid choice, please try again");
                break;
        }
    }
}

bool AskForAuthentication()
{
    Console.WriteLine("Do you want to authenticate?");
    Console.WriteLine("1. Yes");
    Console.WriteLine("2. No");

    var choice = Console.ReadLine();

    if (choice == "2")
    {
        userCredentials = null;
    }

    return choice == "1";
}

AuthenticationRequest GetUserCredentials()
{
    if (userCredentials == null)
    {
        userCredentials = new AuthenticationRequest();
        Console.WriteLine("Enter username:");
        userCredentials.Username = Console.ReadLine();
        Console.WriteLine("Enter password:");
        userCredentials.Password = Console.ReadLine();
    }
    return userCredentials;
}

static void DisplayResult(ChallengeResponse challengeResponse)
{
    if (!challengeResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Status Code: {(int)challengeResponse.StatusCode}");
        Console.WriteLine($"Reason: {challengeResponse.ReasonPhrase}");
    }
    else
    {
        Console.WriteLine($"Challenge id: {challengeResponse.Result.Id}");
        Console.WriteLine($"Challenge expiration date: {challengeResponse.Result.ExpiresAt}");
        Console.WriteLine($"Challenge created by: {challengeResponse.Result.CreatedBy ?? "N/A"}");
        Console.WriteLine($"Challenge created by device: {challengeResponse.Result.CreatedByDevice ?? "N/A"}");
    }
}