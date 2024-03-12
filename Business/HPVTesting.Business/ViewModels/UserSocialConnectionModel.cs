using System;

namespace HPVTesting.Business.ViewModels
{
    public class UserSocialConnectionModel
    {
        public string SocialId { get; set; }
        public Guid UserId { get; set; }
        public int Type { get; set; }
    }
}
