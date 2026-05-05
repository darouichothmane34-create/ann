using System;

namespace AnnuaireEntreprise.Repositories;

/// <summary>
/// Exception métier pour encapsuler les erreurs d'accès aux données.
/// </summary>
public class RepositoryException : Exception
{
    public RepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
