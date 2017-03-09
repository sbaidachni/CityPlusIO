#r "System.Data"
#r "Microsoft.Rest.ClientRuntime.dll"

// 8.0.1 for net45
#r "Microsoft.WindowsAzure.Storage.dll"

// 2.0.18 for net45
#r "King.Azure.dll"

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

private const string ConnString = System.Environment.GetEnvironmentVariable("ConnString", EnvironmentVariableTarget.Process);
private const string cityplusstorage_STORAGE = System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process);

public static async void Run(string myQueueItem, TraceWriter log)
{
    log.Info($"C# Queue trigger function processed: {myQueueItem}");
    log.Info(ConnString);
    log.Info(cityplusstorage_STORAGE);

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
            UpdateAnalyticsData().Wait();
        }
    }
    conn.Close();
}

private static async Task UpdateAnalyticsData()
{
    /*var client = new HttpClient();
    var queryString = HttpUtility.ParseQueryString(string.Empty);

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

    var uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?" + queryString;

    HttpResponseMessage response;

    byte[] byteData = Encoding.UTF8.GetBytes("{body}");

    using (var content = new ByteArrayContent(byteData))
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
        response = await client.PostAsync(uri, content);
    }*/
}