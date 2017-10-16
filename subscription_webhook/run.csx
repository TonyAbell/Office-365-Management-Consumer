#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static async Task<object> Run(HttpRequestMessage req, IAsyncCollector<object> metadata, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    JArray array = JArray.Parse(jsonContent);

    foreach (JObject item in array)
    {
        item.Add("id", item["contentId"]);
        await metadata.AddAsync(item);

    }
    log.Info($"Done!");


    return req.CreateResponse(HttpStatusCode.OK);
}
