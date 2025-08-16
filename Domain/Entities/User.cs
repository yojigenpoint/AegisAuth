using YojigenPoint.Aegisauth.Domain.Common;
using YojigenPoint.VaultPrime.Extensions;
using System.Collections.Generic;

namespace YojigenPoint.Aegisauth.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation properties for related data
        public virtual ICollection<Role> Roles { get; private set; } = [];
        public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

        // Private constructor to enforce creation via the factory method
        private User() { }

        /// <summary>
        /// Factory method to create a new User instance.
        /// This is the only way to create a User, ensuring that all validation is met.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="passwordHash">The pre-hashed user password.</param>
        /// <returns>A new, valid user object.</returns>
        public static User Create(string email, string passwordHash)
        {
            // Domain logic and validation
            if (email.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            if (passwordHash.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            }

            return new User
            {
                Email = email,
                PasswordHash = passwordHash
            };
        }
    }
}
