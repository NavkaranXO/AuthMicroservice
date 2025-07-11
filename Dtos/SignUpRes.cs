namespace MyMicroservice.Dtos;

public class SignUpRes
{
    public bool? UserConfirmed { get; set; }
    public required string CodeDeliveryDestination { get; set; }
    public required string CodeDeliveryMedium { get; set; }

}