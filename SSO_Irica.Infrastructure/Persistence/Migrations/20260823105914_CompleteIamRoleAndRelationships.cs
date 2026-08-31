using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteIamRoleAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Fld_Code",
                table: "Tbl_Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Fld_Description",
                table: "Tbl_Roles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Fld_IsActive",
                table: "Tbl_Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_User_Roles_Fld_Role_Id",
                table: "Tbl_User_Roles",
                column: "Fld_Role_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_User_Roles_Fld_User_Id_Fld_Role_Id",
                table: "Tbl_User_Roles",
                columns: new[] { "Fld_User_Id", "Fld_Role_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_User_Positions_Fld_Position_Id",
                table: "Tbl_User_Positions",
                column: "Fld_Position_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_User_Positions_Fld_User_Id",
                table: "Tbl_User_Positions",
                column: "Fld_User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Roles_Fld_Code",
                table: "Tbl_Roles",
                column: "Fld_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Role_Permissions_Fld_Module_Id",
                table: "Tbl_Role_Permissions",
                column: "Fld_Module_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Role_Permissions_Fld_Permision_Id",
                table: "Tbl_Role_Permissions",
                column: "Fld_Permision_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Positions_Fld_Org/Unit_Id",
                table: "Tbl_Positions",
                column: "Fld_Org/Unit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Organization_Unit_Fld_Type_Id",
                table: "Tbl_Organization_Unit",
                column: "Fld_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Actions_Log_Fld_Modules_Id",
                table: "Tbl_Actions_Log",
                column: "Fld_Modules_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Actions_Log_Tbl_Modules_Fld_Modules_Id",
                table: "Tbl_Actions_Log",
                column: "Fld_Modules_Id",
                principalTable: "Tbl_Modules",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Organization_Unit_Tbl_Organization_Type_Fld_Type_Id",
                table: "Tbl_Organization_Unit",
                column: "Fld_Type_Id",
                principalTable: "Tbl_Organization_Type",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Positions_Tbl_Organization_Unit_Fld_Org/Unit_Id",
                table: "Tbl_Positions",
                column: "Fld_Org/Unit_Id",
                principalTable: "Tbl_Organization_Unit",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Role_Permissions_Tbl_Modules_Fld_Module_Id",
                table: "Tbl_Role_Permissions",
                column: "Fld_Module_Id",
                principalTable: "Tbl_Modules",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Role_Permissions_Tbl_Permissions_Fld_Permision_Id",
                table: "Tbl_Role_Permissions",
                column: "Fld_Permision_Id",
                principalTable: "Tbl_Permissions",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Role_Permissions_Tbl_Roles_Fld_Role_Id",
                table: "Tbl_Role_Permissions",
                column: "Fld_Role_Id",
                principalTable: "Tbl_Roles",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_User_Positions_Tbl_Positions_Fld_Position_Id",
                table: "Tbl_User_Positions",
                column: "Fld_Position_Id",
                principalTable: "Tbl_Positions",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_User_Positions_Tbl_User_Fld_User_Id",
                table: "Tbl_User_Positions",
                column: "Fld_User_Id",
                principalTable: "Tbl_User",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_User_Roles_Tbl_Roles_Fld_Role_Id",
                table: "Tbl_User_Roles",
                column: "Fld_Role_Id",
                principalTable: "Tbl_Roles",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_User_Roles_Tbl_User_Fld_User_Id",
                table: "Tbl_User_Roles",
                column: "Fld_User_Id",
                principalTable: "Tbl_User",
                principalColumn: "Fld_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Actions_Log_Tbl_Modules_Fld_Modules_Id",
                table: "Tbl_Actions_Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Organization_Unit_Tbl_Organization_Type_Fld_Type_Id",
                table: "Tbl_Organization_Unit");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Positions_Tbl_Organization_Unit_Fld_Org/Unit_Id",
                table: "Tbl_Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Role_Permissions_Tbl_Modules_Fld_Module_Id",
                table: "Tbl_Role_Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Role_Permissions_Tbl_Permissions_Fld_Permision_Id",
                table: "Tbl_Role_Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Role_Permissions_Tbl_Roles_Fld_Role_Id",
                table: "Tbl_Role_Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_User_Positions_Tbl_Positions_Fld_Position_Id",
                table: "Tbl_User_Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_User_Positions_Tbl_User_Fld_User_Id",
                table: "Tbl_User_Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_User_Roles_Tbl_Roles_Fld_Role_Id",
                table: "Tbl_User_Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_User_Roles_Tbl_User_Fld_User_Id",
                table: "Tbl_User_Roles");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_User_Roles_Fld_Role_Id",
                table: "Tbl_User_Roles");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_User_Roles_Fld_User_Id_Fld_Role_Id",
                table: "Tbl_User_Roles");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_User_Positions_Fld_Position_Id",
                table: "Tbl_User_Positions");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_User_Positions_Fld_User_Id",
                table: "Tbl_User_Positions");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Roles_Fld_Code",
                table: "Tbl_Roles");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Role_Permissions_Fld_Module_Id",
                table: "Tbl_Role_Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Role_Permissions_Fld_Permision_Id",
                table: "Tbl_Role_Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Positions_Fld_Org/Unit_Id",
                table: "Tbl_Positions");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Organization_Unit_Fld_Type_Id",
                table: "Tbl_Organization_Unit");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Actions_Log_Fld_Modules_Id",
                table: "Tbl_Actions_Log");

            migrationBuilder.DropColumn(
                name: "Fld_Code",
                table: "Tbl_Roles");

            migrationBuilder.DropColumn(
                name: "Fld_Description",
                table: "Tbl_Roles");

            migrationBuilder.DropColumn(
                name: "Fld_IsActive",
                table: "Tbl_Roles");
        }
    }
}
