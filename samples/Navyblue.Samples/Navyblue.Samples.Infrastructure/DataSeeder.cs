using Navyblue.Samples.Application.Authentication;
using Navyblue.Samples.Application.Users;
using Navyblue.Samples.Domain.Authentication;
using Navyblue.Samples.Domain.Users;
using Navyblue.Foundation.Primitives;
using Navyblue.Foundation.Security;

namespace Navyblue.Samples.Infrastructure;

/// <summary>
///     Seeds an initial admin user and credential on startup if the store is empty.
/// </summary>
public static class DataSeeder
{
    private const string AdminEmail = "admin@navyblue.local";
    private const string AdminPassword = "Admin@123";

    /// <summary>
    ///     Ensures a single admin user exists. Safe to call on every startup.
    /// </summary>
    public static async Task SeedAsync(IUserRepository userRepository, IAuthRepository authRepository, IIdGenerator<long> idGenerator)
    {
        User? existing = await userRepository.FindByEmailAsync(AdminEmail);
        if (existing is not null) return;

        long userId = idGenerator.NextId();
        User admin = new(userId, "Administrator", AdminEmail);
        await userRepository.AddAsync(admin);

        string hash = PasswordHasher.Hash(AdminPassword);
        string salt = ExtractSalt(hash);
        Auth auth = new(idGenerator.NextId(), userId, AdminEmail, hash, salt);
        await authRepository.AddAsync(auth);
    }

    private static string ExtractSalt(string stored)
    {
        string[] parts = stored.Split('$');
        return parts.Length == 4 ? parts[2] : string.Empty;
    }
}
