

namespace TaletApi.utility
{
    public static class BycryptService
    {
        public static string HashPassword(string password)
        {
            // Generate a new salt for each password hash
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12); // 12 is the recommended salt work factor
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public static bool VerifyPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
