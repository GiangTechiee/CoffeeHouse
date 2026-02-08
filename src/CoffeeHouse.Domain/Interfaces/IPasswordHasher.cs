namespace CoffeeHouse.Domain.Interfaces
{
    /// <summary>
    /// Interface for password hashing operations.
    /// Provides secure password hashing and verification using BCrypt.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a plain text password using BCrypt with a work factor of 12.
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>The BCrypt hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a BCrypt hash.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hash">The BCrypt hash to verify against</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        bool VerifyPassword(string password, string hash);
    }
}

