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
using FluentAssertions;
using Xunit;

namespace Common.Utils.Test;

public class UrlValidationHelperTest
{
    [Theory]
    [InlineData("http://foo.com/blah_blah")]
    [InlineData("http://foo.com/blah_blah/")]
    [InlineData("http://foo.com/blah_blah_(wikipedia)")]
    [InlineData("http://foo.com/blah_blah_(wikipedia)_(again)")]
    [InlineData("http://www.example.com/wpstyle/?p=364")]
    [InlineData("https://www.example.com/foo/?bar=baz&inga=42&quux")]
    [InlineData("http://✪df.ws/123")]
    [InlineData("http://userid:password@example.com:8080")]
    [InlineData("http://userid:password@example.com:8080/")]
    [InlineData("http://userid@example.com")]
    [InlineData("http://userid@example.com/")]
    [InlineData("http://userid@example.com:8080")]
    [InlineData("http://userid@example.com:8080/")]
    [InlineData("http://userid:password@example.com")]
    [InlineData("http://userid:password@example.com/")]
    [InlineData("http://142.42.1.1/")]
    [InlineData("http://142.42.1.1:8080/")]
    [InlineData("http://➡.ws/䨹")]
    [InlineData("http://⌘.ws")]
    [InlineData("http://⌘.ws/")]
    [InlineData("http://foo.com/blah_(wikipedia)#cite-1")]
    [InlineData("http://foo.com/blah_(wikipedia)_blah#cite-1")]
    [InlineData("http://foo.com/unicode_(✪)_in_parens")]
    [InlineData("http://foo.com/(something)?after=parens")]
    [InlineData("http://☺.damowmow.com/")]
    [InlineData("http://code.google.com/events/#&product=browser")]
    [InlineData("http://j.mp")]
    [InlineData("http://foo.bar/?q=Test%20URL-encoded%20stuff")]
    [InlineData("http://مثال.إختبار")]
    [InlineData("http://例子.测试")]
    [InlineData("http://उदाहरण.परीक्ष")]
    [InlineData("http://-.~_!$&'()*+)];=:%40:80%2f::::::@example.com")]
    [InlineData("http://1337.net")]
    [InlineData("http://a.b-c.de")]
    [InlineData("http://223.255.255.254")]
    public void Valid_http_url_throw_nothing(string validUrl)
    {
        // Call
        UrlValidationHelper.ValidateHttpUrl(validUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    [InlineData("http://")]
    [InlineData("http://.")]
    [InlineData("http://..")]
    [InlineData("http://../")]
    [InlineData("http://?")]
    [InlineData("http://??")]
    [InlineData("http://??/")]
    [InlineData("http://#")]
    [InlineData("http://##")]
    [InlineData("http://##/")]
    [InlineData("//")]
    [InlineData("//a")]
    [InlineData("///a")]
    [InlineData("///")]
    [InlineData("foo.com")]
    [InlineData("rdar://1234")]
    [InlineData("h://test")]
    [InlineData(":// should fail")]
    [InlineData("ftps://foo.bar/")]
    public void Invalid_http_url_throws_exception(string invalidUrl)
    {
        // Call
        Action call = () => UrlValidationHelper.ValidateHttpUrl(invalidUrl);

        // Assert
        call.Should().ThrowExactly<ArgumentException>()
            .WithMessage("url is an invalid http or https url.");
    }
}