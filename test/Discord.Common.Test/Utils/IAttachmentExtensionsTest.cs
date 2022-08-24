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

using Discord.Common.Utils;
using NSubstitute;
using WeekendBot.TestUtils;
using Xunit;

namespace Discord.Common.Test.Utils;

public class IAttachmentExtensionsTest
{
    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    [InlineData("someImageExtension")]
    public void Attachment_returns_true_when_image(string? imageType)
    {
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns($"image/{imageType}");

        bool result = attachment.IsImage();

        Assert.True(result);
    }

    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    [InlineData("somePrefix/")]
    public void Attachment_returns_false_when_not_an_image(string? imageType)
    {
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns(imageType);

        bool result = attachment.IsImage();

        Assert.False(result);
    }
}