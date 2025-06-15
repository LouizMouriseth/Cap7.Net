using Isopoh.Cryptography.Argon2;

namespace Infrastructure.Extensions;

public static class PasswordExtension
{
    /// <summary>
    /// Generates a hash with bcrypt
    /// </summary>
    /// <param name="password">String to be hashed</param>
    /// <returns>HashPassword</returns>
    public static string HashPassword(this string password)
    {
        return Argon2.Hash(password);
    }

    /// <summary>
    /// Verify if a bcrypt hash is valid
    /// </summary>
    /// <param name="hash">HashPassword to be verified</param>
    /// <param name="password">String to verify</param>
    /// <returns>True if valid</returns>
    public static bool VerifyHash(this string hash, string password)
    {
        return Argon2.Verify(hash, password);
    }
}