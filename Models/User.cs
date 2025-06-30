using System;
using Microsoft.AspNetCore.Identity;

namespace dot_net_qtec.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
