using System;
using UnityEngine;

namespace LethalAdmin.Logging;

public abstract class LogBase
{
    private readonly float _time = Time.realtimeSinceStartup;

    protected abstract string GetString();

    public string GetTimeFormattedString()
    {
        var t = TimeSpan.FromSeconds(_time);
        return "[" + t.ToString("hh':'mm':'ss") + "] " + GetString();
    }
}