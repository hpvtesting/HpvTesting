using System;

namespace HPVTesting.Domain.Models
{
    public class UserSocialConnection : BaseEntity
    {
        public string SocialId { get; set; }
        public Guid UserId { get; set; }
        public int Type { get; set; }

        public virtual User User { get; set; }
    }
}
