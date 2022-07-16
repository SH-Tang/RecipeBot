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
        [InlineData(11, DayOfWeek.Monday)]
        [InlineData(12, DayOfWeek.Tuesday)]
        [InlineData(13, DayOfWeek.Wednesday)]
        [InlineData(14, DayOfWeek.Thursday)]
        public void GetIsWeekendMessage_DayNotWeekend_ReturnsExpectedMessage(int day, DayOfWeek expectedDayOfWeek)
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
        [InlineData(15, DayOfWeek.Friday)]
        [InlineData(16, DayOfWeek.Saturday)]
        [InlineData(17, DayOfWeek.Sunday)]
        public void GetIsWeekendMessage_DayWeekend_ReturnsExpectedMessage(int day, DayOfWeek expectedDayOfWeek)
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
    }
}