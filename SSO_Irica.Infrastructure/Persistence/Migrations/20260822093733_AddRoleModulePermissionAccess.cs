using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SSO_Irica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleModulePermissionAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_Modules",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Modules", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Permissions",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Permissions", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Role_Access",
                columns: table => new
                {
                    Fld_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Role_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Module_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Permission_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Network = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Role_Access", x => x.Fld_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Modules_Fld_Code",
                table: "Tbl_Modules",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Permissions_Fld_Code",
                table: "Tbl_Permissions",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Role_Access_Fld_Role_Id_Fld_Module_Id_Fld_Permission_Id~",
                table: "Tbl_Role_Access",
                columns: new[] { "Fld_Role_Id", "Fld_Module_Id", "Fld_Permission_Id", "Fld_Network" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_Modules");

            migrationBuilder.DropTable(
                name: "Tbl_Permissions");

            migrationBuilder.DropTable(
                name: "Tbl_Role_Access");
        }
    }
}
