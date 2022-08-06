![License](https://img.shields.io/github/license/SiuHinTang/WeekendBot)
![GitHub last commit](https://img.shields.io/github/last-commit/SiuHinTang/WeekendBot)
![Unit tests](https://github.com/SiuHinTang/WeekendBot/actions/workflows/dotnet.yml/badge.svg)
[![codecov](https://codecov.io/gh/SiuHinTang/WeekendBot/branch/master/graph/badge.svg?token=20HZTP4M1O)](https://codecov.io/gh/SiuHinTang/WeekendBot)


# WeekendBot

> Saturday and Sunday, or Friday evening until Sunday night:
> 
> **Source:** [Cambridge Dictionary](https://dictionary.cambridge.org/dictionary/english/weekend)

A simple bot that keeps track of "Weekend" (hereafter defined as Fridays, past 4pm) related items:

* A short message indicating whether it is weekend (or not)
* A message to indicate how much time before it is weekend (unless it already is weekend :wink:)

# Setup
In order to run the Bot, a `config.json` file must be created next to the executable with the following content. Note that only the key `Token` is mandatory. The remaining object literals and their attributes are all optional, unless specified otherwise.

```json
{
    "Token": "{Your Discord token for the Bot}",
    "StringFormatOptions":
    {
        "FloatingNumberFormat": "F2",
        "TimeSpanFormat": "d\\.hh\\:mm\\:ss",
        "DateTimeFormat": "dddd d/MM/yy HH:mm:ss"
    },
    "CommandOptions":
    {
        "TestGuildId": "{TestGuildId}"
    }
}
```

## StringFormatOptions (optional)
| Key | Description |
|---|---|
| FloatingNumberFormat | The number format to use for floating numbers, see also [C# Standard Numeric Formats](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings) |
| TimeSpanFormat | The format to display time span information, see also [C# custom time span formats](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings) |
| DateTimeFormat | The format to display date time information, see also [C# custom date time format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)

## CommandOptions (optional)


# Supported commands

* `help`

   Provides a summary about all available commands in the bot.
   
* `weekend?`

   Gets a response about whether it is weekend yet.
   
* `timetoweekend?`

   Gets a response between the time of invoking the command and Friday 4pm of the current week. 
   
   _Note_ This will result in a default response when the command is invoked between Friday 4pm and Monday 12am.

# License

This project was released under the GPLv3 license. A copy of this license can be found [here](/licenses).

# Goals

The main goal of this application is to familiarize with the following frameworks:

* EF Core 6 and AutoMapper
* [Discord.Net](https://github.com/discord-net/Discord.Net)
* xUnit 
