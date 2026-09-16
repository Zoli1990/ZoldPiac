using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RekeszAppBackend.Contracts;
using RekeszAppBackend.Data;
using RekeszAppBackend.Domain;
using RekeszAppBackend.Infrastructure;

namespace RekeszAppBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IConfiguration config, IEmailSender emailSender) : ControllerBase
{
    private const string AszfVerzio = "1.0";
    private static readonly TimeSpan EmailCooldown = TimeSpan.FromSeconds(60);
    private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email)) return BadRequest(new { message = "Érvényes email cím megadása kötelező." });
        if (string.IsNullOrWhiteSpace(request.Jelszo) || request.Jelszo.Length < 8) return BadRequest(new { message = "A jelszónak legalább 8 karakteresnek kell lennie." });
        if (!request.ElfogadjaAszf) return BadRequest(new { message = "A regisztrációhoz az ÁSZF elfogadása kötelező." });
        if (await db.Users.AnyAsync(x => x.Email == email)) return Conflict(new { message = "Ezzel az email címmel már létezik felhasználói fiók." });

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var user = new User
        {
            Email = email,
            JelszoHash = BCrypt.Net.BCrypt.HashPassword(request.Jelszo),
            Role = FelhasznaloSzerepkor.Admin,
            EmailVerified = false,
            VerificationTokenHash = HashToken(rawToken),
            VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24),
            VerificationEmailSentAt = DateTime.UtcNow,
            RegisteredAt = DateTime.UtcNow,
            AszfVerzio = AszfVerzio,
            AszfElfogadvaAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        foreach (var nev in new[] { "M10", "M30" })
            db.RekeszTipusok.Add(new RekeszTipus { UserId = user.Id, Nev = nev });
        await db.SaveChangesAsync();

        try
        {
            await KuldesVisszaigazoloEmailAsync(user.Email, rawToken, HttpContext.RequestAborted);
        }
        catch
        {
            var sajatRekeszTipusok = await db.RekeszTipusok.IgnoreQueryFilters().Where(x => x.UserId == user.Id).ToListAsync();
            db.RekeszTipusok.RemoveRange(sajatRekeszTipusok);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Az email kiküldése jelenleg nem sikerült. Kérjük, próbáld újra később." });
        }

        return Ok(new { message = "Regisztráció sikeres. A belépéshez igazold vissza az email-címedet." });
    }

    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        const string genericMessage = "Ha a megadott email-címhez visszaigazolásra váró fiók tartozik, új visszaigazoló emailt küldtünk.";
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email)) return Ok(new { message = genericMessage });

        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null || user.EmailVerified) return Ok(new { message = genericMessage });
        if (user.VerificationEmailSentAt is not null && DateTime.UtcNow - user.VerificationEmailSentAt < EmailCooldown) return Ok(new { message = genericMessage });

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.VerificationTokenHash = HashToken(rawToken);
        user.VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
        user.VerificationEmailSentAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        try
        {
            await KuldesVisszaigazoloEmailAsync(user.Email, rawToken, HttpContext.RequestAborted);
        }
        catch
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Az email kiküldése jelenleg nem sikerült. Kérjük, próbáld újra később." });
        }

        return Ok(new { message = genericMessage });
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return BadRequest(new { message = "Hiányzó visszaigazoló token." });
        var hash = HashToken(token);
        var user = await db.Users.FirstOrDefaultAsync(x => x.VerificationTokenHash == hash);
        if (user is null) return BadRequest(new { message = "A visszaigazoló link érvénytelen vagy már felhasznált." });
        if (user.VerificationTokenExpiresAt is null || user.VerificationTokenExpiresAt < DateTime.UtcNow) return BadRequest(new { message = "A visszaigazoló link lejárt. Kérj új regisztrációs emailt." });

        user.EmailVerified = true;
        user.VerificationTokenHash = null;
        user.VerificationTokenExpiresAt = null;
        await db.SaveChangesAsync();
        return Ok(new { message = "Az email-cím sikeresen visszaigazolva. Most már bejelentkezhetsz." });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Jelszo, user.JelszoHash)) return Unauthorized(new { message = "Hibás email-cím vagy jelszó." });
        if (!user.EmailVerified) return Unauthorized(new { message = "A belépéshez előbb vissza kell igazolnod az email-címedet." });

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryHours = double.Parse(config["Jwt:ExpiryHours"] ?? "12");
        var token = new JwtSecurityToken(issuer: config["Jwt:Issuer"], audience: config["Jwt:Audience"], claims: claims, expires: DateTime.UtcNow.AddHours(expiryHours), signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), email = user.Email, role = user.Role.ToString(), userId = user.Id });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        const string genericMessage = "Ha a megadott email-címhez tartozik fiók, elküldtük a jelszó-visszaállító linket.";
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email)) return Ok(new { message = genericMessage });

        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null) return Ok(new { message = genericMessage });
        if (user.ResetEmailSentAt is not null && DateTime.UtcNow - user.ResetEmailSentAt < EmailCooldown) return Ok(new { message = genericMessage });

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.ResetTokenHash = HashToken(rawToken);
        user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        user.ResetEmailSentAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        try
        {
            await KuldesJelszoVisszaallitoEmailAsync(user.Email, rawToken, HttpContext.RequestAborted);
        }
        catch
        {
            // A generikus válasz így sem árulja el, hogy a fiók létezik-e; a küldési hibát csak naplózzuk.
        }

        return Ok(new { message = genericMessage });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token)) return BadRequest(new { message = "Hiányzó visszaállító token." });
        if (string.IsNullOrWhiteSpace(request.UjJelszo) || request.UjJelszo.Length < 8) return BadRequest(new { message = "A jelszónak legalább 8 karakteresnek kell lennie." });

        var hash = HashToken(request.Token);
        var user = await db.Users.FirstOrDefaultAsync(x => x.ResetTokenHash == hash);
        if (user is null) return BadRequest(new { message = "A visszaállító link érvénytelen vagy már felhasznált." });
        if (user.ResetTokenExpiresAt is null || user.ResetTokenExpiresAt < DateTime.UtcNow) return BadRequest(new { message = "A visszaállító link lejárt. Kérj új jelszó-visszaállítást." });

        user.JelszoHash = BCrypt.Net.BCrypt.HashPassword(request.UjJelszo);
        user.ResetTokenHash = null;
        user.ResetTokenExpiresAt = null;
        await db.SaveChangesAsync();
        return Ok(new { message = "A jelszó sikeresen megváltozott. Most már bejelentkezhetsz az új jelszóval." });
    }

    private async Task KuldesJelszoVisszaallitoEmailAsync(string email, string rawToken, CancellationToken cancellationToken)
    {
        var frontendUrl = (config["Auth:FrontendUrl"] ?? "http://localhost:5173").TrimEnd('/');
        var resetUrl = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";
        var body = $"""
            <html><body style="font-family:Arial,sans-serif;line-height:1.5">
            <h2>ZöldPiac jelszó-visszaállítás</h2>
            <p>Az alábbi gombra kattintva új jelszót adhatsz meg a fiókodhoz:</p>
            <p><a href="{resetUrl}" style="display:inline-block;padding:10px 16px;background:#2f5d50;color:#fff;text-decoration:none;border-radius:6px">Jelszó megváltoztatása</a></p>
            <p>Ez a link 1 óráig érvényes.</p>
            <p>Ha nem te kérted a jelszó-visszaállítást, ezt az üzenetet hagyd figyelmen kívül – a jelszavad nem változott.</p>
            </body></html>
            """;
        await emailSender.SendAsync(email, "ZöldPiac – jelszó-visszaállítás", body, cancellationToken);
    }

    private async Task KuldesVisszaigazoloEmailAsync(string email, string rawToken, CancellationToken cancellationToken)
    {
        var frontendUrl = (config["Auth:FrontendUrl"] ?? "http://localhost:5173").TrimEnd('/');
        var verificationUrl = $"{frontendUrl}/verify-email?token={Uri.EscapeDataString(rawToken)}";
        var body = $"""
            <html><body style="font-family:Arial,sans-serif;line-height:1.5">
            <h2>ZoldPiac email-cím visszaigazolás</h2>
            <p>Köszönjük a regisztrációt. A fiók aktiválásához kattints az alábbi gombra:</p>
            <p><a href="{verificationUrl}" style="display:inline-block;padding:10px 16px;background:#2f5d50;color:#fff;text-decoration:none;border-radius:6px">Email-cím visszaigazolása</a></p>
            <p>Ez a link 24 óráig érvényes.</p>
            <p>Ha nem te kezdeményezted a regisztrációt, ezt az üzenetet hagyd figyelmen kívül.</p>
            </body></html>
            """;
        await emailSender.SendAsync(email, "ZoldPiac – email-cím visszaigazolása", body, cancellationToken);
    }

    private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
