#load "..\SharedCode\HelloRequest.csx"
#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;a
using Newtonsoft.Json.Linq;

public static object Run(HttpRequestMessage req, TraceWriter log,
                                     out string outputBlob)
{
    outputBlob = string.Empty;
    string jsonContent = string.Empty;
    JToken activityLog = null;

    log.Info($"Webhook was triggered!");

    Task.Run(async () => {
        jsonContent = await req.Content.ReadAsStringAsync();
        activityLog = JObject.Parse(jsonContent.ToString())
            .SelectToken("data.context.activityLog");

        if (activityLog == null ||    
            !string.Equals((string)activityLog["resourceType"], 
            "Microsoft.Resources/subscriptions/resourceGroups"))
        {
            log.Error("An error occurred");
            activityLog = null;
        }
    });

    Thread.Sleep(500);

    if (activityLog == null)
        return req.CreateResponse(HttpStatusCode.BadRequest, new
            {
                error = "Unexpected message payload or wrong alert received."
            });
    else {
        var helloRequest = new HelloRequest
        {
            Number = (string)activityLog["resourceGroupName"],
            Message = string.Format("Resource group '{0}' was {1} on {2}.",
            (string)activityLog["resourceGroupName"],
            ((string)activityLog["subStatus"]).ToLower(), 
            (DateTime)activityLog["submissionTimestamp"])
        };
        
        outputBlob = JsonConvert.SerializeObject(helloRequest);
    }

    return req.CreateResponse(HttpStatusCode.OK);    
}