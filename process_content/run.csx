#r "Microsoft.Azure.Documents.Client"
#r "NewtonSoft.Json"
using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using Newtonsoft.Json.Linq;

public async static Task Run(IReadOnlyList<Document> input,  IAsyncCollector<object> data,TraceWriter log)
{
    if (input != null && input.Count > 0)
    {
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
      
        foreach (dynamic item in input)
        {
            var contentUri = item.contentUri;
            var contentStr = client.DownloadString(contentUri);
            var contentJson = JArray.Parse(contentStr);
            foreach (JObject contentItem in contentJson)
            {
                contentItem.Add("id", contentItem["Id"]);
                await data.AddAsync(contentItem);
                //var str = contentItem.ToString(Newtonsoft.Json.Formatting.Indented);
                //log.Info($"{str}");
            }
            //log.Info($"contentUrl: {contentUri}");
        }
    }
    log.Info("Done!");
}
