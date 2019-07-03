using System;
using UnityEngine;

public class TimeRecorder
{
    private float startTime;
    public void Begin()
    {
        startTime = Time.realtimeSinceStartup;
    }

    public string End()
    {
        return (Time.realtimeSinceStartup - startTime).ToString("F3");
    }

    public void End(string prefix)
    {
        Logger.DEBUG(prefix, "cost time", End(), "s");
    }
}