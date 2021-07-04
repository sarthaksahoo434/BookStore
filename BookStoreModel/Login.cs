﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BookStoreModel
{
    public class Login
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
