using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp4
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, TraceWriter log,
            [Queue("pmi-az-af-queue", Connection = "AzureWebJobsStorage")] ICollector<string> queueCollector)
        {
            DateTime now = DateTime.Now;

            log.Info($"C# Timer trigger function executed at: {now}");

            queueCollector.Add("Time the function was triggered: " + now.ToString("dd/MM/yyyy HH:mm:ss.fff"));
        }
    }
}
