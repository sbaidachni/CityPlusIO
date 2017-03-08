#r "Newtonsoft.Json"

using System;

public static async Task Run(HttpRequestMessage req, out string message, TraceWriter log)
{
    // Get request body
    var data = await req.Content.ReadAsAsync<Resource>();

    //Insert into database

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