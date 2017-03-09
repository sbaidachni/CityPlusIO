#r "System.Data"
#r "Microsoft.Rest.ClientRuntime.dll"
#r "System.Web"

// 8.0.1 for net45
#r "Microsoft.WindowsAzure.Storage.dll"

// 2.0.18 for net45
#r "King.Azure.dll"

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System​.Net​.Http​.Headers;

public static async void Run(string myQueueItem, TraceWriter log)
{
    string ConnString = System.Environment.GetEnvironmentVariable("ConnString", EnvironmentVariableTarget.Process);
    string cityplusstorage_STORAGE = System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process);

    log.Info($"C# Queue trigger function processed: {myQueueItem}");

    int queryID=int.Parse(myQueueItem);
    SqlConnection conn=new SqlConnection(ConnString);
    SqlCommand comm=new SqlCommand("select * from Conversations where ConversationId=@par1",conn);

    comm.Parameters.Add(new SqlParameter("par1",queryID));
    conn.Open();
    var reader=comm.ExecuteReader();
    while(reader.Read())
    {
        if (reader["Text"].ToString().Length>0)
        {
            log.Info($"Ready for TextAnalitycs API: {reader["Text"].ToString()}");
            var sentiment=await UpdateAnalyticsData(reader["Text"].ToString());
            log.Info($"Service returned: {sentiment}");
        }
    }
    conn.Close();
}

private static async Task<double> UpdateAnalyticsData(string text)
{
    var client = new HttpClient();
    var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

    string subscriptionKey=client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", System.Environment.GetEnvironmentVariable("textAnalytics", EnvironmentVariableTarget.Process));

    var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + queryString;
    
    HttpResponseMessage response;
    string body = $"{{\"documents\": [{{\"language\": \"en\",\"id\": \"1\",\"text\": \"{ text}\"}}]}}";
    log.Info(body);
    byte[] byteData = Encoding.UTF8.GetBytes(body);

    double ret=0;

    using (var content = new ByteArrayContent(byteData))
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        response = await client.PostAsync(uri, content);
        log.Info(response.ToString());
    }

    return ret;
}