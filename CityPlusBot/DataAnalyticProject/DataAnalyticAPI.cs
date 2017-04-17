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

        public static void AddResources(int sessionId,List<Resource> resources)
        {
            try
            {
                Conversation conv = new Conversation();
                conv.SessionId = sessionId;
                conv.Text = "resources provided";
                conv.UtcDateTime = DateTime.UtcNow;

                foreach (var r in resources)
                {
                    ResourcesProvided res = new ResourcesProvided();
                    res.ResourceId = r.Id;
                    conv.ResourcesProvideds.Add(res);
                }
                db.Conversations.Add(conv);
                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
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

        public static void AddUser(string accountId, string accountName, string channel)
        {
            Person p = new Person();
            p.AccountID = accountId;
            p.Channel = channel;
            p.AccountName = accountName;
            db.People.Add(p);
            db.SaveChanges();
        }

        public static void RemoveUser(string accountId, string accountName)
        {
            var person = (from a in db.People where a.AccountID == accountId && a.AccountName == accountName select a).FirstOrDefault();
            if (person != null)
            {
                db.People.Remove(person);
                db.SaveChanges();
            }
        }
    }

    public enum contentType { image, video};
}
