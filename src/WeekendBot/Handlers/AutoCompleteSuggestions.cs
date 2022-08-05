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

using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using WeekendBot.Utils;

namespace WeekendBot.Handlers;

/// <summary>
/// Class containing autocomplete suggestions.
/// </summary>
public class AutoCompleteSuggestions
{
    private readonly List<string> suggestions;

    public AutoCompleteSuggestions()
    {
        suggestions = new List<string>();
    }

    public void AddSuggestion(string suggestion)
    {
        suggestion.IsNotNullOrWhiteSpaces(nameof(suggestion));
        suggestions.Add(suggestion);
    }

    public IEnumerable<string> Suggestions => suggestions;
}