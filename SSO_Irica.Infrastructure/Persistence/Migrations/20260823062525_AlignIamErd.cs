using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IAM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignIamErd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sso_Refresh_Tokens");

            migrationBuilder.DropTable(
                name: "Tbl_Role_Access");

            migrationBuilder.AddColumn<int>(
                name: "Fld_Gender",
                table: "Tbl_User",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tbl_Actions_Log",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Entity_Id = table.Column<long>(type: "bigint", nullable: false),
                    Fld_Modules_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Actions_Log", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Gender",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Code = table.Column<int>(type: "integer", nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Gender", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Organization_Type",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Code = table.Column<int>(type: "integer", nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Organization_Type", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Organization_Unit",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Parent_Id = table.Column<int>(type: "integer", nullable: true),
                    Fld_Code = table.Column<int>(type: "integer", nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Fld_Type_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Organization_Unit", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Positions",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Code = table.Column<int>(type: "integer", nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Fld_OrgUnit_Id = table.Column<int>(name: "Fld_Org/Unit_Id", type: "integer", nullable: false),
                    Fld_Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Positions", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Role_Permissions",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Role_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Module_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Permision_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Role_Permissions", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_User_Positions",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fld_Position_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Fld_StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fld_EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_User_Positions", x => x.Fld_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Organization_Type_Fld_Code",
                table: "Tbl_Organization_Type",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Organization_Unit_Fld_Code",
                table: "Tbl_Organization_Unit",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Positions_Fld_Code",
                table: "Tbl_Positions",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Role_Permissions_Fld_Role_Id_Fld_Module_Id_Fld_Permisio~",
                table: "Tbl_Role_Permissions",
                columns: new[] { "Fld_Role_Id", "Fld_Module_Id", "Fld_Permision_Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_Actions_Log");

            migrationBuilder.DropTable(
                name: "Tbl_Gender");

            migrationBuilder.DropTable(
                name: "Tbl_Organization_Type");

            migrationBuilder.DropTable(
                name: "Tbl_Organization_Unit");

            migrationBuilder.DropTable(
                name: "Tbl_Positions");

            migrationBuilder.DropTable(
                name: "Tbl_Role_Permissions");

            migrationBuilder.DropTable(
                name: "Tbl_User_Positions");

            migrationBuilder.DropColumn(
                name: "Fld_Gender",
                table: "Tbl_User");

            migrationBuilder.CreateTable(
                name: "Sso_Refresh_Tokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReplacedByHash = table.Column<string>(type: "text", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sso_Refresh_Tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Role_Access",
                columns: table => new
                {
                    Fld_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Fld_Module_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Network = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Fld_Permission_Id = table.Column<int>(type: "integer", nullable: false),
                    Fld_Role_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Role_Access", x => x.Fld_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sso_Refresh_Tokens_TokenHash",
                table: "Sso_Refresh_Tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sso_Refresh_Tokens_UserId_RevokedAt",
                table: "Sso_Refresh_Tokens",
                columns: new[] { "UserId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Role_Access_Fld_Role_Id_Fld_Module_Id_Fld_Permission_Id~",
                table: "Tbl_Role_Access",
                columns: new[] { "Fld_Role_Id", "Fld_Module_Id", "Fld_Permission_Id", "Fld_Network" },
                unique: true);
        }
    }
}
