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
using System.Threading.Tasks;
using Discord.Common.Options;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using WeekendBot.Utils;

namespace Discord.Common.Handlers;

/// <summary>
/// The command handler that uses the Discord interaction framework.
/// </summary>
public class InteractionDiscordCommandHandler : DiscordCommandHandlerBase
{
    private readonly InteractionService interactionService;

    /// <inheritdoc />
    /// <summary>
    /// Creates a new instance of <see cref="InteractionDiscordCommandHandler"/>.
    /// </summary>
    /// <param name="interactionService">The <see cref="InteractionService"/>.</param>
    public InteractionDiscordCommandHandler(
        IServiceProvider services, DiscordSocketClient client,
        IOptions<DiscordCommandOptions> options, ILoggingService logger, InteractionService interactionService)
        : base(services, client, options, logger)
    {
        interactionService.IsNotNull(nameof(interactionService));

        this.interactionService = interactionService;
        interactionService.Log += async message => await OnLogEventHandler(message);

        Client.InteractionCreated += async arg => await InteractionCreatedEventHandler(arg);

        AddModuleFunc = (provider, type) => interactionService.AddModuleAsync(type, provider);
    }

    protected override Func<IServiceProvider, Type, Task> AddModuleFunc { get; }

    protected override void PostProcessInitialization()
    {
        Client.Ready += ReadyEventHandler;
    }

    private async Task ReadyEventHandler()
    {
        // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
        // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
#if DEBUG
        await interactionService.RegisterCommandsToGuildAsync(CommandOptions.TestGuildId);
#else
        await _handler.RegisterCommandsGloballyAsync(true);
#endif
    }

    private async Task InteractionCreatedEventHandler(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(Client, interaction);

            // Execute the incoming command.
            IResult? result = await interactionService.ExecuteCommandAsync(context, Services);
            if (!result.IsSuccess)
            {
                var errorMessage = $"Command failed: {result.ErrorReason}";
                await context.Channel.SendMessageAsync(errorMessage);
                await Logger.LogErrorAsync(errorMessage);
            }
        }
        catch (Exception e)
        {
            await Logger.LogErrorAsync(e.Message);

            string? stackTrace = e.StackTrace;
            if (!string.IsNullOrWhiteSpace(stackTrace))
            {
                await Logger.LogErrorAsync(stackTrace);
            }

            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }
    }
}