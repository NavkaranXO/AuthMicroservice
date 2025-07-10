namespace MyMicroservice.Dtos;

public class LoginResponse
{
    public required string IdToken { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
}