#load "..\SharedCode\CreateHelloRequest.csx"
#load "..\SharedCode\HelloRequest.csx"

#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;

public static void Run (CreateHelloRequest myQueueItem, TraceWriter log, 
                        out string outputBlob,
                        DateTimeOffset insertionTime,
                        string id)
{
    log.Info(
        $"C# Queue trigger function processed: {myQueueItem}");

    log.Info($"InsertionTime: {insertionTime}");
    log.Info($"Id: {id}");

    var helloRequest = new HelloRequest
    {
        Number = myQueueItem.Number,
        Message = GenerateHello(myQueueItem.FirstName)
    };
    
    outputBlob = JsonConvert.SerializeObject(helloRequest);
}

private static string GenerateHello (string firstName)
{
    string hello;
    int hourOfTheDay = DateTime.Now.Hour;

    if (hourOfTheDay <= 12) 
        hello = "The Morning...";
    else if (hourOfTheDay <= 18) 
        hello = "The Afternoon...";
    else 
        hello = "The Evening...";

    return $"{hello} {firstName}";
}