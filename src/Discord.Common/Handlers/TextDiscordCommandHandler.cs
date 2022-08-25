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
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Common.Options;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using WeekendBot.Utils;

namespace Discord.Common.Handlers;

/// <summary>
/// The text handler to deal with Discord commands.
/// </summary>
public class TextDiscordCommandHandler : DiscordCommandHandlerBase
{
    private readonly CommandService commandService;

    /// <inheritdoc />
    /// <summary>
    /// Creates a new instance of <see cref="TextDiscordCommandHandler"/>.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/>.</param>
    public TextDiscordCommandHandler(
        IServiceProvider services, DiscordSocketClient client,
        IOptions<DiscordCommandOptions> options, ILoggingService logger, CommandService commandService)
        : base(services, client, options, logger)
    {
        commandService.IsNotNull(nameof(commandService));

        this.commandService = commandService;
        commandService.CommandExecuted += CommandExecutedEventHandler;
        commandService.Log += async arg => await LogEventHandler(arg);

        Client.MessageReceived += MessageReceivedEventHandler;
    }

    protected override Func<Type, IServiceProvider, Task> AddModuleFunc => commandService.AddModuleAsync;

    private async Task MessageReceivedEventHandler(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null)
        {
            return;
        }

        // Create a number to track where the prefix ends and the command begins
        var argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix(CommandOptions.CommandPrefix, ref argPos)
              || message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            || message.Author.IsBot)
        {
            return;
        }

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(Client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await commandService.ExecuteAsync(context, argPos, Services);
    }

    private async Task CommandExecutedEventHandler(
        Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
    {
        if (!commandInfo.IsSpecified || result.IsSuccess)
        {
            return;
        }

        var errorMessage = $"Command {commandInfo.Value.Name} failed: {result.ErrorReason}";
        await commandContext.Channel.SendMessageAsync(errorMessage);
        await Logger.LogErrorAsync(errorMessage);
    }
}