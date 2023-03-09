using System.ComponentModel.DataAnnotations;

namespace Challenges.API.Tests.Integration.Models;
public class AuthenticationParameters
{
    [Required]
    public string GrantType { get; set; }
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string ClientSecret { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
