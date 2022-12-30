using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeEntities_AuthorEntities_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AuthorEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorEntities_Id",
                table: "AuthorEntities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeEntities_AuthorId",
                table: "RecipeEntities",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeEntities_Id",
                table: "RecipeEntities",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeEntities");

            migrationBuilder.DropTable(
                name: "AuthorEntities");
        }
    }
}
