#load "..\SharedCode\CreateHelloRequest.csx"

using System;

public static void Run(CreateHelloRequest input, TraceWriter log, out CreateHelloRequest outputQueueItem)
{
    log.Info($"C# manually triggered function called with input: {input}");
    outputQueueItem = input;
}