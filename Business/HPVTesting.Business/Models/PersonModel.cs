using System;
using System.ComponentModel.DataAnnotations;

namespace HPVTesting.Business.Models
{
    public class PersonModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public bool IsRememberMe { get; set; }
    }
}