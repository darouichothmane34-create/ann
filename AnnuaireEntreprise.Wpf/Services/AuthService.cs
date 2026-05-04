using System;

namespace AnnuaireEntreprise.Services;

/// <summary>
/// Service simple d'authentification administrateur.
/// </summary>
public class AuthService
{
    private const string DefaultAdminPassword = "Admin123!";

    public bool VerifyAdminPassword(string? password)
    {
        return string.Equals(password, DefaultAdminPassword, StringComparison.Ordinal);
    }
}
