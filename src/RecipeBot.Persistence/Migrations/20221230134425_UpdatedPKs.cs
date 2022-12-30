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

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RecipeEntities",
                newName: "RecipeEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeEntities_Id",
                table: "RecipeEntities",
                newName: "IX_RecipeEntities_RecipeEntityId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AuthorEntities",
                newName: "AuthorEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorEntities_Id",
                table: "AuthorEntities",
                newName: "IX_AuthorEntities_AuthorEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipeEntityId",
                table: "RecipeEntities",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeEntities_RecipeEntityId",
                table: "RecipeEntities",
                newName: "IX_RecipeEntities_Id");

            migrationBuilder.RenameColumn(
                name: "AuthorEntityId",
                table: "AuthorEntities",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorEntities_AuthorEntityId",
                table: "AuthorEntities",
                newName: "IX_AuthorEntities_Id");
        }
    }
}
