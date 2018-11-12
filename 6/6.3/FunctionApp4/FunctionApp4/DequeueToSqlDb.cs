using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionApp4
{
    public static class DequeueToSqlDb
    {
        [FunctionName("DequeueToSqlDb")]
        public static async Task Run([QueueTrigger("pmi-az-af-queue", Connection = "AzureWebJobsStorage")] string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            var str = Environment.GetEnvironmentVariable("sqldb-connection");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "insert into dbo.AzureFunctionOutput (functionOutput) select '" + myQueueItem + "'";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.Info($"{rows} rows were updated");
                }
            }
        }
    }
}
