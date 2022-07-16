// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

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
                ? "Ja het is weekend! XD"
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