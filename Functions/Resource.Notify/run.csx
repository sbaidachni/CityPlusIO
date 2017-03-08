using System;

public static void Run(string resource, TraceWriter log)
{
    log.Info($"C# Queue trigger function processed: {resource}");
}

public class Resource
{
    public string description;
    public string category;
    public int quantity;
    public decimal latitude;
    public decimal latitude;
}