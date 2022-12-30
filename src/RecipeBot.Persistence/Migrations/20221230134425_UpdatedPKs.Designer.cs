﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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

// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeBot.Persistence;

#nullable disable

namespace RecipeBot.Persistence.Migrations
{
    [DbContext(typeof(RecipeBotDbContext))]
    [Migration("20221230134425_UpdatedPKs")]
    partial class UpdatedPKs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("RecipeBot.Persistence.Entities.AuthorEntity", b =>
                {
                    b.Property<int>("AuthorEntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorEntityId");

                    b.HasIndex("AuthorEntityId")
                        .IsUnique();

                    b.ToTable("AuthorEntities");
                });

            modelBuilder.Entity("RecipeBot.Persistence.Entities.RecipeEntity", b =>
                {
                    b.Property<int>("RecipeEntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("RecipeEntityId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("RecipeEntityId")
                        .IsUnique();

                    b.ToTable("RecipeEntities");
                });

            modelBuilder.Entity("RecipeBot.Persistence.Entities.RecipeEntity", b =>
                {
                    b.HasOne("RecipeBot.Persistence.Entities.AuthorEntity", "Author")
                        .WithMany("Recipes")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("RecipeBot.Persistence.Entities.AuthorEntity", b =>
                {
                    b.Navigation("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}