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

namespace WeekendBot.Implementations.Test
{
    public class WeekendInquiryServiceTest
    {
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
        public void GetIsWeekendMessage_DayIsWeekDay_ReturnsExpectedMessage(int day, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var currentDate = new DateTime(2022, 7, day);

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
        public void GetIsWeekendMessage_DayIsWeekendDay_ReturnsExpectedMessage(int day, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var currentDate = new DateTime(2022, 7, day);

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
        public void GetTimeToWeekendMessage_DayIsWeekDay_ReturnsExpectedMessage(int day, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var random = new Random(21);
            var currentDate = new DateTime(2022, 7, day, random.Next(23), random.Next(59), random.Next(59));

            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentDateTime().Returns(currentDate);

            var service = new WeekendInquiryService(timeProvider);

            // Precondition
            Assert.Equal(expectedDayOfWeek, currentDate.DayOfWeek);

            // Call
            string message = service.GetTimeToWeekendMessage();

            // Assert
            TimeSpan expectedTimeToWeekend = new DateTime(2022, 7, 16) - currentDate;
            string expectedMessage = $"De tijd tot het weekend is {expectedTimeToWeekend}, oftewel:" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalDays} dagen" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalHours} uren" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalMinutes} minuten" + Environment.NewLine +
                                     $"- {expectedTimeToWeekend.TotalSeconds} seconden";
            Assert.Equal(expectedMessage, message);
        }

        [Theory]
        [MemberData(nameof(WeekendDays))]
        public void GetTimeToWeekendMessage_DayIsWeekendDay_ReturnsExpectedMessage(int day, DayOfWeek expectedDayOfWeek)
        {
            // Setup
            var random = new Random(21);
            var currentDate = new DateTime(2022, 7, day);

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
            yield return new object[]
            {
                11,
                DayOfWeek.Monday
            };

            yield return new object[]
            {
                12,
                DayOfWeek.Tuesday
            };

            yield return new object[]
            {
                13,
                DayOfWeek.Wednesday
            };

            yield return new object[]
            {
                14,
                DayOfWeek.Thursday
            };

            yield return new object[]
            {
                15,
                DayOfWeek.Friday
            };
        }

        private static IEnumerable<object[]> WeekendDays()
        {
            yield return new object[]
            {
                16,
                DayOfWeek.Saturday
            };

            yield return new object[]
            {
                17,
                DayOfWeek.Sunday
            };
        }
    }
}