using System.Security.Cryptography;

namespace SV22T1080045.Shop.BusinessLayers
{
    public interface IPasswordHasherService
    {
        string Hash(string plainTextPassword);
        bool Verify(string plainTextPassword, string storedHash);
    }

    public class PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public string Hash(string plainTextPassword)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                plainTextPassword,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string plainTextPassword, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
                return false;

            // Backward compatibility: existing plain text passwords.
            if (!storedHash.Contains('.'))
                return storedHash == plainTextPassword;

            var parts = storedHash.Split('.');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out var iterations))
                return false;

            var salt = Convert.FromBase64String(parts[1]);
            var expectedHash = Convert.FromBase64String(parts[2]);

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                plainTextPassword,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
    }
}
