using Microsoft.EntityFrameworkCore;

namespace RekeszAppBackend.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        // A database schema mindig a verziózott EF Core migrationökből épül fel.
        // Közös admin/admin felhasználót többé nem hozunk létre.
        await db.Database.MigrateAsync();
    }
}
