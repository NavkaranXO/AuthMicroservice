using System.ComponentModel.DataAnnotations;
namespace MyMicroservice.Dtos;

public class ConfirmSignUpReq
{   
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string ConfirmationCode { get; set; }
}