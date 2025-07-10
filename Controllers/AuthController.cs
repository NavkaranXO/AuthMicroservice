using System;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyMicroservice.Dtos;
using MyMicroservice.Models;

namespace MyMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
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
        if (request.Username == null || request.Password == null)
        {
            return BadRequest("Username or Password fields cannot be empty!");
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
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // public async Task<IActionResult> Signup([FromBody] )
    // {
        
    // }
}
