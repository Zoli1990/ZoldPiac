using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RekeszAppBackend.Migrations;

public partial class PasswordReset : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(name: "VerificationEmailSentAt", table: "Users", type: "datetime(6)", nullable: true);
        migrationBuilder.AddColumn<string>(name: "ResetTokenHash", table: "Users", type: "longtext", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "ResetTokenExpiresAt", table: "Users", type: "datetime(6)", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "ResetEmailSentAt", table: "Users", type: "datetime(6)", nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "ResetEmailSentAt", table: "Users");
        migrationBuilder.DropColumn(name: "ResetTokenExpiresAt", table: "Users");
        migrationBuilder.DropColumn(name: "ResetTokenHash", table: "Users");
        migrationBuilder.DropColumn(name: "VerificationEmailSentAt", table: "Users");
    }
}
