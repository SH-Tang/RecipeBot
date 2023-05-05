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
/// Entity class to persist author related data.
/// </summary>
[Index(nameof(AuthorEntityId), IsUnique = true)]
public class AuthorEntity
{
    [Key]
    public long AuthorEntityId { get; set; }

    [Required]
    public string AuthorId { get; set; } = null!;

    public ICollection<RecipeEntity> Recipes { get; set; } = null!;
}