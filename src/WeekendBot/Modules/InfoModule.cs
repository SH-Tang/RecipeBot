using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace WeekendBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private const DayOfWeek weekends = DayOfWeek.Friday | DayOfWeek.Saturday | DayOfWeek.Sunday;

        [Command("weekend?")]
        [Summary("Responds whether the current day is a weekend.")]
        public Task GetIsItWeekendResponseAsync()
        {
            DateTime currentTime = DateTime.Now;
            string message = ((currentTime.DayOfWeek & weekends) != 0)
                ? "Ja het is weekend!"
                : "Nee, dat is het niet :(";
            return ReplyAsync(message);
        }

        [Command("Bijna weekend?")]
        [Summary("Keeps track who is invoking almost weekend.")]
        public Task AlmostWeekendResponseAsync()
        {
            SocketUser user = Context.User;
            var messageReference = new MessageReference(Context.Message.Id);

            return ReplyAsync($"Gebruiker {user.Username} heeft bijna weekend gespammed!");
        }
    }
}