using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string enteredPassword, string storedHash);
}

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(salt); }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 256 / 8));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }
    public bool VerifyPassword(string enteredPassword, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        string savedHash = parts[1];

        string calculatedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: enteredPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(savedHash),
            Convert.FromBase64String(calculatedHash));
    }
}