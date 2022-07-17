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

namespace WeekendBot.Core
{
    /// <summary>
    /// Interface for providing the time.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the current date time on the host.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representing the current date time on the host.</returns>
        Task<DateTime> GetCurrentDateTimeAsync();
    }
}