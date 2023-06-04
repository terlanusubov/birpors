using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Birpors.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.AddColumn<bool>(
                name: "IsSupport",
                table: "Conversations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "IsSupport",
                table: "Conversations");
        }
    }
}
