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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RecipeBot.Persistence.Entities;

/// <summary>
/// Entity class to persist recipe related data.
/// </summary>
[Index(nameof(RecipeEntityId), IsUnique = true)]
public class RecipeEntity
{
    [Key]
    public long RecipeEntityId { get; set; }

    [Required]
    public string RecipeTitle { get; set; } = null!;

    [Required]
    public PersistentRecipeCategory RecipeCategory { get; set; }

    [Required]
    public AuthorEntity Author { get; set; } = null!;

    public ICollection<RecipeFieldEntity> RecipeFields { get; set; } = null!;

    public ICollection<RecipeTagEntity> Tags { get; set; } = null!;
}