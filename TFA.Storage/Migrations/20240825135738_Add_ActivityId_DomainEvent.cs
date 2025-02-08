using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFA.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Add_ActivityId_DomainEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityId",
                table: "DomainEvents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "DomainEvents");
        }
    }
}
