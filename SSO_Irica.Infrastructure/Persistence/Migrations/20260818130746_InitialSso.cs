using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SSO_Irica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_Roles",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Roles", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_User",
                columns: table => new
                {
                    Fld_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fld_UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fld_Password = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Fld_FirstName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Fld_LastName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Fld_Mobile = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Fld_Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_User", x => x.Fld_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_User_Roles",
                columns: table => new
                {
                    Fld_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fld_Role_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_User_Roles", x => x.Fld_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_User_Fld_UserName",
                table: "Tbl_User",
                column: "Fld_UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_Roles");

            migrationBuilder.DropTable(
                name: "Tbl_User");

            migrationBuilder.DropTable(
                name: "Tbl_User_Roles");
        }
    }
}
