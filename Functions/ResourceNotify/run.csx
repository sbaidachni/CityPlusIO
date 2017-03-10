#load "entities.csx"

#r "Newtonsoft.Json"

using System;
using System.Collections.Generic;
using Newtonsoft;
using King.Mapper;
using King.Mapper.Data;

public static async Task Run(string msg, ICollector<string> output, TraceWriter log)
{
    var data = JsonConvert.DeserializeObject<Resource>(msg);

    var connectionString = Env("cityplus_SQLDatabase");
    var select = $"SELECT [channelId] FROM person WHERE Location.geography::Point({data.latitude}, {data.longitude}, 4326) <= 100";

    using (var connection = new SqlConnection(connectionString))
    {
        var reader = await executor.DataReader(insert);

        var users = reader.Models<User>();
        foreach (var user in users)
        {
            output.Add(JsonConvert.Serialize(new Notification() {
                channelId = user.channelId
                , resourceDescription = data.description
                , resourceLatitude = data.latitude
                , resourceLongitude = data.longitude
            }));
        }
    }
}
private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);