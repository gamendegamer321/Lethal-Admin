using System;
using UnityEngine;

namespace LethalAdmin.Logging;

public class Log
{
    internal readonly string Prefix;
    
    private readonly float _time = Time.realtimeSinceStartup;
    private readonly string _message;

    public Log(string message, string prefix="Info")
    {
        _message = message;
        Prefix = prefix;
    }

    public string GetTimeFormattedString()
    {
        var t = TimeSpan.FromSeconds(_time);
        return $"[{Prefix}] [{t:hh':'mm':'ss}] {_message}";
    }
}