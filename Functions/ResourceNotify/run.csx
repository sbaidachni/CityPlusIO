#r "Newtonsoft.Json"

using System;
using King.Mapper;
using King.Mapper.Data;

public static void Run(string msg, TraceWriter log)
{
    var data = JsonConvert.DeserializeObject<Resource>(msg);

    var connectionString = Env("cityplus_SQLDatabase");
    var select = $"SELECT [channelId] FROM person WHERE location.geography::Point({data.latitude}, {data.longitude}, 4326) <= 100";

    using (var connection = new SqlConnection(connectionString))
    {
        var dataReader = await executor.DataReader(insert);

        var users = reader.Models<User>();
        foreach (var user in users)
        {
            //Send Notifications to uses
        }
    }
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
    public decimal latitude;
}