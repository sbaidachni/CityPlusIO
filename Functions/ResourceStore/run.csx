#load "entities.csx"

#r "Newtonsoft.Json"

using System;
using King.Mapper;
using King.Mapper.Data;

public static async Task Run(HttpRequestMessage req, out string message, TraceWriter log)
{
    log.Info($"I just want to see Log Log: {message}");

    var data = await req.Content.ReadAsAsync<Resource>();

    var connectionString = Env("cityplus_SQLDatabase");
    var insert = $"INSERT INTO resources ([description], [quantity], [category], [latlong]) VALUES ('{data.description}', {data.quantity}, '{data.category}', geography::Point({data.latitude}, {data.longitude}, 4326))";
    
    log.Info($"I just want to see Log Log: {insert}");
    
    using (var connection = new SqlConnection(connectionString))
    {
         var executor = new Executor(connection);
         await executor.NonQuery(insert);
    }

     message = JsonConvert.SerializeObject(data);
}

private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);