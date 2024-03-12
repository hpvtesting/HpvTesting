using System;
using System.ComponentModel.DataAnnotations;

namespace HPVTesting.Business.ViewModels
{
    public partial class EmailVerifyModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int Code { get; set; }
    }
}