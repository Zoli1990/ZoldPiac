namespace RekeszAppBackend.Domain;

public enum FelhasznaloSzerepkor { Admin, Vendeg }

public enum FelvasarlasHelyszin { Kocsi, Raktar }

// Minden üzleti rekord egy regisztrált felhasználóhoz tartozik.
public interface IFelhasznaloTulajdona
{
    int? UserId { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string JelszoHash { get; set; } = "";
    public FelhasznaloSzerepkor Role { get; set; } = FelhasznaloSzerepkor.Admin;
    public bool EmailVerified { get; set; }
    public string? VerificationTokenHash { get; set; }
    public DateTime? VerificationTokenExpiresAt { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string AszfVerzio { get; set; } = "1.0";
    public DateTime AszfElfogadvaAt { get; set; }
}

public class Partner : IFelhasznaloTulajdona
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Nev { get; set; } = "";
    public string? Megjegyzes { get; set; }
}

public class Vevo : IFelhasznaloTulajdona
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Nev { get; set; }
    public string? Megjegyzes { get; set; }
}

public class Zoldseg : IFelhasznaloTulajdona
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Nev { get; set; } = "";
    public int? AlapertelmezettRekeszTipusId { get; set; }
    public RekeszTipus? AlapertelmezettRekeszTipus { get; set; }
    public string? KepUrl { get; set; }
}

public class RekeszTipus : IFelhasznaloTulajdona
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Nev { get; set; } = "";
}

public class FelvasarlasTetel : IFelhasznaloTulajdona
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int NapiSorszam { get; set; }
    public DateOnly Datum { get; set; }
    public DateTime Ido { get; set; }
    public bool SajatTermek { get; set; }
    public int? PartnerId { get; set; }
    public Partner? Partner { get; set; }
    public int ZoldsegId { get; set; }
    public Zoldseg Zoldseg { get; set; } = null!;
    public int RekeszTipusId { get; set; }
    public RekeszTipus RekeszTipus { get; set; } = null!;
    public int Mennyiseg { get; set; }
    public bool Fizetve { get; set; }
    public int AdottRekeszDb { get; set; }
    public decimal? Egysegar { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Athozott { get; set; }
    public FelvasarlasHelyszin Helyszin { get; set; } = FelvasarlasHelyszin.Kocsi;
    public int AthelyezveDb { get; set; }
}

public class EladasTetel : IFelhasznaloTulajdona
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int NapiSorszam { get; set; }
    public DateOnly Datum { get; set; }
    public DateTime Ido { get; set; }
    public int? VevoId { get; set; }
    public Vevo? Vevo { get; set; }
    public int ZoldsegId { get; set; }
    public Zoldseg Zoldseg { get; set; } = null!;
    public int RekeszTipusId { get; set; }
    public RekeszTipus RekeszTipus { get; set; } = null!;
    public int Mennyiseg { get; set; }
    public bool Fizetve { get; set; }
    public bool Elvitte { get; set; }
    public int VisszahozottDb { get; set; }
    public int HianyFizettDb { get; set; }
    public decimal? Egysegar { get; set; }
    public string? Megjegyzes { get; set; }
}
