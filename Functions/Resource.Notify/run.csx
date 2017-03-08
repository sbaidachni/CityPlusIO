#r "Newtonsoft.Json"

using System;

public static void Run(string msg, TraceWriter log)
{
    var resource = JsonConvert.DeserializeObject<Resource>(msg);

    var users = new User[];
    foreach (var user in users)
    {
        //Notification
    }
}

public class User
{
    public string Channel;
}

public class Resource
{
    public string description;
    public string category;
    public int quantity;
    public decimal latitude;
    public decimal latitude;
}