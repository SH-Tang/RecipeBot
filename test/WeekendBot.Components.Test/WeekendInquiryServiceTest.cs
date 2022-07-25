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
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Common.Options;
using Microsoft.Extensions.Options;
using NSubstitute;
using WeekendBot.Core;
using Xunit;

namespace WeekendBot.Components.Test
{
    public class WeekendInquiryServiceTest
    {
        private static readonly DateTime weekendDateTime = new DateTime(2022, 7, 15, 16, 0, 0);

        [Fact]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var timeProvider = Substitute.For<ITimeProvider>();
            var formatOptions = Substitute.For<IOptions<StringFormatOptions>>();

            // Call
            var service = new WeekendInquiryService(timeProvider, formatOptions);

            // Assert
            Assert.IsAssignableFrom<IWeekendInquiryService>(service);
        }

        [Theory]
        [MemberData(nameof(WeekDays))]
        public async Task GetIsWeekendMessageAsync_DayIsWeekDayDateTime_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var formatOptions = Substitute.For<IOptions<StringFormatOptions>>();

            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTimeAsync().ReturnsForAnyArgs(currentDate);

            var service = new WeekendInquiryService(timeProvider, formatOptions);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = await service.GetIsWeekendMessageAsync();

            // Assert
            Assert.Equal("Nee, dat is het niet...", message);
        }

        [Theory]
        [MemberData(nameof(WeekendDays))]
        public async Task GetIsWeekendMessageAsync_DayIsWeekendDateTime_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var formatOptions = Substitute.For<IOptions<StringFormatOptions>>();

            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTimeAsync().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider, formatOptions);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = await service.GetIsWeekendMessageAsync();

            // Assert
            Assert.Equal("Ja, dat is het!", message);
        }

        [Theory]
        [MemberData(nameof(WeekDays))]
        public async Task GetTimeToWeekendMessageAsync_DayIsWeekDateTime_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var options = Substitute.For<IOptions<StringFormatOptions>>();
            var formatOptions = new StringFormatOptions();
            options.Value.ReturnsForAnyArgs(formatOptions);

            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTimeAsync().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider, options);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = await service.GetTimeToWeekendMessageAsync();

            // Assert
            TimeSpan expectedTimeToWeekend = weekendDateTime - currentDate;
            string expectedMessage = $"De tijd tot {formatOptions.Format(weekendDateTime)} is {formatOptions.Format(expectedTimeToWeekend)}, oftewel:" + Environment.NewLine +
                                     $"- {formatOptions.Format(expectedTimeToWeekend.TotalDays)} dagen" + Environment.NewLine +
                                     $"- {formatOptions.Format(expectedTimeToWeekend.TotalHours)} uren" + Environment.NewLine +
                                     $"- {formatOptions.Format(expectedTimeToWeekend.TotalMinutes)} minuten" + Environment.NewLine +
                                     $"- {formatOptions.Format(expectedTimeToWeekend.TotalSeconds)} seconden";
            Assert.Equal(expectedMessage, message);
        }

        [Theory]
        [MemberData(nameof(WeekendDays))]
        public async Task GetTimeToWeekendMessageAsync_DayIsWeekendDateTime_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var formatOptions = Substitute.For<IOptions<StringFormatOptions>>();

            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTimeAsync().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider, formatOptions);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = await service.GetTimeToWeekendMessageAsync();

            // Assert
            Assert.Equal("De tijd tot het weekend is 0s, want het is al weekend!", message);
        }

        private static DateTime CreateDateTime(int day, int month, int year)
        {
            var random = new Random(21);
            return new DateTime(year, month, day, random.Next(24), random.Next(60), random.Next(60));
        }

        private static IEnumerable<object[]> WeekDays()
        {
            int year = weekendDateTime.Year;
            int month = weekendDateTime.Month;

            yield return new object[]
            {
                CreateDateTime(11, month, year),
                DayOfWeek.Monday
            };

            yield return new object[]
            {
                CreateDateTime(12, month, year),
                DayOfWeek.Tuesday
            };

            yield return new object[]
            {
                CreateDateTime(13, month, year),
                DayOfWeek.Wednesday
            };

            yield return new object[]
            {
                CreateDateTime(14, month, year),
                DayOfWeek.Thursday
            };

            yield return new object[]
            {
                new DateTime(year, month, 15, 15, 59, 59),
                DayOfWeek.Friday
            };
        }

        private static IEnumerable<object[]> WeekendDays()
        {
            int year = weekendDateTime.Year;
            int month = weekendDateTime.Month;

            yield return new object[]
            {
                new DateTime(year, month, 15, 16, 0, 0),
                DayOfWeek.Friday
            };

            yield return new object[]
            {
                CreateDateTime(16, month, year),
                DayOfWeek.Saturday
            };

            yield return new object[]
            {
                CreateDateTime(17, month, year),
                DayOfWeek.Sunday
            };
        }
    }
}