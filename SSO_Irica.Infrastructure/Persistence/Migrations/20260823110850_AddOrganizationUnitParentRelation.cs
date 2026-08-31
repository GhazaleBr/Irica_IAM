using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationUnitParentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Organization_Unit_Fld_Parent_Id",
                table: "Tbl_Organization_Unit",
                column: "Fld_Parent_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Organization_Unit_Tbl_Organization_Unit_Fld_Parent_Id",
                table: "Tbl_Organization_Unit",
                column: "Fld_Parent_Id",
                principalTable: "Tbl_Organization_Unit",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Organization_Unit_Tbl_Organization_Unit_Fld_Parent_Id",
                table: "Tbl_Organization_Unit");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Organization_Unit_Fld_Parent_Id",
                table: "Tbl_Organization_Unit");
        }
    }
}
