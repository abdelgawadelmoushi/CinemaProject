//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace CinemaProject.Migrations
//{
//    /// <inheritdoc />
//    public partial class updatemodels : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropForeignKey(
//                name: "FK_MovieSubImages_Movies_MovieId",
//                table: "MovieSubImages");

//            migrationBuilder.DropPrimaryKey(
//                name: "PK_MovieSubImages",
//                table: "MovieSubImages");

//            migrationBuilder.DropIndex(
//                name: "IX_MovieSubImages_MovieId",
//                table: "MovieSubImages");

//            migrationBuilder.AlterColumn<int>(
//                name: "MovieId",
//                table: "MovieSubImages",
//                type: "int",
//                nullable: false,
//                oldClrType: typeof(int),
//                oldType: "int")
//                .Annotation("SqlServer:Identity", "1, 1");

//            migrationBuilder.AlterColumn<int>(
//                name: "Id",
//                table: "MovieSubImages",
//                type: "int",
//                nullable: false,
//                oldClrType: typeof(int),
//                oldType: "int")
//                .OldAnnotation("SqlServer:Identity", "1, 1");

//            migrationBuilder.AddColumn<int>(
//                name: "MovieId1",
//                table: "MovieSubImages",
//                type: "int",
//                nullable: false,
//                defaultValue: 0);

//            migrationBuilder.AddPrimaryKey(
//                name: "PK_MovieSubImages",
//                table: "MovieSubImages",
//                column: "MovieId");

//            migrationBuilder.CreateIndex(
//                name: "IX_MovieSubImages_MovieId1",
//                table: "MovieSubImages",
//                column: "MovieId1");

//            migrationBuilder.AddForeignKey(
//                name: "FK_MovieSubImages_Movies_MovieId1",
//                table: "MovieSubImages",
//                column: "MovieId1",
//                principalTable: "Movies",
//                principalColumn: "Id",
//                onDelete: ReferentialAction.Cascade);
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropForeignKey(
//                name: "FK_MovieSubImages_Movies_MovieId1",
//                table: "MovieSubImages");

//            migrationBuilder.DropPrimaryKey(
//                name: "PK_MovieSubImages",
//                table: "MovieSubImages");

//            migrationBuilder.DropIndex(
//                name: "IX_MovieSubImages_MovieId1",
//                table: "MovieSubImages");

//            migrationBuilder.DropColumn(
//                name: "MovieId1",
//                table: "MovieSubImages");

//            migrationBuilder.AlterColumn<int>(
//                name: "Id",
//                table: "MovieSubImages",
//                type: "int",
//                nullable: false,
//                oldClrType: typeof(int),
//                oldType: "int")
//                .Annotation("SqlServer:Identity", "1, 1");

//            migrationBuilder.AlterColumn<int>(
//                name: "MovieId",
//                table: "MovieSubImages",
//                type: "int",
//                nullable: false,
//                oldClrType: typeof(int),
//                oldType: "int")
//                .OldAnnotation("SqlServer:Identity", "1, 1");

//            migrationBuilder.AddPrimaryKey(
//                name: "PK_MovieSubImages",
//                table: "MovieSubImages",
//                column: "Id");

//            migrationBuilder.CreateIndex(
//                name: "IX_MovieSubImages_MovieId",
//                table: "MovieSubImages",
//                column: "MovieId");

//            migrationBuilder.AddForeignKey(
//                name: "FK_MovieSubImages_Movies_MovieId",
//                table: "MovieSubImages",
//                column: "MovieId",
//                principalTable: "Movies",
//                principalColumn: "Id",
//                onDelete: ReferentialAction.Cascade);
//        }
//    }
//}
