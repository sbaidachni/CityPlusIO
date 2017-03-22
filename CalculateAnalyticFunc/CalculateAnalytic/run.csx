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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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

        var dataD = JsonConvert.DeserializeObject<Data>(result);

        SqlConnection conn2 =new SqlConnection(ConnString);
        SqlCommand commUpdate=new SqlCommand("UPDATE Attachments SET isAdultContent=@par2, isRacyContent=@par3, adultScore=@par4, racyScore=@par5 WHERE AttachmentId=@par1", conn2);

        commUpdate.Parameters.Add("par1", reader["AttachmentId"].ToString());

        commUpdate.Parameters.Add("par2", Convert.ToInt32(dataD.adult.isAdultContent));
        commUpdate.Parameters.Add("par3", Convert.ToInt32(dataD.adult.isRacyContent));
        commUpdate.Parameters.Add("par4", dataD.adult.adultScore);
        commUpdate.Parameters.Add("par5", dataD.adult.racyScore);

        log.Info("parameters provided");


        conn2.Open();
        log.Info("update Attachments table");
        commUpdate.ExecuteNonQuery();
        log.Info("Attachment is updated");
        conn2.Close();


if (dataD.tags!=null)
{
        foreach (var tag in dataD.tags)
        {
            SqlConnection conn3 = new SqlConnection(ConnString);
            SqlCommand commInsert = new SqlCommand("INSERT INTO AttachmentTags (AttachmentId, name, confidence) VALUES (@par1, @par2, @par3)", conn3);
            commInsert.Parameters.Add("par1", reader["AttachmentId"].ToString());
            commInsert.Parameters.Add("par2", tag.name);
            commInsert.Parameters.Add("par3", tag.confidence);

            conn3.Open();
            log.Info("update AttachmentTags table");
            commInsert.ExecuteNonQuery();
            log.Info("AttachmentTag is updated");
            conn3.Close();
        }
}

        if (dataD.faces!=null)
        {
            log.Info("start emotion API");
            var emotionResult=await GetEmotionData(reader["ContentUrl"].ToString(), log);
            log.Info("get results from emotion");

            var dataE = JsonConvert.DeserializeObject<FaceData[]>(emotionResult);
            foreach(var face in dataE)
            {
                SqlConnection conn3 = new SqlConnection(ConnString);
                SqlCommand commInsert = new SqlCommand("INSERT INTO Faces (AttachmentId, heightSize, leftSize, topSize,widthSize, anger, contempt, disgust, fear, happiness, neutral, sadness, surprise) VALUES (@par1, @par2, @par3, @par4, @par5, @par6, @par7, @par8, @par9, @par10, @par11, @par12, @par13)", conn3);
                commInsert.Parameters.Add("par1", reader["AttachmentId"].ToString());
                commInsert.Parameters.Add("par2", face.faceRectangle.height);
                commInsert.Parameters.Add("par3", face.faceRectangle.left);
                commInsert.Parameters.Add("par4", face.faceRectangle.top);
                commInsert.Parameters.Add("par5", face.faceRectangle.width);
                commInsert.Parameters.Add("par6", face.scores.anger);
                commInsert.Parameters.Add("par7", face.scores.contempt);
                commInsert.Parameters.Add("par8", face.scores.disgust);
                commInsert.Parameters.Add("par9", face.scores.fear);
                commInsert.Parameters.Add("par10", face.scores.happiness);
                commInsert.Parameters.Add("par11", face.scores.neutral);
                commInsert.Parameters.Add("par12", face.scores.sadness);
                commInsert.Parameters.Add("par13", face.scores.surprise);

                conn3.Open();
                log.Info("update Faces table");
                commInsert.ExecuteNonQuery();
                log.Info("Faces is updated");
                conn3.Close();
            }
        }
    }
    conn.Close();
    log.Info("Analytics is done");
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

private static async Task<string> GetVisionData(string imageUri, TraceWriter log)
{
        log.Info("test");
        var client = new HttpClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);

        // Request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", System.Environment.GetEnvironmentVariable("visionAPI", EnvironmentVariableTarget.Process));

        // Request parameters
        queryString["visualFeatures"] = "Adult, Faces, Tags";
        var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze?" + queryString;

        HttpResponseMessage response;
        var container = new King.Azure.Data.Container("images",  System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process));
        log.Info("test");
        var image = container.Get(imageUri).Result;

        log.Info("test");
        using (var content = new ByteArrayContent(image))
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(uri, content);
            log.Info("test");
            
        }
        string s=await response.Content.ReadAsStringAsync();
        
        log.Info(s);
        return s;
}

private static async Task<string> GetEmotionData(string imageUri, TraceWriter log)
{
        var client = new HttpClient();

        // Request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", System.Environment.GetEnvironmentVariable("emotionAPI", EnvironmentVariableTarget.Process));

        // Request parameters
        var uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";

        HttpResponseMessage response;
        var container = new King.Azure.Data.Container("images",  System.Environment.GetEnvironmentVariable("cityplusstorage_STORAGE", EnvironmentVariableTarget.Process));
        var image = container.Get(imageUri).Result;

        using (var content = new ByteArrayContent(image))
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(uri, content);
        }
        string s=await response.Content.ReadAsStringAsync();
        
        log.Info(s);
        return s;
}

   class Data
    {
        public Adult adult {get; set;}

        public List<Tag> tags { get; set; }

        public List<Face> faces { get; set; }
}

    public class Adult
    {
        public bool isAdultContent { get; set; }

        public bool isRacyContent { get; set; }

        public double adultScore { get; set; }

        public double racyScore { get; set; }


    }

    public class Face
    {
        public int age { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }

        public double confidence { get; set; }
    }

    class FaceData
    {
        public FaceRectangle faceRectangle { get; set; }

        public Scores scores { get; set; }
    }

    public class FaceRectangle
    {
        public int height { get; set; }

        public int left { get; set; }

        public int top { get; set; }

        public int width { get; set; }
    }

    public class Scores
    {
        public double anger { get; set; }

        public double contempt { get; set; }

        public double disgust { get; set; }

        public double fear { get; set; }

        public double happiness { get; set; }

        public double neutral { get; set; }

        public double sadness { get; set; }

        public double surprise { get; set; }
    }