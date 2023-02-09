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
    private readonly string emptyMessage;
    private readonly Func<TDataEntry, string> getEntryFormatFunc;
    private readonly string header;
    private readonly IMessageCharacterLimitProvider limitProvider;

    /// <summary>
    /// Creates a new instance of <see cref="DataEntryCollectionMessageFormattingService{TDataEntry}"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the message character limits from.</param>
    /// <param name="header">The header of each code block.</param>
    /// <param name="emptyMessage">The empty message when there is nothing to format.</param>
    /// <param name="getEntryFormatFunc">The function to format individual lines in the block.</param>
    public DataEntryCollectionMessageFormattingService(IMessageCharacterLimitProvider limitProvider,
                                             string header, string emptyMessage, Func<TDataEntry, string> getEntryFormatFunc)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        header.IsNotNullOrWhiteSpaces(nameof(header));
        emptyMessage.IsNotNullOrWhiteSpaces(nameof(emptyMessage));
        getEntryFormatFunc.IsNotNull(nameof(getEntryFormatFunc));

        this.limitProvider = limitProvider;
        this.header = header;
        this.getEntryFormatFunc = getEntryFormatFunc;
        this.emptyMessage = emptyMessage;
    }

    public IReadOnlyList<string> CreateMessages(IReadOnlyList<TDataEntry> entries)
    {
        entries.IsNotNull(nameof(entries));

        if (!entries.Any())
        {
            return new[]
            {
                emptyMessage
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