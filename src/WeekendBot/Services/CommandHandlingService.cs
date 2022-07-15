using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using WeekendBot.Utils;

namespace WeekendBot.Services
{
    /// <summary>
    /// The service to deal with Discord commands.
    /// </summary>
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IServiceProvider services;

        /// <summary>
        /// Creates a new instance of <see cref="CommandHandlingService"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceProvider"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public CommandHandlingService(IServiceProvider services)
        {
            services.IsNotNull(nameof(services));

            this.services = services;
            commands = services.GetRequiredService<CommandService>();

            client = services.GetRequiredService<DiscordSocketClient>();
            client.MessageReceived += HandleCommandAsync;
        }

        public async Task InitializeServiceAsync()
        {
            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
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
            if (!(message.HasCharPrefix('~', ref argPos) ||
                  message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await commands.ExecuteAsync(context, argPos, services);
        }
    }
}