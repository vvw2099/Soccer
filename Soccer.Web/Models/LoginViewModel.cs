﻿using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        [Required]
        [MinLength(6)]
        public string password { get; set; }
        public bool RememberMe { get; set; }
    }
}