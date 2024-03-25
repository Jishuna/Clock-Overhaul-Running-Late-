using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace RunningLate
{
    public class ModEntry : Mod
    {
        public static IMonitor Logger { get; private set; } = null!;
        public static ModConfig Config { get; set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Logger = Monitor;
            Config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.DayTimeMoneyBox), nameof(StardewValley.Menus.DayTimeMoneyBox.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(DayTimeMoneyBoxPatch), nameof(DayTimeMoneyBoxPatch.Draw_Prefix)),
                transpiler: new HarmonyMethod(typeof(DayTimeMoneyBoxPatch), nameof(DayTimeMoneyBoxPatch.Draw_Transpiler))
            );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () =>
                {
                    Config = new ModConfig();
                    Helper.WriteConfig(Config);
                },
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddSectionTitle(this.ModManifest, I18n.Config_Section_Display_Name);

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: I18n.Config_HourFormat_Name,
                tooltip: I18n.Config_HourFormat_Tooltip,
                allowedValues: Enum.GetNames(typeof(HourFormat)),
                formatAllowedValue: this.TranslateHourFormat,
                getValue: Config.HourFormat.ToString,
                setValue: value => Config.HourFormat = (HourFormat)Enum.Parse(typeof(HourFormat), value),
                fieldId: "hour_format"
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: I18n.Config_MinuteFormat_Name,
                tooltip: I18n.Config_MinuteFormat_Tooltip,
                allowedValues: Enum.GetNames(typeof(MinuteFormat)),
                formatAllowedValue: this.TranslateMinuteFormat,
                getValue: Config.MinuteFormat.ToString,
                setValue: value => Config.MinuteFormat = (MinuteFormat)Enum.Parse(typeof(MinuteFormat), value),
                fieldId: "minute_format"
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: I18n.Config_AmPmPosition_Name,
                tooltip:I18n.Config_AmPmPosition_Tooltip,
                allowedValues: Enum.GetNames(typeof(AmPmPosition)),
                formatAllowedValue: this.TranslateAmPmPosition,
                getValue: Config.AmPmPosition.ToString,
                setValue: value => Config.AmPmPosition = (AmPmPosition)Enum.Parse(typeof(AmPmPosition), value),
                fieldId: "am_pm_position"
            );

            configMenu.AddSectionTitle(this.ModManifest, I18n.Config_Section_Extra_Name);

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: I18n.Config_DisableClock_Name,
                tooltip: I18n.Config_DisableClock_Tooltip,
                getValue: () => Config.DisableClock,
                setValue: value => Config.DisableClock = value,
                fieldId: "disable_clock"
            );
        }

        private string TranslateHourFormat(string rawFormat)
        {
            if (!Enum.TryParse(rawFormat, out HourFormat format))
                return rawFormat;

            return format switch
            {
                HourFormat.H12_HOUR => I18n.Config_HourFormat_Values_12(),
                HourFormat.H24_HOUR => I18n.Config_HourFormat_Values_24(),
                _ => format.ToString()
            };
        }

        private string TranslateMinuteFormat(string rawFormat)
        {
            if (!Enum.TryParse(rawFormat, out MinuteFormat format))
                return rawFormat;

            return format switch
            {
                MinuteFormat.EACH_MINUTE => I18n.Config_MinuteFormat_Values_EachMinute(),
                MinuteFormat.VANILLA => I18n.Config_MinuteFormat_Values_Vanilla(),
                _ => format.ToString()
            };
        }

        private string TranslateAmPmPosition(string rawPosition)
        {
            if (!Enum.TryParse(rawPosition, out AmPmPosition position))
                return rawPosition;

            return position switch
            {
                AmPmPosition.AFTER => I18n.Config_AmPmPosition_Values_After(),
                AmPmPosition.BEFORE => I18n.Config_AmPmPosition_Values_Before(),
                AmPmPosition.NONE => I18n.Config_AmPmPosition_Values_None(),
                _ => position.ToString()
            };
        }
    }
}
