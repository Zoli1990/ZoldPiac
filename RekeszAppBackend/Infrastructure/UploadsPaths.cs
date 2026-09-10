namespace RekeszAppBackend.Infrastructure;

public static class UploadsPaths
{
    public static string Resolve(IWebHostEnvironment env, IConfiguration config, int? userId = null)
    {
        var configured = config["Uploads:Path"];
        var root = string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(env.ContentRootPath, "private", "images", "users")
            : Path.IsPathRooted(configured)
                ? configured
                : Path.GetFullPath(Path.Combine(env.ContentRootPath, configured));

        Directory.CreateDirectory(root);
        if (userId is null) return root;

        var userDirectory = Path.Combine(root, userId.Value.ToString());
        Directory.CreateDirectory(userDirectory);
        return userDirectory;
    }

    public static void DeleteIfExists(IWebHostEnvironment env, IConfiguration config, int userId, string? kepUrl)
    {
        if (string.IsNullOrWhiteSpace(kepUrl) || !kepUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;
        try
        {
            var fileName = Path.GetFileName(kepUrl["/uploads/".Length..]);
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var filePath = Path.Combine(Resolve(env, config, userId), fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        catch
        {
            // Egy régi kép törlésének hibája nem állíthatja meg az új kép feltöltését.
        }
    }
}
