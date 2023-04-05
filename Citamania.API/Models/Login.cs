﻿using System.ComponentModel.DataAnnotations;

namespace Citamania.API.Models
{ 
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }    

    }
}
