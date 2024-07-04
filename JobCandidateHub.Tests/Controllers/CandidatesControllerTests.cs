using JobCandidateHub.Controllers;
using JobCandidateHub.Data;
using JobCandidateHub.Models;
using JobCandidateHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JobCandidateHub.Tests.Controllers
{
    [Collection("Sequential-Tests")]
    public class CandidatesControllerTests
    {
        private readonly DbContextOptions<CandidateDbContext> _contextOptions;
        private readonly IValidationService _validationService;
        private readonly IMemoryCache _cache;

        public CandidatesControllerTests()
        {
            _contextOptions = new DbContextOptionsBuilder<CandidateDbContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

            _validationService = new ValidationService();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        private CandidatesController GetController()
        {
            var context = new CandidateDbContext(_contextOptions);
            var candidateService = new CandidateService(context, _cache, _validationService);
            return new CandidatesController(candidateService);
        }

        private void ResetDatabase()
        {
            using var context = new CandidateDbContext(_contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Fact]
        public void UpsertCandidate_CreatesNewCandidate_WhenCandidateDoesNotExist()
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                BestTimeToCall = "12:00-16:00"
            };

            var result = controller.UpsertCandidate(candidate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Candidate>(okResult.Value);
            Assert.Equal(candidate.Email, returnValue.Email);
            Assert.Equal(candidate.FirstName, returnValue.FirstName);
            Assert.Equal(candidate.LastName, returnValue.LastName);
        }

        [Fact]
        public async Task UpsertCandidate_UpdatesExistingCandidate_WhenCandidateExists()
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "Existing candidate",
                BestTimeToCall = "12:00-16:00"
            };

            using (var context = new CandidateDbContext(_contextOptions))
            {
                context.Candidates.Add(candidate);
                await context.SaveChangesAsync();
            }

            candidate.FirstName = "Jane";

            var result = controller.UpsertCandidate(candidate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Candidate>(okResult.Value);
            Assert.Equal(candidate.Email, returnValue.Email);
            Assert.Equal("Jane", returnValue.FirstName);
            Assert.Equal(candidate.LastName, returnValue.LastName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid-email")]
        public void UpsertCandidate_ReturnsBadRequest_WhenEmailIsInvalid(string email)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate"
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("Email"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpsertCandidate_ReturnsBadRequest_WhenFirstNameIsInvalid(string firstName)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = firstName,
                LastName = "Doe",
                Comments = "New candidate"
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("FirstName"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpsertCandidate_ReturnsBadRequest_WhenLastNameIsInvalid(string lastName)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = lastName,
                Comments = "New candidate"
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("LastName"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpsertCandidate_ReturnsBadRequest_WhenCommentsIsInvalid(string comments)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = comments
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("Comments"));
        }

        [Theory]
        [InlineData("16:00-12:00")] // Invalid interval: start time is after end time
        [InlineData("25:00-26:00")] // Out of range
        [InlineData("12:00-24:30")] // End time out of range
        [InlineData("24:00-00:00")] // Start time out of range
        [InlineData("invalid-interval")] // Invalid format
        public void UpsertCandidate_ReturnsBadRequest_WhenTimeIntervalIsInvalid(string bestTimeToCall)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                BestTimeToCall = bestTimeToCall
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("BestTimeToCall"));
        }

        [Theory]
        [InlineData("https://www.linkedin.com/in/valid-profile")]
        [InlineData(null)]
        public void UpsertCandidate_AcceptsValidLinkedInProfile(string linkedInProfile)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                LinkedInProfile = linkedInProfile
            };

            var result = controller.UpsertCandidate(candidate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Candidate>(okResult.Value);
            Assert.Equal(candidate.LinkedInProfile, returnValue.LinkedInProfile);
        }

        [Theory]
        [InlineData("invalid-url")]
        public void UpsertCandidate_ReturnsBadRequest_WhenLinkedInProfileIsInvalid(string linkedInProfile)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                LinkedInProfile = linkedInProfile
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("LinkedInProfile"));
        }

        [Theory]
        [InlineData("https://github.com/valid-profile")]
        [InlineData(null)]
        public void UpsertCandidate_AcceptsValidGitHubProfile(string gitHubProfile)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                GitHubProfile = gitHubProfile
            };

            var result = controller.UpsertCandidate(candidate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Candidate>(okResult.Value);
            Assert.Equal(candidate.GitHubProfile, returnValue.GitHubProfile);
        }

        [Theory]
        [InlineData("invalid-url")]
        public void UpsertCandidate_ReturnsBadRequest_WhenGitHubProfileIsInvalid(string gitHubProfile)
        {
            ResetDatabase();
            var controller = GetController();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                GitHubProfile = gitHubProfile
            };

            var result = controller.UpsertCandidate(candidate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("GitHubProfile"));
        }
    }
}