using UnityEngine;
using System;

public sealed class TimeLine
{
    private sealed class TimeInfo
    {
        public double ServerTicks;
        public double OffsetTicks;

        public TimeInfo()
        {
            ServerTicks = 0;
            OffsetTicks = Time.realtimeSinceStartup;
        }

        public override string ToString()
        {
            return ToString(string.Empty);
        }

        public string ToString(string prefix)
        {
            return string.Format("time info[{0}]: server:{1}, unity start: {2}", prefix, ServerTicks.ToString("F2"), OffsetTicks.ToString("F2"));
        }
    }

    private TimeInfo startTimeInfo;
    private TimeInfo nowTimeInfo;

    public void Init(double ticks)
    {
        startTimeInfo = new TimeInfo();
        startTimeInfo.ServerTicks = ticks;

        nowTimeInfo = new TimeInfo();

        Logger.DEBUG("start time info: ", startTimeInfo.ToString());
    }

    public void Loop(double nt)
    {
        nowTimeInfo.ServerTicks = startTimeInfo.ServerTicks + nt - startTimeInfo.OffsetTicks;
    }

    //public void SyncServerTicks(double ticks)
    //{
    //    nowTimeInfo.ServerTicks = ticks;
    //}

    public double GetNowServerTicks()
    {
        return startTimeInfo.ServerTicks + Time.realtimeSinceStartup - startTimeInfo.OffsetTicks;
    }

    public double OffsetTicks()
    {
        return startTimeInfo.OffsetTicks;
    }
}
