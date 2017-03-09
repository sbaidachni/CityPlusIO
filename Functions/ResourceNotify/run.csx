#r "Newtonsoft.Json"

using System;
using System.Collections.Generic;
using Newtonsoft;
using King.Mapper;
using King.Mapper.Data;

public static async Task<IList<Notification>> Run(string msg, TraceWriter log)
{
    var data = JsonConvert.DeserializeObject<Resource>(msg);

    var connectionString = Env("cityplus_SQLDatabase");
    var select = $"SELECT [channelId] FROM person WHERE location.geography::Point({data.latitude}, {data.longitude}, 4326) <= 100";

    var notifications = new List<Notification>();

    using (var connection = new SqlConnection(connectionString))
    {
        var reader = await executor.DataReader(insert);

        var users = reader.Models<User>();
        foreach (var user in users)
        {
            notifications.Add(new Notification() { channelId = user.channelId });
        }
    }

    return notifications;
}

public class Notification
{
    public string channelId;
}

public class User
{
    public string channelId;
}

public class Resource
{
    public string description;
    public string category;
    public int quantity;
    public decimal latitude;
    public decimal longitude;
}