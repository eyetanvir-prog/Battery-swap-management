using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatterySwap.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "balance_recharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AddedByAdminId = table.Column<int>(type: "int", nullable: true),
                    AddedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    RechargeTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balance_recharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_balance_recharges_admins_AddedByAdminId",
                        column: x => x.AddedByAdminId,
                        principalTable: "admins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "batteries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatteryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CurrentClientId = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batteries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 1000m),
                    CurrentBatteryId = table.Column<int>(type: "int", nullable: true),
                    CreatedByAdminId = table.Column<int>(type: "int", nullable: true),
                    CreatedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_clients_admins_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "admins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_clients_batteries_CurrentBatteryId",
                        column: x => x.CurrentBatteryId,
                        principalTable: "batteries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StationId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    JoiningDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StationCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stations_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "swaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    ReturnedBatteryId = table.Column<int>(type: "int", nullable: false),
                    AssignedBatteryId = table.Column<int>(type: "int", nullable: false),
                    SwapCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 140m),
                    SwapTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ProcessedByEmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_swaps_batteries_AssignedBatteryId",
                        column: x => x.AssignedBatteryId,
                        principalTable: "batteries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_swaps_batteries_ReturnedBatteryId",
                        column: x => x.ReturnedBatteryId,
                        principalTable: "batteries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_swaps_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_swaps_employees_ProcessedByEmployeeId",
                        column: x => x.ProcessedByEmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_swaps_stations_StationId",
                        column: x => x.StationId,
                        principalTable: "stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_admins_Email",
                table: "admins",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admins_Phone",
                table: "admins",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_balance_recharges_AddedByAdminId",
                table: "balance_recharges",
                column: "AddedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_balance_recharges_AddedByEmployeeId",
                table: "balance_recharges",
                column: "AddedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_balance_recharges_ClientId",
                table: "balance_recharges",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_balance_recharges_StationId",
                table: "balance_recharges",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_batteries_BatteryCode",
                table: "batteries",
                column: "BatteryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_batteries_CurrentClientId",
                table: "batteries",
                column: "CurrentClientId",
                unique: true,
                filter: "[CurrentClientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_batteries_StationId",
                table: "batteries",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_clients_CreatedByAdminId",
                table: "clients",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_clients_CreatedByEmployeeId",
                table: "clients",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_clients_CurrentBatteryId",
                table: "clients",
                column: "CurrentBatteryId",
                unique: true,
                filter: "[CurrentBatteryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_clients_Nid",
                table: "clients",
                column: "Nid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_Phone",
                table: "clients",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_Nid",
                table: "employees",
                column: "Nid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_Phone",
                table: "employees",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_StationId",
                table: "employees",
                column: "StationId",
                unique: true,
                filter: "[StationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_employees_Username",
                table: "employees",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stations_EmployeeId",
                table: "stations",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_stations_StationCode",
                table: "stations",
                column: "StationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_swaps_AssignedBatteryId",
                table: "swaps",
                column: "AssignedBatteryId");

            migrationBuilder.CreateIndex(
                name: "IX_swaps_ClientId",
                table: "swaps",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_swaps_ProcessedByEmployeeId",
                table: "swaps",
                column: "ProcessedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_swaps_ReturnedBatteryId",
                table: "swaps",
                column: "ReturnedBatteryId");

            migrationBuilder.CreateIndex(
                name: "IX_swaps_StationId",
                table: "swaps",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_balance_recharges_clients_ClientId",
                table: "balance_recharges",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_balance_recharges_employees_AddedByEmployeeId",
                table: "balance_recharges",
                column: "AddedByEmployeeId",
                principalTable: "employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_balance_recharges_stations_StationId",
                table: "balance_recharges",
                column: "StationId",
                principalTable: "stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_batteries_clients_CurrentClientId",
                table: "batteries",
                column: "CurrentClientId",
                principalTable: "clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_batteries_stations_StationId",
                table: "batteries",
                column: "StationId",
                principalTable: "stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_employees_CreatedByEmployeeId",
                table: "clients",
                column: "CreatedByEmployeeId",
                principalTable: "employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_stations_StationId",
                table: "employees",
                column: "StationId",
                principalTable: "stations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clients_admins_CreatedByAdminId",
                table: "clients");

            migrationBuilder.DropForeignKey(
                name: "FK_batteries_clients_CurrentClientId",
                table: "batteries");

            migrationBuilder.DropForeignKey(
                name: "FK_stations_employees_EmployeeId",
                table: "stations");

            migrationBuilder.DropTable(
                name: "balance_recharges");

            migrationBuilder.DropTable(
                name: "swaps");

            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "batteries");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "stations");
        }
    }
}
