using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MyMicroservice.Dtos;
using MyMicroservice.Models;

namespace MyMicroservice.Controllers;

[ApiController]
[Route("authapi/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly CognitoOptions _cognitoOptions;

    public AuthController(IAmazonCognitoIdentityProvider cognitoClient, IOptions<CognitoOptions> cognitoOptions)
    {
        _cognitoClient = cognitoClient;
        _cognitoOptions = cognitoOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authRequest = new AdminInitiateAuthRequest
        {
            UserPoolId = _cognitoOptions.UserPoolId,
            ClientId = _cognitoOptions.ClientId,
            AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH,
            AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", request.Username },
                { "PASSWORD", request.Password },
            }
        };
        try
        {
            var response = await _cognitoClient.AdminInitiateAuthAsync(authRequest);

            if (response.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
            {
                return BadRequest("User must change their password.");
            }

            if (response.AuthenticationResult == null)
            {
                return BadRequest("Authentication failed: No tokens returned.");
            }

            return Ok(new LoginResponse
            {
                IdToken = response.AuthenticationResult.IdToken,
                AccessToken = response.AuthenticationResult.AccessToken,
                RefreshToken = response.AuthenticationResult.RefreshToken,
                ExpiresIn = response.AuthenticationResult.ExpiresIn
            });

        }
        catch (NotAuthorizedException)
        {
            return Unauthorized("Invalid username or password");
        }
        catch (UserNotConfirmedException)
        {
            return BadRequest("User account is not confirmed");
        }
        catch (Exception)
        {
            // Unexpected error: return 500 without leaking details
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignUpReq request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var signUp = new SignUpRequest
        {
            ClientId = _cognitoOptions.ClientId,
            Username = request.Username,
            Password = request.Password,
            UserAttributes = new()
            {
                new() { Name = "email", Value = request.Email },
                new() { Name = "phone_number",Value = request.PhoneNumber },
                new() { Name = "given_name",  Value = request.GivenName },
                new() { Name = "family_name", Value = request.LastName }
            }
        };
        try
        {
            var response = await _cognitoClient.SignUpAsync(signUp);

            return Ok(new SignUpRes
            {
                UserConfirmed = response.UserConfirmed,
                CodeDeliveryDestination = response.CodeDeliveryDetails.Destination,
                CodeDeliveryMedium = response.CodeDeliveryDetails.DeliveryMedium
            });
        }
        catch (UsernameExistsException)
        {
            return Conflict("Username already exists!");
        }
        catch (InvalidPasswordException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidParameterException paramEx)
        {
            // e.g. attribute missing or malformed
            return BadRequest($"Invalid parameter: {paramEx.Message}");
        }
        catch (Exception)
        {
            // Unexpected error: return 500 without leaking details
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> SignUpConfirmation([FromBody] ConfirmSignUpReq request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var confirmSignup = new ConfirmSignUpRequest
        {
            ClientId = _cognitoOptions.ClientId,
            Username = request.Username,
            ConfirmationCode = request.ConfirmationCode
        };

        try
        {
            var response = await _cognitoClient.ConfirmSignUpAsync(confirmSignup);
            return Ok(new { Message = "User Confirmed Successfully!" });
        }
        catch (CodeMismatchException)
        {
            return BadRequest("Invalid Code!");
        }
        catch (ExpiredCodeException)
        {
            return BadRequest("The Confirmation code has Expired. Please request a new one.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected Error Occured!");    
        }
    }

}
