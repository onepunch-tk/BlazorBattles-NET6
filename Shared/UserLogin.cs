using System.ComponentModel.DataAnnotations;

namespace BlazorBattles.Shared;

public class UserLogin
{
    [Required,EmailAddress]
    public string Email { get; set; }
    [Required(ErrorMessage = "test04")]
    public string Password { get; set; }
}