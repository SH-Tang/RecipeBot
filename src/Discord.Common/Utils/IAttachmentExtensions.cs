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
using WeekendBot.Utils;

namespace Discord.Common.Utils;

/// <summary>
/// Class containing extension methods for <see cref="IAttachment"/>.
/// </summary>
public static class IAttachmentExtensions
{
    /// <summary>
    /// Returns whether the attachment represents an image.
    /// </summary>
    /// <param name="attachment">The <see cref="IAttachment"/> to check.</param>
    /// <returns><c>true</c> if <see cref="IAttachment"/> is an image; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="attachment"/> is <c>null</c>.</exception>
    public static bool IsImage(this IAttachment attachment)
    {
        attachment.IsNotNull(nameof(attachment));

        string contentType = attachment.ContentType;
        return !string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("image/");
    }
}