# AuthMicroservice

A RESTful authentication microservice built with **ASP.NET Core (C#)** and **AWS Cognito**, designed to handle user identity and session management for client applications. Built as a hands-on learning project to explore cloud-based authentication, clean backend architecture, and secure token handling in a real-world context.

---

## Features

- **User Registration** — Create new user accounts via AWS Cognito
- **User Login** — Authenticate users and return a full token set
- **Token Issuance** — Returns an ID token, access token, and refresh token on successful login
- **DTO Validation** — All request and response payloads are structured using Data Transfer Objects (DTOs) for clean, validated data flow

---

## Tech Stack

| Technology | Purpose |
|---|---|
| C# / ASP.NET Core | Backend framework |
| AWS Cognito | Cloud identity & user management |
| REST API | Exposes endpoints for frontend/service consumption |
| DTOs | Request/response validation and data shaping |

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive tokens |
| POST | `/api/auth/refresh` | Refresh access token |

> Endpoints may vary slightly — refer to the Controllers folder for the full routing details.

---

## Token Response (Login)

On a successful login, the API returns:

```json
{
  "idToken": "...",
  "accessToken": "...",
  "refreshToken": "..."
}
```

- **ID Token** — Contains user identity claims (name, email, etc.)
- **Access Token** — Used to authorize protected API requests
- **Refresh Token** — Used to obtain a new access token without re-logging in

---

## Project Structure

```
AuthMicroservice/
├── Controllers/       # API route handlers
├── Data/              # Data access layer
├── Dtos/              # Request & response data transfer objects
├── Models/            # Domain models
├── Program.cs         # App entry point and service configuration
└── appsettings.json   # App configuration (Cognito credentials, etc.)
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- An AWS account with a configured Cognito User Pool

### Setup

1. Clone the repository
   ```bash
   git clone https://github.com/NavkaranXO/AuthMicroservice.git
   cd AuthMicroservice
   ```

2. Update `appsettings.json` with your AWS Cognito credentials
   ```json
   {
     "AWS": {
       "Region": "your-region",
       "UserPoolId": "your-user-pool-id",
       "ClientId": "your-client-id"
     }
   }
   ```

3. Run the project
   ```bash
   dotnet run
   ```

---

## What I Learned

- Integrating a cloud identity provider (AWS Cognito) into a .NET backend
- Structuring a microservice with separation of concerns (Controllers, DTOs, Models)
- Handling JWT token flows including issuance, access, and refresh patterns
- Building and testing REST APIs in ASP.NET Core
