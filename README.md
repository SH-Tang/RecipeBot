[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SH-Tang_RecipeBot&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SH-Tang_RecipeBot)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SH-Tang_RecipeBot&metric=coverage)](https://sonarcloud.io/summary/new_code?id=SH-Tang_RecipeBot)
![GitHub last commit](https://img.shields.io/github/last-commit/SH-Tang/RecipeBot)
[![.NET](https://github.com/SH-Tang/RecipeBot/actions/workflows/dotnet.yml/badge.svg)](https://github.com/SH-Tang/RecipeBot/actions/workflows/dotnet.yml)

# RecipeBot

![Alt text](docs/ReadMe/RecipeCommand.gif?raw=true "Recipe Command within Discord")

The RecipeBot is intended to provide the following functionality in Discord as a bot:

* Formatting recipes according to a standardised format
* Persistence of recipes to allow retrieval based on tags or the type of dish
* Formatting annotated web recipes according to a standardised format

# Realised features
At the moment of writing the bot is capable of:
* Formatting user recipes according to a standard format
* Adding tags and category for a recipe
* Persisting and retrieving recipes

For more information about the commands within the bot, see the [Wiki](https://github.com/SH-Tang/RecipeBot/wiki).

# Deployment
In order to run the RecipeBot, a `config.json` file must be created next to the executable with the following content. The minimum content is listed below:

```json
{
    "Token": "{Your Discord token for the Bot}",
    "ConnectionStrings":
    {
        "DefaultConnection": "Data Source={File path to the SQLite database}"
    }
}
```
## Token (mandatory)
This is the Discord token as provided by the Discord Developer portal.

## ConnectionString (mandatory)
| Key | Description |
|---|---|
| DefaultConnection | Set this to the path of the SQLite database to store and retrieve the data from. In case the database does not exist, the database is created by the application. |

# Used libraries

## Functionality
* Discord.Net
* ReHackt.Extensions.Options.Validations
* Microsoft.Entity.Framework.Core

## Testing purposes
* xUnit 
* xUnit.runner.visualstudio
* NSubstitute
* Microsoft.NET.Test.Sdk
* AutoFixture
* coverlet.collector
* Fluent Assertions

# License

This project was released under the GPLv3 license. A copy of this license can be found [here](https://www.gnu.org/licenses/gpl-3.0.html).
