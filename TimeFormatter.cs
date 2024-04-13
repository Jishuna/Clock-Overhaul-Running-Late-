using StardewValley;
using System;
using System.Collections.Generic;

namespace RunningLate
{
    public class TimeFormatter
    {
        private static readonly IDictionary<HourFormat, Func<int, int, string>> HourFormatters = new Dictionary<HourFormat, Func<int, int, string>>
        {
            { HourFormat.H12_HOUR, (time, _) => time / 100 % 12 == 0 ? "12" : (time / 100 % 12).ToString() },
            { HourFormat.H24_HOUR, (time, _) => (time / 100 % 24).ToString() },
             { HourFormat.H24_HOUR_FIXED, (time, _) => (time / 100 % 24).ToString("00") },
        };

        private static readonly IDictionary<MinuteFormat, Func<int, int, string>> MinuteFormatters = new Dictionary<MinuteFormat, Func<int, int, string>>
        {
            { MinuteFormat.EACH_MINUTE, (time, msTime) => $"{time % 100 / 10}{msTime / GetMsPerGameMinute()}" },
            { MinuteFormat.VANILLA, (time, _) => (time % 100).ToString("00") },
        };

        public static string FormatHour(HourFormat format, int time, int msTime)
        {
            return HourFormatters[format].Invoke(time, msTime);
        }

        public static string FormatMiute(MinuteFormat format, int time, int msTime)
        {
            return MinuteFormatters[format].Invoke(time, msTime);
        }

        private static int GetMsPerGameMinute()
        {
            return Game1.realMilliSecondsPerGameMinute + (Game1.MasterPlayer.currentLocation == null ? 0 : Game1.MasterPlayer.currentLocation.ExtraMillisecondsPerInGameMinute);
        }
    }
}
