using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Data.SqlClient;

namespace DatabaseAPI
{
    public static class UpdateToken
    {
        private static readonly string SelectQuery = "SELECT * FROM Devices Where DeviceId = @deviceId";
        private static readonly string UpdateQuery = "UPDATE Devices SET Token = @token Where DeviceId = @deviceId";
        private static readonly string InsertQuery = "Insert Into Devices VALUES (@deviceId, @token)";

        [FunctionName("UpdateToken")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string deviceId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "deviceId", true) == 0)
                .Value;

            string token = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "token", true) == 0)
                .Value;

            return ExecuteQuery(deviceId, token)
                ? req.CreateResponse(HttpStatusCode.OK)
                : req.CreateResponse(HttpStatusCode.BadRequest, "Failed to update token");
        }

        private static bool ExecuteQuery(string deviceId, string token)
        {
            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(token))
                return false;

            var cb = new SqlConnectionStringBuilder
            {
                DataSource = "babysafe.database.windows.net",
                UserID = "Babysafe",
                Password = "Ahmad123",
                InitialCatalog = "BabysafeDB"
            };

            try
            {
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand(SelectQuery, connection);
                    cmd.Parameters.AddWithValue("deviceId", deviceId);

                    var exists = false;
                    using (var reader = cmd.ExecuteReader())
                        exists = reader.HasRows;

                    if (exists)
                    {
                        cmd = new SqlCommand(UpdateQuery, connection);
                        cmd.Parameters.AddWithValue("deviceId", deviceId);
                        cmd.Parameters.AddWithValue("token", token);

                        cmd.ExecuteReader();
                    }
                    else
                    {
                        cmd = new SqlCommand(InsertQuery, connection);
                        cmd.Parameters.AddWithValue("deviceId", deviceId);
                        cmd.Parameters.AddWithValue("token", token);

                        cmd.ExecuteReader();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
