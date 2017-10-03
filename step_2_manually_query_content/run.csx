#r "NewtonSoft.Json"
#r "Microsoft.Azure.Documents.Client"
using System;
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using Newtonsoft.Json.Linq;

public async static Task Run(string input, IAsyncCollector<object> metadata, TraceWriter log)
{
    //log.Info($"C# manually triggered function called with input: {input}");
    var ClientId  = ConfigurationManager.AppSettings["ClientId"];
    var ClientSecret  = ConfigurationManager.AppSettings["ClientSecret"];
    var Tenantid  = ConfigurationManager.AppSettings["Tenantid"];
    var PublisherIdentifier  = ConfigurationManager.AppSettings["PublisherIdentifier"];
    
    var AuthorityUrl = $"https://login.windows.net/{Tenantid}/oauth2/authorize";
    var ResourceUrl = "https://manage.office.com";

    var ClientCredential = new ClientCredential(ClientId, ClientSecret);

    var authenticationContext = new AuthenticationContext(AuthorityUrl);
    var authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl, ClientCredential);

    var GetContentUrl = $"https://manage.office.com/api/v1.0/{Tenantid}/activity/feed/subscriptions/content?contentType=Audit.General&&PublisherIdentifier={PublisherIdentifier}&startTime=2017-09-01";

    var client = new WebClient();
    client.Headers.Add("Authorization", $"Bearer {authenticationResult.AccessToken}");
    client.Headers.Add("Content-Type", "application/json; utf-8");

    string contentList = client.DownloadString(GetContentUrl);
    var jsonArray = JArray.Parse(contentList);

    foreach (JObject item in jsonArray)
    {
       
        item.Add("id", item["contentId"]);
        await metadata.AddAsync(item);

    }
    log.Info($"Done!");
}