#r "Newtonsoft.Json"

using System;
using King.Mapper;
using King.Mapper.Data;

public static async Task Run(HttpRequestMessage req, out string message, TraceWriter log)
{
    // Get request body
    var data = await req.Content.ReadAsAsync<Resource>();

    var connectionString = Env("cityplus_SQLDatabase");
    var insert = $"INSERT INTO resources ([description], [quantity], [category], [location]) VALUES ('{data.description}', {data.quantity}, '{data.category}', geography::Point({data.latitude}, {data.longitude}, 4326))";
    using (var connection = new SqlConnection(connectionString))
    {
        var executor = new Executor(connection);
        await executor.NonQuery(insert);
    }

    message = JsonConvert.SerializeObject(data);
}

public class Resource
{
    public string description;
    public string category;
    public int quantity;
    public decimal latitude;
    public decimal longitude;
}

private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);