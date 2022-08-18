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

using System.Collections;
using System.Collections.Generic;

namespace WeekendBot.TestUtils;

/// <summary>
/// Generator that generates invalid urls with the http(s) protocol.
/// </summary>
public class InvalidHttpUrlDataGenerator : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[]
        {
            string.Empty
        };
        yield return new object?[]
        {
            "         "
        };
        yield return new object?[]
        {
            null
        };
        yield return new object?[]
        {
            "http://"
        };
        yield return new object?[]
        {
            "https://"
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}