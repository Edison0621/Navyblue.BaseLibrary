// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : Auth.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="Auth.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Domain.Authentication;

/// <summary>
///     Authentication credential for a <see cref="Users.User" />.
///     Stores the PBKDF2 password hash (salt embedded) and a separate salt field
///     for auditing/migration. Verification is delegated to PasswordHasher.
/// </summary>
public sealed class Auth : Entity<long>
{
    /// <summary>EF Core materialization constructor.</summary>
    private Auth() : base(default)
    {
        this.Login = null!;
        this.PasswordHash = null!;
        this.Salt = null!;
    }

    public Auth(long id, long userId, string login, string passwordHash, string salt) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        ArgumentNullException.ThrowIfNull(passwordHash);
        ArgumentNullException.ThrowIfNull(salt);

        this.UserId = userId;
        this.Login = login;
        this.PasswordHash = passwordHash;
        this.Salt = salt;
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>User id this credential belongs to.</summary>
    public long UserId { get; private set; }

    /// <summary>Login identifier (matches the user email).</summary>
    public string Login { get; private set; }

    /// <summary>PBKDF2 password hash string (format: pbkdf2-sha256$iter$salt$hash).</summary>
    public string PasswordHash { get; private set; }

    /// <summary>Hex/base64 salt used when the hash was created.</summary>
    public string Salt { get; private set; }

    /// <summary>When the credential was created.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Last time the password was changed.</summary>
    public DateTimeOffset? PasswordChangedAt { get; private set; }

    /// <summary>Replace the password hash and salt.</summary>
    public void ChangePassword(string passwordHash, string salt)
    {
        ArgumentNullException.ThrowIfNull(passwordHash);
        ArgumentNullException.ThrowIfNull(salt);

        this.PasswordHash = passwordHash;
        this.Salt = salt;
        this.PasswordChangedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Update the login identifier (typically kept in sync with user email).</summary>
    public void ChangeLogin(string login)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);

        this.Login = login.Trim();
    }
}