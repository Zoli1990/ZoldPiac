using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RekeszAppBackend.Migrations;

public partial class MultiUserAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM `Users` WHERE `Felhasznalonev` = 'admin';");

        migrationBuilder.DropIndex(name: "IX_Users_Felhasznalonev", table: "Users");
        migrationBuilder.RenameColumn(name: "Felhasznalonev", table: "Users", newName: "Email");
        migrationBuilder.AlterColumn<string>(name: "Email", table: "Users", type: "varchar(320)", maxLength: 320, nullable: false, oldClrType: typeof(string), oldType: "longtext");

        migrationBuilder.AddColumn<bool>(name: "EmailVerified", table: "Users", type: "tinyint(1)", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<string>(name: "VerificationTokenHash", table: "Users", type: "longtext", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "VerificationTokenExpiresAt", table: "Users", type: "datetime(6)", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "RegisteredAt", table: "Users", type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)");
        migrationBuilder.AddColumn<string>(name: "AszfVerzio", table: "Users", type: "longtext", nullable: false, defaultValue: "1.0");
        migrationBuilder.AddColumn<DateTime>(name: "AszfElfogadvaAt", table: "Users", type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)");
        migrationBuilder.CreateIndex(name: "IX_Users_Email", table: "Users", column: "Email", unique: true);

        AddOwnership(migrationBuilder, "Partnerek");
        AddOwnership(migrationBuilder, "Vevek");
        AddOwnership(migrationBuilder, "Zoldsegek");
        AddOwnership(migrationBuilder, "RekeszTipusok");
        AddOwnership(migrationBuilder, "FelvasarlasTetelek");
        AddOwnership(migrationBuilder, "EladasTetelek");

        migrationBuilder.DropIndex(name: "IX_Zoldsegek_Nev", table: "Zoldsegek");
        migrationBuilder.CreateIndex(name: "IX_Zoldsegek_UserId_Nev", table: "Zoldsegek", columns: new[] { "UserId", "Nev" }, unique: true);
        migrationBuilder.DropIndex(name: "IX_RekeszTipusok_Nev", table: "RekeszTipusok");
        migrationBuilder.CreateIndex(name: "IX_RekeszTipusok_UserId_Nev", table: "RekeszTipusok", columns: new[] { "UserId", "Nev" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_Partnerek_UserId_Nev", table: "Partnerek", columns: new[] { "UserId", "Nev" }, unique: true);

        AddUserForeignKey(migrationBuilder, "Partnerek");
        AddUserForeignKey(migrationBuilder, "Vevek");
        AddUserForeignKey(migrationBuilder, "Zoldsegek");
        AddUserForeignKey(migrationBuilder, "RekeszTipusok");
        AddUserForeignKey(migrationBuilder, "FelvasarlasTetelek");
        AddUserForeignKey(migrationBuilder, "EladasTetelek");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        DropUserForeignKey(migrationBuilder, "EladasTetelek");
        DropUserForeignKey(migrationBuilder, "FelvasarlasTetelek");
        DropUserForeignKey(migrationBuilder, "RekeszTipusok");
        DropUserForeignKey(migrationBuilder, "Zoldsegek");
        DropUserForeignKey(migrationBuilder, "Vevek");
        DropUserForeignKey(migrationBuilder, "Partnerek");

        migrationBuilder.DropIndex(name: "IX_Partnerek_UserId_Nev", table: "Partnerek");
        migrationBuilder.DropIndex(name: "IX_RekeszTipusok_UserId_Nev", table: "RekeszTipusok");
        migrationBuilder.CreateIndex(name: "IX_RekeszTipusok_Nev", table: "RekeszTipusok", column: "Nev", unique: true);
        migrationBuilder.DropIndex(name: "IX_Zoldsegek_UserId_Nev", table: "Zoldsegek");
        migrationBuilder.CreateIndex(name: "IX_Zoldsegek_Nev", table: "Zoldsegek", column: "Nev", unique: true);

        DropOwnership(migrationBuilder, "EladasTetelek");
        DropOwnership(migrationBuilder, "FelvasarlasTetelek");
        DropOwnership(migrationBuilder, "RekeszTipusok");
        DropOwnership(migrationBuilder, "Zoldsegek");
        DropOwnership(migrationBuilder, "Vevek");
        DropOwnership(migrationBuilder, "Partnerek");

        migrationBuilder.DropIndex(name: "IX_Users_Email", table: "Users");
        migrationBuilder.DropColumn(name: "AszfElfogadvaAt", table: "Users");
        migrationBuilder.DropColumn(name: "AszfVerzio", table: "Users");
        migrationBuilder.DropColumn(name: "RegisteredAt", table: "Users");
        migrationBuilder.DropColumn(name: "VerificationTokenExpiresAt", table: "Users");
        migrationBuilder.DropColumn(name: "VerificationTokenHash", table: "Users");
        migrationBuilder.DropColumn(name: "EmailVerified", table: "Users");
        migrationBuilder.RenameColumn(name: "Email", table: "Users", newName: "Felhasznalonev");
        migrationBuilder.AlterColumn<string>(name: "Felhasznalonev", table: "Users", type: "longtext", nullable: false, oldClrType: typeof(string), oldType: "varchar(320)", oldMaxLength: 320);
        migrationBuilder.CreateIndex(name: "IX_Users_Felhasznalonev", table: "Users", column: "Felhasznalonev", unique: true).Annotation("MySql:IndexPrefixLength", new[] { 191 });
    }

    private static void AddOwnership(MigrationBuilder m, string table)
    {
        m.AddColumn<int>(name: "UserId", table: table, type: "int", nullable: true);
        m.CreateIndex(name: $"IX_{table}_UserId", table: table, column: "UserId");
    }

    private static void DropOwnership(MigrationBuilder m, string table)
    {
        m.DropIndex(name: $"IX_{table}_UserId", table: table);
        m.DropColumn(name: "UserId", table: table);
    }

    private static void AddUserForeignKey(MigrationBuilder m, string table)
    {
        m.AddForeignKey(name: $"FK_{table}_Users_UserId", table: table, column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }

    private static void DropUserForeignKey(MigrationBuilder m, string table)
    {
        m.DropForeignKey(name: $"FK_{table}_Users_UserId", table: table);
    }
}
