#r "System.Data"
#r "Microsoft.Rest.ClientRuntime.dll"
#r "System.Web"
#r "Newtonsoft.Json.dll"
#r "Microsoft.ProjectOxford.Vision.dll"
#r "System.IO"

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
using System.IO;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

public static async void Run(string myQueueItem, TraceWriter log)
{
    string ConnString = System.Environment.GetEnvironmentVariable("ConnString", EnvironmentVariableTarget.Process);
    string cityplusstorage_STORAGE = System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process);

    log.Info($"C# Queue trigger function processed: {myQueueItem}");

    int queryID=int.Parse(myQueueItem);

    //TextAnalitycs
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
            var sentiment=await GetAnalyticsData(reader["Text"].ToString(), log);
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
    log.Info("Text Analytics is finished");

    //Vision API (Tags)
    comm=new SqlCommand("select * from Attachments where ConversationId=@par1 and CHARINDEX('image',ContentType)>0",conn);
    comm.Parameters.Add("par1",queryID);
    conn.Open();
    log.Info("Ready to execute command");
    reader=comm.ExecuteReader();
    while(reader.Read())
    {
        log.Info("get an attachment info");
        var result=await GetVisionData(reader["ContentUrl"].ToString(), log);
        log.Info("get results from vision");

        SqlConnection conn2 =new SqlConnection(ConnString);
        SqlCommand commUpdate=new SqlCommand("UPDATE Attachments SET isAdultContent=@par2, isRacyContent=@par3, adultScore=@par4, racyScore=@par5 WHERE AttachmentId=@par1", conn2);
        log.Info(reader["AttachmentId"].ToString());
        log.Info(Convert.ToInt32(result.Adult.IsAdultContent).ToString());
        log.Info(Convert.ToInt32(result.Adult.IsRacyContent).ToString());
        log.Info(result.Adult.AdultScore.ToString());
        log.Info(result.Adult.RacyScore.ToString());
        commUpdate.Parameters.Add("par1", reader["AttachmentId"].ToString());
        commUpdate.Parameters.Add("par2", Convert.ToInt32(result.Adult.IsAdultContent));
        commUpdate.Parameters.Add("par3", Convert.ToInt32(result.Adult.IsRacyContent));
        commUpdate.Parameters.Add("par4", result.Adult.AdultScore);
        commUpdate.Parameters.Add("par5", result.Adult.RacyScore);

        log.Info("parameters provided");


        conn2.Open();
        log.Info("update Attachments table");
        commUpdate.ExecuteNonQuery();
        log.Info("Attachment is updated");
        conn2.Close();


        foreach (var tag in result.Tags)
        {
            SqlConnection conn3 = new SqlConnection(ConnString);
            SqlCommand commInsert = new SqlCommand("INSERT INTO AttachmentTags (AttachmentId, name, confidence) VALUES (@par1, @par2, @par3)", conn2);
            commInsert.Parameters.Add("par1", reader["AttachmentId"].ToString());
            commInsert.Parameters.Add("par2", tag.Name);
            commInsert.Parameters.Add("par3", tag.Confidence);

            conn3.Open();
            log.Info("update AttachmentTags table");
            commInsert.ExecuteNonQuery();
            log.Info("AttachmentTag is updated");
            conn3.Close();
        }
    }
    conn.Close();
    log.Info("Vision API is done");

    //Emotion API


    var emotionKey=System.Environment.GetEnvironmentVariable("emotionAPI", EnvironmentVariableTarget.Process);
}

private static async Task<double> GetAnalyticsData(string text, TraceWriter log)
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

private static async Task<AnalysisResult> GetVisionData(string imageUri, TraceWriter log)
{
    AnalysisResult res=null;
    try
    {
        log.Info("start");
    var container = new King.Azure.Data.Container("images",  System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process));
    
    var image = container.Get(imageUri).Result;  

    log.Info("Image");

    using (var stream = new System.IO.MemoryStream(image))
    {
        log.Info("start call");
        IVisionServiceClient client = new VisionServiceClient(System.Environment.GetEnvironmentVariable("visionAPI", EnvironmentVariableTarget.Process));
        res=await client.AnalyzeImageAsync(stream,visualFeatures:null);
    }
    log.Info("image done");
    }
    catch(Exception ex)
    {
        log.Info(ex.Message);
    }

    return res;
}