using System.Security.Cryptography;
using System.Text;

namespace BRCSISTEM.Domain.Security
{
    public static class PasswordHasher
    {
        public const string GlobalSalt = "BRCSISTEM2025";

        public static string HashSha256(string password, string userSalt)
        {
            var combinedSalt = string.IsNullOrWhiteSpace(userSalt) ? GlobalSalt : GlobalSalt + userSalt;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes((password ?? string.Empty) + combinedSalt);
                var hash = sha.ComputeHash(bytes);
                return BytesToHex(hash);
            }
        }

        public static string HashLegacyMd5(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password ?? string.Empty);
                var hash = md5.ComputeHash(bytes);
                return BytesToHex(hash);
            }
        }

        private static string BytesToHex(byte[] data)
        {
            var builder = new StringBuilder(data.Length * 2);
            foreach (var value in data)
            {
                builder.Append(value.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
