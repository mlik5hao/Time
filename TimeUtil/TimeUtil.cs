using UnityEngine;

public static class TimeUtil
{
    private static int DaySeconds = 86400;
    private static int HourSeconds = 3600;
    
    public static bool Inited;
    private static TimeLine timeLine;
    private static TimedTask timedTask;
    private static TimeRecorder timeRecorder;
    private static string[] timeArr = new string[3];

    public static void Init(object ticks)
    {
        timeLine = new TimeLine();
        timeLine.Init(System.Convert.ToDouble(ticks));

        Logger.DEBUG("ticks offset: ", timeLine.OffsetTicks());

        timedTask = new TimedTask();
        timedTask.Init();

        timeRecorder = new TimeRecorder();

        Inited = true;
    }

    // 添加超时任务.
    // 参数: 1-起始时间(从服务器获取, 单位:秒), 2-超时总时间(单位:秒), 3-回调.
    public static int AddTimeoutTask(double startTicks, double cdTicks, TimedTask.TaskAction action)
    {
        int ret = timedTask.Add(startTicks, startTicks + cdTicks, action, 1, false);
        Logger.DEBUG("AddTimeoutTask Add info: ", startTicks, cdTicks, GetNowServerTicks(), ret);
        return ret;
    }

    // 添加定时循环任务.
    // 参数: 1-起始时间(从服务器获取, 单位:秒), 2-定时间隔时间(单位:秒), 3-回调.
    public static int AddIntervalTask(double startTicks, double cdTicks, TimedTask.TaskAction action)
    {
        int ret = timedTask.Add(startTicks, startTicks + cdTicks, action, 2, true);
        Logger.DEBUG("AddIntervalTask Add info: ", startTicks, cdTicks, GetNowServerTicks(), ret);
        return ret;
    }

    // 添加秒表倒计时任务.
    // 参数: 1-起始时间(从服务器获取, 单位:秒), 2-总计时时间(单位:秒), 3-回调, 4-是否自动移除.
    public static int AddSecondsTask(double startTicks, double totalTicks, TimedTask.TaskAction action, bool autoRemove = true)
    {
        Logger.DEBUG("AddSecondsTask action info: ", action);
        int ret = timedTask.Add(startTicks, startTicks + totalTicks, action, 3, !autoRemove);
        Logger.DEBUG("AddSecondsTask Add info: ", startTicks, totalTicks, GetNowServerTicks(), ret);
        return ret;
    }

    // 添加时钟定时任务.
    // 参数: 1-回调, 4-是否每小时任务.
    public static int AddHoursTask(TimedTask.TaskAction action, bool isEveryHour = false)
    {
        int serverTicks = (int)GetNowServerTicks();

        int tempSeconds = serverTicks % HourSeconds;

        double st = serverTicks - tempSeconds;

        int ret = timedTask.Add(st, st + HourSeconds, action, 4, isEveryHour);

        Logger.DEBUG("AddHoursTask Add Info:", serverTicks, tempSeconds, st, GetNowServerTicks(), ret);
        return ret;
    }

    // 添加每日定时任务.
    // 参数: 1-时区(取值范围[0, 23]), 2-时钟数值(取值范围[0, 23]), 3-回调, 4-是否每天任务.
    public static int AddDaysTask(int timeZone, int hour, TimedTask.TaskAction action, bool isEveryDay = false)
    {
        int serverTicks = (int)GetNowServerTicks();

        int utcHour = hour - timeZone;

        int tempSeconds = serverTicks % DaySeconds;

        double st = serverTicks - tempSeconds + utcHour * HourSeconds;
        st = st > serverTicks ? st - DaySeconds : st;
        int ret = timedTask.Add(st, st + DaySeconds, action, 5, isEveryDay);

        Logger.DEBUG("AddDaysTask Add Info:", serverTicks, hour, timeZone, utcHour, tempSeconds, st, GetNowServerTicks(), ret);
        return ret;
    }

    // 调用定时任务回调.
    public static void RunTimedTaskAction(int id)
    {
        timedTask.DoAction(id);
    }

    // 取消定时任务.
    public static void RemoveTimedTask(int id)
    {
        timedTask.Remove(id);
    }

    // 检查定时任务是否存在.
    public static bool HasTimedTask(int id)
    {
        //Logger.ERROR("find task info: ", id);
        return (timedTask.Get(id) != null);
    }

    // 获取当前时间.
    public static double GetNowServerTicks()
    {
        if (timeLine == null) return 0;
        return timeLine.GetNowServerTicks();
    }

    private static double timeTick;
    public static void LateUpdate()
    {
        if (!Inited) return;

        if (Time.realtimeSinceStartup - timeTick < 1) return;
        timeTick = Time.realtimeSinceStartup;

        timeLine.Loop(timeTick);
        timedTask.Loop(timeLine.GetNowServerTicks());
    }

    //时间格式 小时：分钟：秒
    public static string[] TimeFormate(int t)
    {
        
        if (t < 0) return timeArr;
        int hour = t / 3600;
        int min = t % 3600 /60;
        int sec = t % 60;
        timeArr[0] = hour < 10 ? "0" + hour.ToString() : hour.ToString();
        timeArr[1] = min < 10 ? "0" + min.ToString(): min.ToString();
        timeArr[2] = sec < 10 ? "0" + sec.ToString() : sec.ToString();
        return timeArr;
    }
}
