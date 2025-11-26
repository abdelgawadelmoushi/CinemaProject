using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaProject.Migrations
{
    /// <inheritdoc />
    public partial class correctingsubImges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieSubImages_Movies_MovieId",
                table: "MovieSubImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSubImages",
                table: "MovieSubImages");

            migrationBuilder.RenameTable(
                name: "MovieSubImages",
                newName: "MovieSubImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSubImages",
                table: "MovieSubImages",
                columns: new[] { "MovieId", "Img" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSubImages_Movies_MovieId",
                table: "MovieSubImages",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieSubImages_Movies_MovieId",
                table: "MovieSubImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSubImages",
                table: "MovieSubImages");

            migrationBuilder.RenameTable(
                name: "MovieSubImages",
                newName: "MovieSubImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSubImages",
                table: "MovieSubImages",
                columns: new[] { "MovieId", "Img" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSubImages_Movies_MovieId",
                table: "MovieSubImages",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
