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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
using Discord;
using RecipeBot.Controllers;

namespace RecipeBot.Services;

/// <summary>
/// Service to format data entry collections into messages.
/// </summary>
/// <typeparam name="TDataEntry">The data entry object to format.</typeparam>
internal class DataEntryCollectionMessageFormattingService<TDataEntry>
{
    private readonly Func<TDataEntry, string> getEntryFormatFunc;
    private readonly string header;
    private readonly IMessageCharacterLimitProvider limitProvider;

    /// <summary>
    /// Creates a new instance of <see cref="DataEntryCollectionMessageFormattingService{TDataEntry}"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the message character limits from.</param>
    /// <param name="header">The header of each code block.</param>
    /// <param name="getEntryFormatFunc">The function to format individual lines in the block.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limitProvider"/>
    /// or <paramref name="getEntryFormatFunc"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="header"/> is <c>null</c> or
    /// consists of whitespaces.</exception>
    public DataEntryCollectionMessageFormattingService(IMessageCharacterLimitProvider limitProvider,
                                                       string header, Func<TDataEntry, string> getEntryFormatFunc)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        header.IsNotNullOrWhiteSpaces(nameof(header));
        getEntryFormatFunc.IsNotNull(nameof(getEntryFormatFunc));

        this.limitProvider = limitProvider;
        this.header = header;
        this.getEntryFormatFunc = getEntryFormatFunc;
    }

    /// <summary>
    /// Creates a collection of formatted messages based on its input arguments.
    /// </summary>
    /// <param name="entries">The collection of data entries to format.</param>
    /// <param name="emptyCollectionMessage">The message to display when <paramref name="entries"/> is empty.</param>
    /// <returns>A collection of formatted messages.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entries"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="emptyCollectionMessage"/> is <c>null</c> or
    /// consists of whitespaces.</exception>
    public IReadOnlyList<string> CreateMessages(IEnumerable<TDataEntry> entries, string emptyCollectionMessage)
    {
        entries.IsNotNull(nameof(entries));
        emptyCollectionMessage.IsNotNullOrWhiteSpaces(nameof(emptyCollectionMessage));

        if (!entries.Any())
        {
            return new[]
            {
                emptyCollectionMessage
            };
        }

        var messages = new List<string>();
        StringBuilder messageBuilder = new StringBuilder().AppendLine(header);
        string formattedCurrentMessage = Format.Code(messageBuilder.ToString());

        foreach (TDataEntry currentEntry in entries)
        {
            string formattedEntry = getEntryFormatFunc(currentEntry);

            string messageWithCurrentEntry = $"{messageBuilder}{formattedEntry}";
            if (Format.Code(messageWithCurrentEntry).Length > limitProvider.MaxMessageLength)
            {
                messages.Add(formattedCurrentMessage);
                messageBuilder.Clear();
                messageBuilder.AppendLine(header);
            }

            messageBuilder.AppendLine(formattedEntry);

            var currentMessage = messageBuilder.ToString();
            formattedCurrentMessage = Format.Code(currentMessage);
        }

        messages.Add(formattedCurrentMessage);
        return messages;
    }
}