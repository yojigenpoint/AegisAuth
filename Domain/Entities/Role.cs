using YojigenPoint.Aegisauth.Domain.Common;
using System.Collections.Generic;

namespace YojigenPoint.Aegisauth.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        // Navigation properties for the many-to-many relationship with Users
        public virtual ICollection<User> Users { get; set; } = [];
    }
}
