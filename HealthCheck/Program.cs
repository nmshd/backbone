var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.ConnectionClose = true;

var uriArgument = args[1];

var isValidUri = args.Length == 2 && Uri.IsWellFormedUriString(uriArgument, UriKind.Absolute);
if (!isValidUri)
{
    throw new ArgumentException("A valid URI must be given as first argument");
}

var uri = new Uri(uriArgument);

var response = await httpClient.GetAsync(uri);
return response.IsSuccessStatusCode ? 0 : 1;
