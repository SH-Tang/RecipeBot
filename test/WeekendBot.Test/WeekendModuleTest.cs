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

using Discord.Commands;
using NSubstitute;
using WeekendBot.Core;
using WeekendBot.Modules;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Test
{
    public class WeekendModuleTest
    {
        [Fact]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var inquiryService = Substitute.For<IWeekendInquiryService>();

            // Call
            var module = new WeekendModule(inquiryService);

            // Assert
            Assert.IsAssignableFrom<ModuleBase<SocketCommandContext>>(module);
        }

        [Fact]
        public void GetIsItWeekendResponseAsync_Always_ReturnsExpectedAttributes()
        {
            // Call
            CommandAttribute commandAttribute = ReflectionHelper.GetCustomAttribute<WeekendModule, CommandAttribute>(
                nameof(WeekendModule.GetIsItWeekendResponseAsync));
            SummaryAttribute summaryAttribute = ReflectionHelper.GetCustomAttribute<WeekendModule, SummaryAttribute>(
                nameof(WeekendModule.GetIsItWeekendResponseAsync));

            // Assert
            Assert.NotNull(commandAttribute);
            Assert.Equal("weekend?", commandAttribute.Text.ToLower());

            Assert.NotNull(commandAttribute);
            const string expectedSummary = "Gets a response whether the current (local) time is defined as a weekend.";
            Assert.Equal(expectedSummary, summaryAttribute.Text);
        }

        [Fact]
        public void GetTimeToWeekendResponseAsync_Always_ReturnsExpectedAttributes()
        {
            // Call
            CommandAttribute commandAttribute = ReflectionHelper.GetCustomAttribute<WeekendModule, CommandAttribute>(
                nameof(WeekendModule.GetTimeToWeekendResponseAsync));
            SummaryAttribute summaryAttribute = ReflectionHelper.GetCustomAttribute<WeekendModule, SummaryAttribute>(
                nameof(WeekendModule.GetTimeToWeekendResponseAsync));

            // Assert
            Assert.NotNull(commandAttribute);
            Assert.Equal("timetoweekend?", commandAttribute.Text.ToLower());

            Assert.NotNull(commandAttribute);
            const string expectedSummary = "Gets a response between the time of invoking the command and until it is the first weekend date time.";
            Assert.Equal(expectedSummary, summaryAttribute.Text);
        }
    }
}