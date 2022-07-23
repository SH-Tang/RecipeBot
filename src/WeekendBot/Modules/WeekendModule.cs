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
using Discord.Commands;
using WeekendBot.Core;
using WeekendBot.Utils;

namespace WeekendBot.Modules
{
    /// <summary>
    /// Definition containing the weekend module commands.
    /// </summary>
    public class WeekendModule : ModuleBase<SocketCommandContext>
    {
        private readonly IWeekendInquiryService inquiryService;

        /// <summary>
        /// Creates a new instance of <see cref="WeekendModule"/>.
        /// </summary>
        /// <param name="inquiryService">The <see cref="IWeekendInquiryService"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inquiryService"/> is <c>null</c>.</exception>
        public WeekendModule(IWeekendInquiryService inquiryService)
        {
            this.inquiryService = inquiryService;
            inquiryService.IsNotNull(nameof(inquiryService));
        }

        [Command("weekend?")]
        [Summary("Gets a response whether the current (local) time is defined as a weekend.")]
        public async Task GetIsItWeekendResponseAsync()
        {
            string message = await inquiryService.GetIsWeekendMessageAsync();
            await ReplyAsync(message);
        }

        [Command("timetoweekend?")]
        [Summary("Gets a response between the time of invoking the command and until it is the first weekend date time.")]
        public async Task GetTimeToWeekendResponseAsync()
        {
            string message = await inquiryService.GetTimeToWeekendMessageAsync();
            await ReplyAsync(message);
        }
    }
}