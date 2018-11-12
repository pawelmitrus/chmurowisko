using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp4
{
    public static class HttpReturnSqlDbRows
    {
        [Disable]
        [FunctionName("HttpReturnSqlDbRows")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string date = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "date", true) == 0)
                .Value;

            if (date == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                date = data?.name;
            }
            else
            {
                var str = Environment.GetEnvironmentVariable("sqldb-connection");
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select functionOutput from dbo.AzureFunctionOutput where CAST(RIGHT([functionOutput], LEN('12/11/2018 07:08:35.002')) AS DATE) =  CAST('" + date + "' AS DATE)";

                    using (SqlCommand cmd = new SqlCommand(text, conn))
                    {

                        try
                        {
                            SqlDataReader reader = cmd.ExecuteReader();

                            // In this part below, I want the SqlDataReader  to 
                            // read all of the records from database returned, 
                            // and I want the result to be returned as Array or 
                            // Json type, but I don't know how to write this part.
                            List<string> result = new List<string>();

                            while (reader.Read())
                            {
                                result.Add(reader[0].ToString());
                            }
                            reader.Close();
                            string json = JsonConvert.SerializeObject(result, Formatting.None);
                            
                            //var jsonSerialiser = new JavaScriptSerializer();
                            //var json = jsonSerialiser.Serialize(result);

                            return req.CreateResponse(HttpStatusCode.OK, json);
                        }
                        catch (Exception ex)
                        {
                            return req.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                        }

                        // Execute the command and log the # rows affected.
                        //var rows = await cmd.ExecuteNonQueryAsync();
                        //log.Info($"{rows} rows were returned");
                    }
                }
            }
            return date == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + date);
        }
    }
}
