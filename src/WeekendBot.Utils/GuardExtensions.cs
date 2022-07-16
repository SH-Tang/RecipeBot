// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// Balanced Field Length is free software: you can redistribute it and/or modify
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

namespace WeekendBot.Utils
{
    /// <summary>
    /// Class containing extension methods as guards.
    /// </summary>
    public static class GuardExtensions
    {
        /// <summary>
        /// Guards that <param name="argument"> is not <c>null</c>.</param>
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="argument"/></typeparam>
        /// <param name="argument">The argument to guard.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is <c>null</c>.</exception>
        public static void IsNotNull<T>(this T argument, string argumentName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}