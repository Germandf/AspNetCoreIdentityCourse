﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityCourse.App.Authorization;

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}
