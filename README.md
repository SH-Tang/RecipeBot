![Unit tests](https://github.com/SiuHinTang/WeekendBot/actions/workflows/dotnet.yml/badge.svg)

# WeekendBot

> Saturday and Sunday, or Friday evening until Sunday night:
> 
> **Source:** [Cambridge Dictionary](https://dictionary.cambridge.org/dictionary/english/weekend)

A simple bot that keeps track of "Weekend" (hereafter defined as Fridays, past 4pm) related items:

* A short message indicating whether it is weekend (or not)
* A message to indicate how much time before it is weekend (unless it already is weekend :wink:)

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
