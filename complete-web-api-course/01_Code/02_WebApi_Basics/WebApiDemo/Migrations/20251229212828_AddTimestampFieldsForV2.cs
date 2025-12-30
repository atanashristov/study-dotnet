using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampFieldsForV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Shirts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Shirts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Shirts");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Shirts");
        }
    }
}
