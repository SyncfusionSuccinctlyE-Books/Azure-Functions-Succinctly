#load "..\SharedCode\HelloRequest.csx"
#load "..\SharedCode\MsgSentConfirmation.csx"

#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using Newtonsoft.Json; 
using Microsoft.WindowsAzure.Storage.Blob;

public static void Run(CloudBlockBlob myBlob, string name, 
                       TraceWriter log, Stream outputBlob)
{
    log.Info($"Metadata Name: {myBlob.Name}");
    log.Info($"Metadata StorageUri: {myBlob.StorageUri}");
    log.Info($"Metadata Container: {myBlob.Container.Name}");

    HelloRequest helloRequest = GetHelloRequest(myBlob);
    log.Info($"Hello Request: {helloRequest}");

    string id = SendMessage(helloRequest);

    var confirm = new MsgSentConfirmation
    {
        ReceiptId = id,
        Number = helloRequest.Number,
        Message = helloRequest.Message
    };

    UploadMsg(confirm, outputBlob);
}

public static void UploadMsg(MsgSentConfirmation confirm, 
                             Stream outputBlob)
{
    using (var w = new StreamWriter(outputBlob))
    {
        using (var jw = new JsonTextWriter(w))
        {
            JsonSerializer s = new JsonSerializer();
            s.Serialize(jw, confirm);
            jw.Flush();
        }
    }
}

public static string SendMessage(HelloRequest req)
{
    // We simulate sending SMS with req and returning a unique GUID
    return Guid.NewGuid().ToString(); 
}

public static HelloRequest GetHelloRequest(CloudBlockBlob blob)
{
    HelloRequest helloRequest;
    
    using (var ms = new MemoryStream())
    {
        blob.DownloadToStream(ms);
        ms.Position = 0;

        using (var res = new StreamReader(ms)) 
        {
            using (var jtr = new JsonTextReader(res))
            {
                var s = new JsonSerializer();
                helloRequest = s.Deserialize<HelloRequest>(jtr);
            } 
        }
    }

    return helloRequest;   
}