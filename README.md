# User Management System

This project is a User Management System built with ASP.NET Core, featuring functionalities for importing users from an Excel file, as well as Google and Facebook login integration.

## Features

- **User Import from Excel**: Import users from an Excel file (CSV or XLSX format) and automatically assign roles.
- **Google Login**: Authenticate users via Google and create new user accounts if they do not exist.
- **Facebook Login**: Authenticate users via Facebook and create new user accounts if they do not exist.

## Requirements

- .NET 6.0 SDK or later
- ASP.NET Core Identity
- Entity Framework Core
- EPPlus (for Excel handling)
- Google.Apis.Auth (for Google token validation)
- Newtonsoft.Json (for JSON handling)
- HttpClient (for Facebook token validation)

## Setup

1. **Clone the repository**:
    ```sh
    git clone https://github.com/yourusername/user-management-system.git
    cd user-management-system
    ```

2. **Configure Google and Facebook Credentials**:
    - Create a project in Google Developer Console and obtain `clientId`.
    - Create a Facebook app in the Facebook Developer Console and obtain `appSecret`.

3. **Set up appsettings.json**:
    ```json
    {
        "GoogleAuthSettings": {
            "ClientId": "your-google-client-id"
        },
        "FacebookAuthSettings": {
            "AppSecret": "your-facebook-app-secret"
        },
        "ConnectionStrings": {
            "DefaultConnection": "your-database-connection-string"
        }
    }
    ```

4. **Install Dependencies**:
    ```sh
    dotnet restore
    ```

5. **Run the Application**:
    ```sh
    dotnet run
    ```

## Usage

### Import Users from Excel

1. Ensure the Excel file contains the following headers: `Name, Email, First Name, Last Name, Roles`.
2. Send a POST request to `/api/users/import-from-excel` with the Excel file.

### Google Login

1. Send a POST request to `/api/auth/google-login` with the Google ID token.
2. The backend will validate the token and create a user if it doesn't exist.

### Facebook Login

1. Send a POST request to `/api/auth/facebook-login` with the Facebook access token.
2. The backend will validate the token and create a user if it doesn't exist.
