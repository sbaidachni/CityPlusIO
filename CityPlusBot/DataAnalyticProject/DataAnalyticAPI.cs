using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyticProject
{
    public static class DataAnalyticAPI
    {

        static DataAnalyticAPI()
        {
            db = new cityplusdbEntities();
        }

        static cityplusdbEntities db;
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
    }

    public enum contentType { image, video};
}
