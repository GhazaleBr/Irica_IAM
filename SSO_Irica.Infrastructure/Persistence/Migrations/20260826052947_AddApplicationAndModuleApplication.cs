using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IAM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationAndModuleApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_Application",
                columns: table => new
                {
                    Fld_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fld_Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fld_Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fld_IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Application", x => x.Fld_Id);
                });

            migrationBuilder.InsertData(
                table: "Tbl_Application",
                columns: new[] { "Fld_Id", "Fld_Code", "Fld_Title", "Fld_IsActive" },
                values: new object[] { 1, "IAM", "IAM", true });

            migrationBuilder.AddColumn<int>(
                name: "Fld_Application_Id",
                table: "Tbl_Modules",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Tbl_Modules"
                SET "Fld_Application_Id" = 1
                WHERE "Fld_Application_Id" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Fld_Application_Id",
                table: "Tbl_Modules",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Modules_Fld_Application_Id",
                table: "Tbl_Modules",
                column: "Fld_Application_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Application_Fld_Code",
                table: "Tbl_Application",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Modules_Tbl_Application_Fld_Application_Id",
                table: "Tbl_Modules",
                column: "Fld_Application_Id",
                principalTable: "Tbl_Application",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Modules_Tbl_Application_Fld_Application_Id",
                table: "Tbl_Modules");

            migrationBuilder.DropTable(
                name: "Tbl_Application");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Modules_Fld_Application_Id",
                table: "Tbl_Modules");

            migrationBuilder.DropColumn(
                name: "Fld_Application_Id",
                table: "Tbl_Modules");
        }
    }
}
