using YojigenPoint.VaultPrime.Helpers;

namespace YojigenPoint.Aegisauth.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; init; } = GuidGenerator.GenerateCombGuid();
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }

        // Navigation property back to the User who owns this token
        public virtual User User { get; set; } = null!;
    }
}
