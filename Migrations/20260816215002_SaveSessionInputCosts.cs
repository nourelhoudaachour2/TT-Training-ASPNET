using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_tunisie_telecome.Migrations
{
    /// <inheritdoc />
    public partial class SaveSessionInputCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HotelNightPrice",
                table: "SessionFormations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NotebookPrice",
                table: "SessionFormations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PenPrice",
                table: "SessionFormations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestaurantTicketPrice",
                table: "SessionFormations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ToteBagPrice",
                table: "SessionFormations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelNightPrice",
                table: "SessionFormations");

            migrationBuilder.DropColumn(
                name: "NotebookPrice",
                table: "SessionFormations");

            migrationBuilder.DropColumn(
                name: "PenPrice",
                table: "SessionFormations");

            migrationBuilder.DropColumn(
                name: "RestaurantTicketPrice",
                table: "SessionFormations");

            migrationBuilder.DropColumn(
                name: "ToteBagPrice",
                table: "SessionFormations");
        }
    }
}
