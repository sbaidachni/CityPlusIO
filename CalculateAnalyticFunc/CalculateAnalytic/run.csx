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
using System.Text;

public static async void Run(string myQueueItem, TraceWriter log)
{
    string ConnString = System.Environment.GetEnvironmentVariable("ConnString", EnvironmentVariableTarget.Process);
    string cityplusstorage_STORAGE = System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process);

    log.Info($"C# Queue trigger function processed: {myQueueItem}");

    int queryID=int.Parse(myQueueItem);
    SqlConnection conn=new SqlConnection(ConnString);
    SqlCommand comm=new SqlCommand("select * from Conversations where ConversationId=@par1",conn);

    comm.Parameters.Add(new SqlParameter("par1",queryID));
    log.Info("Ready to open connection");
    conn.Open();
    log.Info("Ready to execute a reader");
    var reader=comm.ExecuteReader();
    log.Info("Before read");
    while(reader.Read())
    {
        log.Info("Read data");
        if (reader["Text"].ToString().Length>0)
        {
            log.Info($"Ready for TextAnalitycs API: {reader["Text"].ToString()}");
            var sentiment=await UpdateAnalyticsData(reader["Text"].ToString(), log);
            log.Info($"Service returned: {sentiment}");
            SqlConnection conn2=new SqlConnection(ConnString);
            SqlCommand commUpdate=new SqlCommand("Update Conversations Set sentiment=@par1 where ConversationId=@par2",conn2);
            commUpdate.Parameters.Add("par1",sentiment);
            commUpdate.Parameters.Add("par2",queryID);
            log.Info("ready to update DB");
            conn2.Open();
            commUpdate.ExecuteNonQuery();
            conn2.Close();
            log.Info("Db is updated");
        }
    }
    conn.Close();
}

private static async Task<double> UpdateAnalyticsData(string text, TraceWriter log)
{
    var client = new HttpClient();
    var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", System.Environment.GetEnvironmentVariable("textAnalytics", EnvironmentVariableTarget.Process));

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

        string s=await response.Content.ReadAsStringAsync();
        log.Info(s);

        s=s.Substring(s.IndexOf("\"score\":")+8);
        log.Info(s);

        s=s.Substring(0,s.IndexOf(','));
        log.Info(s);

        ret=double.Parse(s);
    }

    return ret;
}