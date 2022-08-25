![License](https://img.shields.io/github/license/SiuHinTang/WeekendBot)
![GitHub last commit](https://img.shields.io/github/last-commit/SiuHinTang/WeekendBot)
![Unit tests](https://github.com/SiuHinTang/WeekendBot/actions/workflows/dotnet.yml/badge.svg)
[![codecov](https://codecov.io/gh/SiuHinTang/WeekendBot/branch/master/graph/badge.svg?token=20HZTP4M1O)](https://codecov.io/gh/SiuHinTang/WeekendBot)


# RecipeBot

![Alt text](docs/documentation/RecipeCommand.gif?raw=true "Recipe Command within Discord")

The RecipeBot is intended to provide the following functionality in Discord as a bot:

* Formatting recipes according to a standardised format
* Persistence of recipes to allow retrieval based on tags or the type of dish
* Formatting annotated web recipes according to a standardised format

# Realised features
At the moment of writing the bot is capable of:
* Formatting user recipes according to a standard format

# Supported bot commands

* `help`

   Provides a summary about all available commands in the bot.
   
* `recipe`

   Spawns a modal with standard fields to allow the user to enter a recipe. The command can be invoked with or without an image of a recipe.

# Deployment
In order to run the RecipeBot, a `config.json` file must be created next to the executable with the following content. Note that only the key `Token` is mandatory. The remaining object literals and their attributes are all optional, unless specified otherwise.

```json
{
    "Token": "{Your Discord token for the Bot}",
    "CommandOptions":
    {
        "TestGuildId": "{TestGuildId}"
    }
}
```

## CommandOptions (optional)
| Key | Description |
|---|---|
| TestGuildId | Set this to a Discord channel to immediately test the bot and its slash commands. Global commands take an hour to register |



# Used libraries

* [Discord.Net](https://github.com/discord-net/Discord.Net)
* xUnit 
* NSubstitute

# License

This project was released under the GPLv3 license. A copy of this license can be found [here](/licenses).
