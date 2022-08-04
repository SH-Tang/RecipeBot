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
using Discord;
using Discord.Common;
using Discord.Interactions;
using Discord.WebSocket;

namespace WeekendBot.Modules;

public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILoggingService logger;
    private static IAttachment? attachmentArgument;

    public SlashCommandModule(ILoggingService logger)
    {
        this.logger = logger;
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task GreetUserAsync()
    {
        await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);
    }

    [SlashCommand("recipe", "Tell us about your recipe")]
    public async Task FoodPreference([Summary("image", "The image of the recipe result")] IAttachment attachment)
    {
        attachmentArgument = attachment;
        // ModalBuilder? modalBuilder = new ModalBuilder()
        //                              .WithTitle("Recipe entry")
        //                              .WithCustomId("food_menu")
        //                              .AddTextInput("RecipeTitle", "title")
        //                              .AddTextInput("Notes", "notes", TextInputStyle.Paragraph,
        //                                            "Insert optional notes here", required: false);
        try
        {
            await Context.Interaction.RespondWithModalAsync<RecipeModal>("recipe");
        }
        catch
        {
            attachmentArgument = null;
        }
    }

    // Responds to the modal.
    [ModalInteraction("recipe")]
    public async Task ModalResponse(RecipeModal modal)
    {
        // Build the message to send.
        // string message = $"Passed modal information:{Environment.NewLine}" +
        //                  $"Recipe title: {modal.RecipeTitle}{Environment.NewLine}" +
        //                  $"Notes: {modal.Notes}{Environment.NewLine}";

        // Respond to the modal, this is required or the Modal freezes up. Note that RespondAsync is the only
        // allowable respond method for modals.
        SocketUser? user = Context.User;

        EmbedBuilder? embedBuilder = new EmbedBuilder()
                                     .WithTitle(modal.RecipeTitle)
                                     .WithAuthor(user.Username, user.GetAvatarUrl())
                                     .WithColor(Color.Green)
                                     .WithImageUrl(attachmentArgument.Url)
                                     // .AddField("Is Image?", attachmentArgument.ContentType.StartsWith("image/"))
                                     // .AddField("Image url", attachmentArgument.Url)
                                     // .AddField("Image size", attachmentArgument.Size)
                                     .AddField("Ingredienten", modal.Ingredients)
                                     .AddField("Stappen", modal.Steps)
                                     .WithCurrentTimestamp();

        // if (!string.IsNullOrWhiteSpace(modal.Notes))
        // {
        //     embedBuilder.AddField("Aantekeningen", modal.Notes);
        // }


        // await RespondAsync(embed: embedBuiler.Build());

        await RespondAsync(embed: embedBuilder.Build());
        attachmentArgument = null;
    }

    public class RecipeModal : IModal
    {
        [ModalTextInput("title")]
        [InputLabel("Title")]
        public string RecipeTitle { get; set; }

        [InputLabel("Ingredienten")]
        [ModalTextInput("ingredients", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength)]
        public string Ingredients { get; set; }

        [InputLabel("Stappen")]
        [ModalTextInput("steps", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength)]
        public string Steps { get; set; }

        // [InputLabel("Aantekeningen")]
        // [ModalTextInput("notes", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength)]
        // [RequiredInput(false)]
        // public string Notes { get; set; }

        public string Title => "Recipe";
    }
}