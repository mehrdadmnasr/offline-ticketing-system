# Offline Ticketing System API

This is a backend Web API for a simplified offline ticketing system, built with ASP.NET Core 8 and Entity Framework Core. The API allows employees to create support tickets and admins to manage them.

## Technical Stack

-   **Backend:** .NET 8, ASP.NET Core Web API
-   **Database:** SQLite (local, file-based)
-   **ORM:** Entity Framework Core
-   **Authentication:** JWT (JSON Web Token)
-   **API Documentation:** Swagger/OpenAPI

## How to Run the Project

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/mehrdadmnasr/offline-ticketing-system.git](https://github.com/mehrdadmnasr/offline-ticketing-system.git)
    cd offline-ticketing-system
    ```
2.  **Open in Visual Studio 2022:**
    Open the `OfflineTicketingSystemAPI.sln` file in Visual Studio 2022.
3.  **Install Dependencies:**
    The required NuGet packages should be restored automatically upon opening the solution. If not, run `dotnet restore` from the command line in the project's directory.
4.  **Create and Seed the Database:**
    Open the **Package Manager Console** in Visual Studio (`Tools` -> `NuGet Package Manager` -> `Package Manager Console`) and run the following command to create the SQLite database and seed the initial data:
    ```bash
    Update-Database
    ```
    This command creates the `OfflineTicketingSystem.db` file and populates it with one admin user and one employee user, along with two sample tickets.
5.  **Run the Application:**
    Press **F5** or **Ctrl+F5** to start the application. The API will be available at a local URL, and your default browser will open the Swagger UI page.

## Initial Data and Credentials

The database is pre-seeded with the following users and credentials:

-   **Admin User:**
    -   **Email:** `admin@test.com`
    -   **Password:** `P@s$w0rd123`
-   **Employee User:**
    -   **Email:** `employee@test.com`
    -   **Password:** `P@s$w0rd123`

## How to Use the API (via Swagger)

1.  **Get an Authentication Token:**
    -   Open the Swagger UI page.
    -   Navigate to the `AuthController` section and use the `POST /api/auth/login` endpoint.
    -   Provide the email and password for either the `admin` or `employee` user.
    -   Execute the request and copy the returned JWT token.
2.  **Authorize in Swagger:**
    -   Click the **`Authorize`** button at the top of the Swagger UI page.
    -   Enter the token in the format `Bearer <YOUR_JWT_TOKEN>` (replace `<YOUR_JWT_TOKEN>` with the token you copied).
    -   Click "Authorize" to secure your subsequent requests.
3.  **Test Endpoints:**
    -   You can now test the various endpoints in the `TicketsController` section.
    -   **Admin-only endpoints** (e.g., `GET /api/tickets`) will work only when you are authorized with the `admin` token.
    -   **Employee-only endpoints** (e.g., `POST /api/tickets`) will work only when authorized with the `employee` token.

## Assumptions and Design Decisions

-   **Database:** SQLite was chosen for its simplicity and local-first nature, as required by the challenge.
-   **Password Hashing:** A simple SHA256 hashing algorithm is used for demonstration. In a production environment, a more robust, slow hashing algorithm with salting (like BCrypt or Argon2) should be used.
-   **JWT Configuration:** For this local challenge, JWT issuer and audience validation were disabled for simplicity. These should be enabled in a production setting.
-   **Error Handling:** Basic error handling (`NotFound`, `BadRequest`, etc.) has been implemented for core logic, focusing on the main requirements rather than exhaustive edge-case coverage.
