using System;

public static void Run(string message, TraceWriter log)
{
    log.Info($"C# Queue trigger function processed: {message}");
}