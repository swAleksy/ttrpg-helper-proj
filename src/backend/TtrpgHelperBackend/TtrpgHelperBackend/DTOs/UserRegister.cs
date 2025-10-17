﻿using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs;
public class UserRegister
{
    [Required]
    [MinLength(3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    
    public bool IsAdminRequest { get; set; } = false; 
}