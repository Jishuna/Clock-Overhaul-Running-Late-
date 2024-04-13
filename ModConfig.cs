namespace RunningLate
{
    public class ModConfig
    {
        public HourFormat HourFormat { get; set; } = HourFormat.H12_HOUR;
        public MinuteFormat MinuteFormat { get; set; } = MinuteFormat.EACH_MINUTE;
        public AmPmPosition AmPmPosition { get; set; } = AmPmPosition.AFTER;
        public bool DisableClock { get; set; } = false;
    }

    public enum MinuteFormat
    {
        EACH_MINUTE, VANILLA
    }

    public enum HourFormat
    {
        H12_HOUR, H24_HOUR, H24_HOUR_FIXED
    }

    public enum AmPmPosition
    {
        AFTER, BEFORE, NONE
    }
}
