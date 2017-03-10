using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace DataAnalyticProject
{
    public static class DataAnalyticAPI
    {

        static DataAnalyticAPI()
        {
            db = new cityplusdbEntities();
        }

        public static cityplusdbEntities db;
        public static int AddSession(string channelid)
        {
            Session ses = new Session();
            ses.ChannelId = channelid;
            ses.UtcDateTime = DateTime.UtcNow;
            db.Sessions.Add(ses);
            db.SaveChanges();
            return ses.SessionId;
        }

        public static int AddConversation(int sessionId, string convText, string address=null, DbGeography geo=null)
        {
            Conversation conv = new Conversation();
            conv.SessionId = sessionId;
            conv.Text = convText;
            conv.UtcDateTime = DateTime.UtcNow;
            if (address!=null)
            {
                conv.Address = address;
            }

            if (geo != null)
                conv.GeoCoordinates = geo;

            db.Conversations.Add(conv);
            db.SaveChanges();
            return conv.ConversationId;
        }

        public static void  AddAttachment(int conversationId, contentType type, string uri)
        {
            Attachment att = new Attachment();
            att.ConversationId = conversationId;
            att.ContentType = type.ToString();
            att.ContentUrl = uri;
            db.Attachments.Add(att);
            db.SaveChanges();
        }

        public static void SubmitQueryForAnalysis(int conversationId)
        {
            CloudStorageAccount account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("cityplusstorage", "cQ4Q29tmbsLR+11W91Tt6IXvu5nPBiBHWkxKTjfHDuBWH8aT9MdWf1a0PjIQj2n6B7arPJa2cMh5TOnmIHC8Fw=="), true);
            var clientQueue=account.CreateCloudQueueClient();
            var queue=clientQueue.GetQueueReference("analyticqueue");

            queue.CreateIfNotExists();

            queue.AddMessage(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(conversationId.ToString()));

        }
    }

    public enum contentType { image, video};
}
