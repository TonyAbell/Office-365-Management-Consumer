#r "NewtonSoft.Json"
using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using Newtonsoft.Json.Linq;

public async static Task Run(string input, TraceWriter log)
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
    var accessToken = authenticationResult.AccessToken;


    var startSubscriptionsUrl = $"https://manage.office.com/api/v1.0/{Tenantid}/activity/feed/subscriptions/start?contentType=Audit.General&PublisherIdentifier={PublisherIdentifier}";
    var client = new WebClient();
    client.Headers.Add("Authorization", $"Bearer {authenticationResult.AccessToken}");
    client.Headers.Add("Content-Type", "application/json; utf-8");
    var body = JObject.FromObject(new { webhook = new { address = "https://requestb.in/tn8u4gtn" } });
    var bodyStr = body.ToString(Newtonsoft.Json.Formatting.Indented);

    try
    {
        var result = client.UploadData(startSubscriptionsUrl, "POST", System.Text.ASCIIEncoding.UTF8.GetBytes(bodyStr));

    }
    catch (WebException ex)
    {
        var response = new System.IO.StreamReader(ex.Response.GetResponseStream());
        Console.WriteLine(response.ReadToEnd());
    }

}