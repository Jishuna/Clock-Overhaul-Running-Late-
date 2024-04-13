using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RunningLate
{
    internal class Events
    {
        private readonly IModHelper modHelper;
        private readonly string modId;
        public Events(IModHelper helper, string modId)
        {
            this.modHelper = helper;
            this.modId = modId;

            helper.Events.GameLoop.UpdateTicking += OnUpdateTick;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void OnUpdateTick(object _, UpdateTickingEventArgs e)
        {
            if (!Game1.IsClient)
            {
                DayTimeMoneyBoxPatch.CurrentTimeMS = Game1.gameTimeInterval;
            }

            if (!Game1.IsServer)
            {
                return;
            }

            modHelper.Multiplayer.SendMessage(Game1.gameTimeInterval, "TimeInterval", new[] { modId });
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == modId && e.Type == "TimeInterval")
            {
                DayTimeMoneyBoxPatch.CurrentTimeMS = e.ReadAs<int>();
            }
        }
    }
}
