using Microsoft.AspNetCore.Identity;

namespace ChessServer.Api.Database;

public class ApplicationUser : IdentityUser
{
    public double Rating { get; set; } = 1500.0;
}