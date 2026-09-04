using System;
using System.Collections.Generic;
using System.Linq;

public class DigitalClock
{
    private Dictionary<string, string> timeZones;

    public DigitalClock()
    {
        // Initialize common time zones
        timeZones = new Dictionary<string, string>
        {
            { "UTC", "UTC" },
            { "EST", "Eastern Standard Time" },
            { "CST", "Central Standard Time" },
            { "MST", "Mountain Standard Time" },
            { "PST", "Pacific Standard Time" },
            { "GMT", "GMT Standard Time" },
            { "CET", "Central European Standard Time" },
            { "IST", "India Standard Time" },
            { "JST", "Tokyo Standard Time" },
            { "AEST", "AUS Eastern Standard Time" }
        };
    }

    /// <summary>
    /// Gets the current time in a specific time zone
    /// </summary>
    public string GetTimeInTimeZone(string timeZoneId)
    {
        try
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZones[timeZoneId]);
            DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.Now, tzi);
            return localTime.ToString("HH:mm:ss");
        }
        catch (KeyNotFoundException)
        {
            return $"Time zone '{timeZoneId}' not found";
        }
    }

    /// <summary>
    /// Displays the current time across all configured time zones
    /// </summary>
    public string DisplayAllTimeZones()
    {
        string output = "=== Digital Clock - World Time ===\n";
        output += DateTime.Now.ToString("dddd, MMMM dd, yyyy") + "\n\n";

        foreach (var tz in timeZones)
        {
            string time = GetTimeInTimeZone(tz.Key);
            output += $"{tz.Key.PadRight(10)}: {time}\n";
        }

        return output;
    }

    /// <summary>
    /// Gets current time with timezone abbreviation
    /// </summary>
    public string GetFormattedTime(string timeZoneId)
    {
        try
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZones[timeZoneId]);
            DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.Now, tzi);
            return $"{timeZoneId}: {localTime:HH:mm:ss}";
        }
        catch (KeyNotFoundException)
        {
            return $"Time zone '{timeZoneId}' not found";
        }
    }

    /// <summary>
    /// Add a custom time zone
    /// </summary>
    public void AddTimeZone(string abbreviation, string windowsTimeZoneId)
    {
        if (!timeZones.ContainsKey(abbreviation))
        {
            timeZones[abbreviation] = windowsTimeZoneId;
        }
    }

    /// <summary>
    /// Gets list of all available time zones
    /// </summary>
    public List<string> GetAvailableTimeZones()
    {
        return timeZones.Keys.ToList();
    }

    /// <summary>
    /// Gets time difference between two time zones
    /// </summary>
    public string GetTimeDifference(string tz1, string tz2)
    {
        try
        {
            TimeZoneInfo tzi1 = TimeZoneInfo.FindSystemTimeZoneById(timeZones[tz1]);
            TimeZoneInfo tzi2 = TimeZoneInfo.FindSystemTimeZoneById(timeZones[tz2]);

            DateTime now = DateTime.UtcNow;
            DateTime time1 = TimeZoneInfo.ConvertTime(now, tzi1);
            DateTime time2 = TimeZoneInfo.ConvertTime(now, tzi2);

            TimeSpan difference = time2 - time1;
            return $"Difference between {tz1} and {tz2}: {(int)difference.TotalHours} hours {difference.Minutes} minutes";
        }
        catch (KeyNotFoundException)
        {
            return "One or both time zones not found";
        }
    }
}
