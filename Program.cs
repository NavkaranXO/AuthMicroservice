using MyMicroservice.Data;
using Microsoft.EntityFrameworkCore;
using MyMicroservice.Models;
using Amazon.CognitoIdentityProvider;
using Amazon;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Amazon.Runtime;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

//Accessing aws configuration from .env file
var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
var region = Environment.GetEnvironmentVariable("AWS_REGION");
var userPoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID");
var clientId = Environment.GetEnvironmentVariable("COGNITO_CLIENT_ID");

//Storing aws credentials and region endpoint 
var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
var regionEndpoint = RegionEndpoint.GetBySystemName(region);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CashCardDbContext>(options =>
    options.UseInMemoryDatabase("CashCardDB"));

//Registering Cognito CLient
builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(
    new AmazonCognitoIdentityProviderClient(awsCredentials, regionEndpoint)
);

builder.Services.Configure<CognitoOptions>(options =>
{
    options.UserPoolId = userPoolId;
    options.ClientId = clientId;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_HZOYi4pmZ",
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CashCardDbContext>();

    if (!context.CashCards.Any())
    {
        context.CashCards.AddRange(
            new CashCard { Id = 1, Owner = "Alice", Balance = 100.0m },
            new CashCard { Id = 2, Owner = "Bob", Balance = 50.0m },
            new CashCard { Id = 3, Owner = "Charlie", Balance = 200.0m }
        );
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();