using System;

namespace HPVTesting.Business.ViewModels
{
    public partial class UserViewModel
    {
        public Guid Id { get; set; }
        public string AspNetUserId { get; set; }

        public Guid? ActivityLavel { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Name { get; set; }

        public DateTime? DOB { get; set; }

        public string? Gender { get; set; }

        public int? FitnessLevel { get; set; }

        public int? DominateHand { get; set; }

        public bool? IsLimitedTrainingSpace { get; set; }

        public bool? IsAllowNotification { get; set; }

        public string? UserRole { get; set; }

        public bool? IsProfileComplete { get; set; }
        public bool? IsMealPlanStepCompleted { get; set; }

        public string? Status { get; set; }

        public string Email { get; set; }

        public string? EmailConfirmed { get; set; }

        public string UserName { get; set; }

        public string? ProfileImage { get; set; }

        public string? ThumbnailProfileImage { get; set; }
       
        public bool? IsDelete { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}