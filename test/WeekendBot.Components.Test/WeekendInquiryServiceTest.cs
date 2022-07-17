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

            // Call
            var service = new WeekendInquiryService(timeProvider);

            // Assert
            Assert.IsAssignableFrom<IWeekendInquiryService>(service);
        }

        [Theory]
        [MemberData(nameof(WeekDays))]
        public void GetIsWeekendMessage_DayIsWeekDay_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTime().ReturnsForAnyArgs(currentDate);

            var service = new WeekendInquiryService(timeProvider);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = service.GetIsWeekendMessage();

            // Assert
            Assert.Equal("Nee, dat is het niet...", message);
        }

        [Theory]
        [MemberData(nameof(WeekendDays))]
        public void GetIsWeekendMessage_DayIsWeekendDay_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTime().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = service.GetIsWeekendMessage();

            // Assert
            Assert.Equal("Ja, dat is het!", message);
        }

        [Theory]
        [MemberData(nameof(WeekDays))]
        public void GetTimeToWeekendMessage_DayIsWeekDay_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTime().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = service.GetTimeToWeekendMessage();

            // Assert
            TimeSpan expectedTimeToWeekend = weekendDateTime - currentDate;
            string expectedMessage = $"De tijd tot het weekend is {expectedTimeToWeekend}, oftewel:" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalDays} dagen" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalHours} uren" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalMinutes} minuten" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalSeconds} seconden";
            Assert.Equal(expectedMessage, message);
        }

        [Theory]
        [MemberData(nameof(WeekendDays))]
        public void GetTimeToWeekendMessage_DayIsWeekendDay_ReturnsExpectedMessage(DateTime currentDate, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTime().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = service.GetTimeToWeekendMessage();

            // Assert
            Assert.Equal("De tijd tot het weekend is 0s, want het is al weekend!", message);
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

        private static DateTime CreateDateTime(int day, int month, int year)
        {
            var random = new Random(21);
            return new DateTime(year, month, day, random.Next(24), random.Next(60), random.Next(60));
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