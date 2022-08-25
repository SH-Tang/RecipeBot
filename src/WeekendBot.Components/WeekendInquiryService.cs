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
using Discord.Common;
using Discord.Common.Options;
using Microsoft.Extensions.Options;
using WeekendBot.Core;
using WeekendBot.Utils;

namespace WeekendBot.Components
{
    /// <summary>
    /// Service for inquiring whether it is weekend.
    /// </summary>
    public class WeekendInquiryService : IWeekendInquiryService
    {
        private readonly ITimeProvider timeProvider;
        private readonly StringFormatOptions formatOptions;

        /// <summary>
        /// Creates a new instance of <see cref="WeekendInquiryService"/>.
        /// </summary>
        /// <param name="timeProvider">The <see cref="ITimeProvider"/> for providing time information.</param>
        /// <param name="options">The options to format the string with.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null.</c></exception>
        public WeekendInquiryService(ITimeProvider timeProvider, IOptions<StringFormatOptions> options)
        {
            timeProvider.IsNotNull(nameof(timeProvider));
            options.IsNotNull(nameof(options));

            this.timeProvider = timeProvider;
            formatOptions = options.Value;
        }

        public async Task<string> GetIsWeekendMessageAsync()
        {
            DateTime currentDateTime = await timeProvider.GetCurrentDateTimeAsync();
            string message = IsWeekend(currentDateTime)
                                 ? "Ja, dat is het!"
                                 : "Nee, dat is het niet...";
            return message;
        }

        public async Task<string> GetTimeToWeekendMessageAsync()
        {
            DateTime currentDateTime = await timeProvider.GetCurrentDateTimeAsync();
            if (IsWeekend(currentDateTime))
            {
                return "De tijd tot het weekend is 0s, want het is al weekend!";
            }

            TimeSpan timeUntilWeekend = GetTimeSpanUntilWeekend(currentDateTime);
            return $"De tijd tot {formatOptions.Format(GetWeekendDateTime(currentDateTime))} is {formatOptions.Format(timeUntilWeekend)}, oftewel:" + Environment.NewLine +
                   $"- {formatOptions.Format(timeUntilWeekend.TotalDays)} dagen" + Environment.NewLine +
                   $"- {formatOptions.Format(timeUntilWeekend.TotalHours)} uren" + Environment.NewLine +
                   $"- {formatOptions.Format(timeUntilWeekend.TotalMinutes)} minuten" + Environment.NewLine +
                   $"- {formatOptions.Format(timeUntilWeekend.TotalSeconds)} seconden";
        }

        private static bool IsWeekend(DateTime currentDateTime)
        {
            DayOfWeek currentDayOfWeek = currentDateTime.DayOfWeek;

            if (currentDayOfWeek == DayOfWeek.Saturday
                || currentDayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            if (currentDayOfWeek == DayOfWeek.Friday && currentDateTime.Hour >= 16)
            {
                return true;
            }

            return false;
        }

        private static TimeSpan GetTimeSpanUntilWeekend(DateTime currentDateTime)
        {
            return GetWeekendDateTime(currentDateTime) - currentDateTime;
        }

        private static DateTime GetWeekendDateTime(DateTime currentDateTime)
        {
            DateTime closestWeekendDateTimeByDays = currentDateTime.AddDays(GetNumberOfDaysUntilWeekend(currentDateTime));
            var closestWeekendDateTime = new DateTime(closestWeekendDateTimeByDays.Year,
                                                      closestWeekendDateTimeByDays.Month,
                                                      closestWeekendDateTimeByDays.Day,
                                                      16, 0, 0);
            return closestWeekendDateTime;
        }

        private static int GetNumberOfDaysUntilWeekend(DateTime currentDateTime)
        {
            DayOfWeek dayOfWeek = currentDateTime.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 4;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 2;
                case DayOfWeek.Thursday:
                    return 1;
                case DayOfWeek.Friday:
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return 0;
                default:
                    throw new NotSupportedException($"Unexpected value {dayOfWeek} of {typeof(DayOfWeek)}");
            }
        }
    }
}