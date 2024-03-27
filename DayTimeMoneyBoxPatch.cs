using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace RunningLate
{
    internal class DayTimeMoneyBoxPatch
    {
        private static readonly FieldInfo f_timeText = AccessTools.Field(typeof(DayTimeMoneyBox), "_timeText");
        private static readonly MethodInfo m_measureString = AccessTools.Method(typeof(SpriteFont), nameof(SpriteFont.MeasureString), new Type[] { typeof(StringBuilder) });
        private static readonly MethodInfo m_test = AccessTools.Method(typeof(DayTimeMoneyBoxPatch), nameof(DayTimeMoneyBoxPatch.UpdateClockTime));

        public static int CurrentTimeMS { get; set; }

        public static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                int index = -1;
                var codes = new List<CodeInstruction>(instructions);
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldfld
                        && codes[i].operand as FieldInfo == f_timeText
                        && codes[i + 1].opcode == OpCodes.Callvirt
                        && codes[i + 1].operand as MethodInfo == m_measureString)
                    {
                        index = i - 1;
                    }
                }

                if (index != -1)
                {
                    codes.Insert(index, new CodeInstruction(OpCodes.Call, m_test));
                    codes.Insert(index, new CodeInstruction(OpCodes.Ldfld, f_timeText));
                    codes.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
                }
                else
                {
                    ModEntry.Logger.Log("Failed to find patch target, mod features will not work!", LogLevel.Alert);
                }

                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log($"Failed in {nameof(Draw_Transpiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }

        public static void UpdateClockTime(StringBuilder builder)
        {
            builder.Clear();

            if (ModEntry.Config.DisableClock)
            {
                builder.Append("??:??");
                return;
            }

            int time = Game1.timeOfDay;

            if (ModEntry.Config.AmPmPosition == AmPmPosition.BEFORE)
            {
                builder.Append(GetAmPmString(time));
                builder.Append(' ');
            }

            builder.Append(TimeFormatter.FormatHour(ModEntry.Config.HourFormat, time, CurrentTimeMS));
            builder.Append(':');
            builder.Append(TimeFormatter.FormatMiute(ModEntry.Config.MinuteFormat, time, CurrentTimeMS));

            if (ModEntry.Config.AmPmPosition == AmPmPosition.AFTER)
            {
                builder.Append(' ');
                builder.Append(GetAmPmString(time));
            }
        }

        private static string GetAmPmString(int time)
        {
            if (time < 1200 || time >= 2400)
            {
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370");
            }
            else
            {
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371");
            }
        }
    }
}
