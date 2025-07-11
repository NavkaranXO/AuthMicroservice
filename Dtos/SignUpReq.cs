using System.ComponentModel.DataAnnotations;

namespace MyMicroservice.Dtos;

public class SignUpReq
{   
    [Required]
    public required string GivenName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string PhoneNumber { get; set; }
}