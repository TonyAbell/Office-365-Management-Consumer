#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static async Task<object> Run(HttpRequestMessage req, IAsyncCollector<object> metadata, TraceWriter log)
{

    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    log.Info(jsonContent);
    var token = JToken.Parse(jsonContent);

    if (token is JArray)
    {
        JArray array = JArray.Parse(jsonContent);
        foreach (JObject item in array)
        {
            item.Add("id", item["contentId"]);
            await metadata.AddAsync(item);
        }
    }
    else if (token is JObject)
    {
        log.Info($"Validation Request !");
    }
    log.Info($"Done!");


    return req.CreateResponse(HttpStatusCode.OK);
}
