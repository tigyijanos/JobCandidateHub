# JobCandidateHub

JobCandidateHub is a .NET-based web application that provides an API for storing and managing job candidate information. The application supports adding and updating candidate details, with potential for future extension to store a large volume of candidate data.

This project is part of an interview test task.

## Features

- Add or update candidate information via REST API.
- Validate candidate data, including email format, time interval format, and URLs.
- Cache candidate data for improved performance.
- Unit tests to ensure functionality and correctness.

## Technologies Used

- .NET 8
- Entity Framework Core
- SQLite
- In-Memory Cache
- xUnit
- Swagger for API documentation

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/tigyijanos/JobCandidateHub.git
    cd JobCandidateHub
    ```

2. Build the project:
    ```sh
    dotnet build
    ```

3. Run the database migrations:
    ```sh
    dotnet ef database update
    ```

### Running the Application

1. To run the application, use the following command:
    ```sh
    dotnet run --project JobCandidateHub
    ```
2. The application will start, and Swagger UI will be available at http://localhost:5276/swagger.

### API Endpoints
- Upsert Candidate
- URL: POST /api/candidates
- Description: Add or update candidate information.

### Request Body
1. json:  
    ```json
        {
        "firstName": "John",
        "lastName": "Doe",
        "phoneNumber": "123-456-7890",
        "email": "john.doe@example.com",
        "bestTimeToCall": "09:00-17:00",
        "linkedInProfile": "https://www.linkedin.com/in/johndoe",
        "gitHubProfile": "https://github.com/johndoe",
        "comments": "Great candidate"
        }
    ```
### Response:
- 200 OK: Candidate information added or updated successfully.
- 400 Bad Request: Validation error.

### Validation Rules
- First Name: Required
- Last Name: Required
- Email: Required, valid email format
- Best Time to Call: Valid time interval (HH:mm-HH
), start time should be before end time, within 24 hours.
- LinkedIn Profile: Optional, valid URL
- GitHub Profile: Optional, valid URL
- Comments: Required

### Testing
To run the unit tests, use the following command:
```sh
dotnet test
```
### Project Structure
```
JobCandidateHub
├── Controllers
│   └── CandidatesController.cs
├── Data
│   └── CandidateDbContext.cs
├── Models
│   └── Candidate.cs
├── Services
│   ├── CandidateService.cs
│   ├── ICandidateService.cs
│   ├── IValidationService.cs
│   ├── ValidationService.cs
├── Validators
│   └── TimeIntervalAttribute.cs
├── Properties
│   └── launchSettings.json
├── appsettings.json
├── Program.cs
├── JobCandidateHub.http
README.md

JobCandidateHub.Tests
├── Controllers
│   └── CandidatesControllerTests.cs
├── Services
│   ├── CandidateServiceTests.cs
│   ├── ValidationServiceTests.cs
├── Validators
│   └── TimeIntervalAttributeTests.cs
└── SequentialTestsCollection.cs

```

### Future Improvements
- Implement authentication and authorization.
- Add more detailed logging.
- Implement pagination for fetching candidate lists.
- Extend API to support bulk operations.
- Optimize database queries for large data sets.

### Contributing
Contributions are welcome! Please create a pull request or submit an issue for any feature requests or bugs.

### License
This project is licensed under the MIT License - see the LICENSE file for details
