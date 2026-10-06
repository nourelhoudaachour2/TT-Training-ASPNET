using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_tunisie_telecome.Migrations
{
    /// <inheritdoc />
    public partial class AddJustification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "JustificationUploadedAt",
                table: "SessionParticipants",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JustificationUploadedAt",
                table: "SessionParticipants");
        }
    }
}
