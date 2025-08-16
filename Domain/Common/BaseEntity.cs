using YojigenPoint.VaultPrime.Helpers;

namespace YojigenPoint.Aegisauth.Domain.Common
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// The unique identifier for the entity. Using 'init' makes it immutable
        /// after the object has been created.
        /// </summary>
        public Guid Id { get; init; } = GuidGenerator.GenerateCombGuid();

        /// <summary>
        /// The UTC timestamp when the entity was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// The UTC timestamp when the entity was last modified.
        /// </summary>
        public DateTime ModifiedAtUtc { get; set; }

        /// <summary>
        /// The UTC timestamp when the entity was soft-deleted. A null value indicates
        /// the entity is active.
        /// </summary>
        public DateTime? DeletedAtUtc { get; set; }
    }
}
