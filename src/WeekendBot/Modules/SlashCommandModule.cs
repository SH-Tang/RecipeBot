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

using System.Threading.Tasks;
using Discord;
using Discord.Common;
using Discord.Interactions;

namespace WeekendBot.Modules;

public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILoggingService logger;

    public SlashCommandModule(ILoggingService logger)
    {
        this.logger = logger;
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task GreetUserAsync()
    {
        await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);
    }

    [SlashCommand("food", "Tell us about your favorite food!")]
    public async Task FoodPreference()
    {
        await Context.Interaction.RespondWithModalAsync<FoodModal>("food_menu");
    }

    // Responds to the modal.
    [ModalInteraction("food_menu")]
    public async Task ModalResponse(FoodModal modal)
    {
        // Build the message to send.
        string message = "hey @everyone, I just learned " +
                         $"{Context.User.Mention}'s favorite food is " +
                         $"{modal.Food} because {modal.Reason}.";

        // Specify the AllowedMentions so we don't actually ping everyone.
        var mentions = new AllowedMentions()
        {
            AllowedTypes = AllowedMentionTypes.Users
        };

        // Respond to the modal, this is required or the Modal freezes up. Note that RespondAsync is the only
        // allowable respond method for modals.
        await RespondAsync();
    }

    public class FoodModal : IModal
    {
        // Strings with the ModalTextInput attribute will automatically become components.
        [InputLabel("What??")]
        [ModalTextInput("food_name", placeholder: "Pizza", maxLength: 20)]
        public string Food { get; set; }

        // Additional paremeters can be specified to further customize the input.
        [InputLabel("Why??")]
        [ModalTextInput("food_reason", TextInputStyle.Paragraph, "Kuz it's tasty", maxLength: 500, initValue:"Soup")]
        public string Reason { get; set; }

        public string Title => "Fav Food";
    }
}