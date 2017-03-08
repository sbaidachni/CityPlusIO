using System;

public static async Task Run(HttpRequestMessage req, TraceWriter log)
{
    // Get request body
    Resource data = await req.Content.ReadAsAsync<Resource>();

    //Insert into database

    return data;
}

public class Resource
{
    public string description;
    public string category;
    public int quantity;
    public decimal latitude;
    public decimal latitude;
}