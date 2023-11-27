using System.Collections.Generic;

namespace LethalAdmin.Logging;

public static class LethalLogger
{
    private const int MaxLogCount = 50;
    private static readonly List<Log> Logs = new();

    public static void AddLog(Log log)
    {
        Logs.Add(log);
        
        while (Logs.Count > MaxLogCount) // Should only run once but just in case a stroke happens
        {
            Logs.RemoveAt(0);
        }
    }

    public static IEnumerable<Log> GetLogs()
    {
        return Logs.ToArray();
    }
}