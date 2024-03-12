using HPVTesting.Business.ViewModels.Account;
using System;

namespace HPVTesting.Domain.Models
{
    public class User : BaseEntity
    {
        public string AspNetUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public DateTime DOB { get; set; }

        public string Gender { get; set; }

        public string? ProfileImage { get; set; }

        public string? ThumbnailProfileImage { get; set; }

        public int? EmailVerificationCode { get; set; }

        public bool? IsProfileComplete { get; set; }

        public virtual ApplicationUser? AspNetUser { get; set; }
    }
}