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

using WeekendBot.Core;
using WeekendBot.Utils;

namespace WeekendBot.Implementations
{
    /// <summary>
    /// Service for inquiring whether it is weekend.
    /// </summary>
    public class WeekendInquiryService : IWeekendInquiryService
    {
        private readonly ITimeProvider timeProvider;

        /// <summary>
        /// Creates a new instance of <see cref="WeekendInquiryService"/>.
        /// </summary>
        /// <param name="timeProvider">The <see cref="ITimeProvider"/> for providing time information.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="timeProvider"/> is <c>null.</c></exception>
        public WeekendInquiryService(ITimeProvider timeProvider)
        {
            timeProvider.IsNotNull(nameof(timeProvider));
            this.timeProvider = timeProvider;
        }

        public string GetIsWeekendMessage()
        {
            return IsWeekend(timeProvider.GetCurrentDateTime())
                       ? "Ja, dat is het!"
                       : "Nee, dat is het niet...";
        }

        public string GetTimeToWeekendMessage()
        {
            DateTime currentDateTime = timeProvider.GetCurrentDateTime();
            if (IsWeekend(currentDateTime))
            {
                return "De tijd tot het weekend is 0s, want het is al weekend!";
            }

            TimeSpan timeUntilWeekend = GetTimeSpanUntilWeekend(currentDateTime);
            return $"De tijd tot het weekend is {timeUntilWeekend}, oftewel:" + Environment.NewLine +
                   $"- {timeUntilWeekend.TotalDays} dagen" + Environment.NewLine +
                   $"- {timeUntilWeekend.TotalHours} uren" + Environment.NewLine +
                   $"- {timeUntilWeekend.TotalMinutes} minuten" + Environment.NewLine +
                   $"- {timeUntilWeekend.TotalSeconds} seconden";
        }

        private static bool IsWeekend(DateTime currentDateTime)
        {
            DayOfWeek currentDayOfWeek = currentDateTime.DayOfWeek;
            return (currentDayOfWeek == DayOfWeek.Saturday
                    || currentDayOfWeek == DayOfWeek.Sunday);
        }

        private static TimeSpan GetTimeSpanUntilWeekend(DateTime currentDateTime)
        {
            DateTime closestWeekendDateTimeByDays = currentDateTime.AddDays(GetNumberOfDaysUntilWeekend(currentDateTime));
            var closestWeekendDateTime = new DateTime(closestWeekendDateTimeByDays.Year,
                                                      closestWeekendDateTimeByDays.Month,
                                                      closestWeekendDateTimeByDays.Day);

            return closestWeekendDateTime - currentDateTime;
        }

        private static int GetNumberOfDaysUntilWeekend(DateTime currentDateTime)
        {
            DayOfWeek dayOfWeek = currentDateTime.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 5;
                case DayOfWeek.Tuesday:
                    return 4;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 2;
                case DayOfWeek.Friday:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}