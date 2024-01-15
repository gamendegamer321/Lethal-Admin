using System;
using UnityEngine;

namespace LethalAdmin.Logging;

public class Log
{
    private readonly string _prefix;
    
    private readonly float _time = Time.realtimeSinceStartup;
    private readonly string _message;

    public Log(string message, string prefix="Info")
    {
        _message = message;
        _prefix = prefix;
    }

    public string GetTimeFormattedString()
    {
        var t = TimeSpan.FromSeconds(_time);
        return $"[{t:hh':'mm':'ss}] [{_prefix}] {_message}";
    }
}