#r "Microsoft.WindowsAzure.Storage"
#r "System.Configuration"

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer triggered function executed at: {DateTime.Now}");
    
    log.Info($"Timer schedule: {myTimer.Schedule}");
    log.Info($"Timer last execution: {myTimer.ScheduleStatus.Last}");
    log.Info($"Timer last execution: {myTimer.ScheduleStatus.Next}");

    string conn = 
      ConfigurationManager.AppSettings["functionappsuccb139_STORAGE"];
    
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conn);
    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
    CloudBlobContainer container = 
      blobClient.GetContainerReference("receipts");
 
    DateTime oldestTime = 
      DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
    
    log.Info($"Checking for old receipts");

    foreach(CloudBlockBlob blob in 
            container.ListBlobs().OfType<CloudBlockBlob>())
    {
        var isOld = blob.Properties.LastModified < oldestTime;
        if (isOld)
        {
            log.Info($"Blob deleted: {blob.Name}");
            blob.Delete();
        }
    }
}