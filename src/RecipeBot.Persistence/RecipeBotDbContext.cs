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

using Microsoft.EntityFrameworkCore;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence;

/// <summary>
/// Context for providing database access of the RecipeBot.
/// </summary>
public class RecipeBotDbContext : DbContext
{
    public RecipeBotDbContext(DbContextOptions options) : base(options) {}

    public DbSet<AuthorEntity> AuthorEntities { get; set; } = null!;

    public DbSet<RecipeEntity> RecipeEntities { get; set; } = null!;

    public DbSet<RecipeFieldEntity> RecipeFieldEntities { get; set; } = null!;

    public DbSet<RecipeTagEntity> RecipeTagEntities { get; set; } = null!;

    public DbSet<TagEntity> TagEntities { get; set; } = null!;
}