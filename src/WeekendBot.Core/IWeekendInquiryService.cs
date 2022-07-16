﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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

namespace WeekendBot.Core
{
    /// <summary>
    /// Interface for describing inquiries about the weekend.
    /// </summary>
    public interface IWeekendInquiryService
    {
        /// <summary>
        /// Gets a message whether it is weekend.
        /// </summary>
        /// <returns>A <see cref="string"/> containing a message whether it is weekend.</returns>
        string GetIsWeekendMessage();

        /// <summary>
        /// Gets a formatted message representing the time until the weekend.
        /// </summary>
        /// <returns>A <see cref="string"/> containing a message until it is weekend.</returns>
        string GetTimeToWeekendMessage();
    }
}