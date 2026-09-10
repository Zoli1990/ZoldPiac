using RekeszAppBackend.Domain;

namespace RekeszAppBackend.Contracts;

public record LoginRequest(string Email, string Jelszo);
public record RegisterRequest(string Email, string Jelszo, bool ElfogadjaAszf);
public record ResendVerificationRequest(string Email);

public record PartnerRequest(string Nev, string? Megjegyzes);
public record VevoRequest(string? Nev, string? Megjegyzes);
public record ZoldsegRequest(string Nev, int? AlapertelmezettRekeszTipusId);
public record RekeszTipusRequest(string Nev);

public record FelvasarlasRequest(
    bool SajatTermek,
    int? PartnerId,
    int ZoldsegId,
    int RekeszTipusId,
    int Mennyiseg,
    bool Fizetve,
    int AdottRekeszDb,
    decimal? Egysegar,
    string? Megjegyzes,
    DateOnly? Datum,
    FelvasarlasHelyszin Helyszin = FelvasarlasHelyszin.Kocsi
);

public record AtvitelRequest(
    int ZoldsegId,
    int RekeszTipusId,
    int Mennyiseg,
    DateOnly CelDatum
);

public record RaktarAthelyezesRequest(
    int FelvasarlasTetelId,
    int Mennyiseg,
    DateOnly CelDatum
);

public record EladasRequest(
    int? VevoId,
    string? UjVevoNev,
    string? UjVevoMegjegyzes,
    int ZoldsegId,
    int RekeszTipusId,
    int Mennyiseg,
    bool Fizetve,
    bool Elvitte,
    int VisszahozottDb,
    int HianyFizettDb,
    decimal? Egysegar,
    string? Megjegyzes,
    DateOnly? Datum
);
