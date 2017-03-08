#r "Newtonsoft.Json"

using System;
using System.Data.SqlClient;

public static async Task Run(HttpRequestMessage req, out string message, TraceWriter log)
{
    // Get request body
    var data = await req.Content.ReadAsAsync<Resource>();

    //Insert into database
    var insert = $"INSERT INTO resource ([description, quantity, category, location]) VALUES ('{data.description}', {data.quantity}, '{data.category}', geography::Point({data.latitude}, {data.longitude}, 4326)')";

    var connectionString = Env("");
    using (var connection = new SqlConnection(connectionString))
    {
        var cmd = new SqlCommand(insert, myConnection);
        await cmd.ExecuteNonQueryAsync();
    }

    message = JsonConvert.SerializeObject(data);
}

public class Resource
{
    public string description;
    public string category;
    public int quantity;
    public decimal latitude;
    public decimal latitude;
}

private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);