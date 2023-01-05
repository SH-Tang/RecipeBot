// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of RecipeBot.
//
// RecipeBot is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.IO;

namespace Common.Utils
{
    /// <summary>
    /// Class containing extension methods as guards.
    /// </summary>
    public static class GuardExtensions
    {
        /// <summary>
        /// Guards that <paramref name="argument"/> is not <c>null</c>.
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

        /// <summary>
        /// Guards that <paramref name="argument"/> is not <c>null</c> or consists of only whitespaces.
        /// </summary>
        /// <param name="argument">The argument to guard.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="argument"/> is <c>null</c> or consists of only whitespaces.</exception>
        public static void IsNotNullOrWhiteSpaces(this string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException($"{argumentName} cannot be null or exists of whitespaces only.", argumentName);
            }
        }

        /// <summary>
        /// Guards that <paramref name="argument"/> is a valid, accessible file path.
        /// </summary>
        /// <param name="argument">The argument to guard.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="argument"/> is:
        /// <list type="bullet">
        /// <item>An invalid file path or;</item>
        /// <item>A non-existing file path or;</item>
        /// <item>the file path does not have the sufficient permissions to access.</item>
        /// </list></exception>
        public static void IsExistingFilePath(this string argument, string argumentName)
        {
            if (!File.Exists(argument))
            {
                throw new ArgumentException($"{argumentName} must be an existing and accessible file path.", argumentName);
            }
        }

        /// <summary>
        /// Guards that the <paramref name="argument"/> is a valid enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum.</typeparam>
        /// <param name="argument">The argument to guard.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="argument"/> is an invalid enum.</exception>
        public static void IsValidEnum<TEnum>(this TEnum argument, string argumentName) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), argument))
            {
                throw new InvalidEnumArgumentException(argumentName, Convert.ToInt32(argument), typeof(TEnum));
            }
        }

        /// <summary>
        /// Guards that <paramref name="argument"/> is a valid argument.
        /// </summary>
        /// <param name="argument">The argument to guard.</param>
        /// <param name="getIsValidArgumentFunc">The function to evaluate whether the argument is valid.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="getIsValidArgumentFunc"/> returns <c>true</c>
        /// based on the <paramref name="argument"/>.</exception>
        public static void IsValidArgument<T>(this T argument, Func<T, bool> getIsValidArgumentFunc, string message, string parameterName)
        {
            if (!getIsValidArgumentFunc(argument))
            {
                throw new ArgumentException(message, parameterName);
            }
        }
    }
}