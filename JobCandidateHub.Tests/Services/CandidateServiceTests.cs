using JobCandidateHub.Data;
using JobCandidateHub.Models;
using JobCandidateHub.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JobCandidateHub.Tests.Services
{
    [Collection("Sequential-Tests")]
    public class CandidateServiceTests
    {
        private readonly DbContextOptions<CandidateDbContext> _contextOptions;
        private readonly IValidationService _validationService;
        private readonly IMemoryCache _cache;

        public CandidateServiceTests()
        {
            _contextOptions = new DbContextOptionsBuilder<CandidateDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _validationService = new ValidationService();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        private CandidateService GetCandidateService()
        {
            var context = new CandidateDbContext(_contextOptions);
            return new CandidateService(context, _cache, _validationService);
        }

        private void ResetDatabase()
        {
            using var context = new CandidateDbContext(_contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task UpsertCandidate_CreatesNewCandidate_WhenCandidateDoesNotExist()
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                BestTimeToCall = "12:00-16:00"
            };

            var result = service.UpsertCandidate(candidate);

            using var context = new CandidateDbContext(_contextOptions);
            var dbCandidate = await context.Candidates.FirstOrDefaultAsync(c => c.Email == candidate.Email);
            Assert.NotNull(dbCandidate);
            Assert.Equal(candidate.Email, dbCandidate.Email);
            Assert.Equal(candidate.FirstName, dbCandidate.FirstName);
            Assert.Equal(candidate.LastName, dbCandidate.LastName);
        }

            [Fact]
            public async Task UpsertCandidate_UpdatesExistingCandidate_WhenCandidateExists()
            {
                ResetDatabase();
                var service = GetCandidateService();

                var candidate = new Candidate
                {
                    Email = "test@example.com75",
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

                var result = service.UpsertCandidate(candidate);

                using (var context = new CandidateDbContext(_contextOptions))
                {
                    var dbCandidate = await context.Candidates.FirstOrDefaultAsync(c => c.Email == candidate.Email);
                    Assert.NotNull(dbCandidate);
                    Assert.Equal(candidate.Email, dbCandidate.Email);
                    Assert.Equal("Jane", dbCandidate.FirstName);
                    Assert.Equal(candidate.LastName, dbCandidate.LastName);
                }
            }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid-email")]
        public void UpsertCandidate_ReturnsBadRequest_WhenEmailIsInvalid(string email)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate"
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpsertCandidate_ReturnsBadRequest_WhenFirstNameIsInvalid(string firstName)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = firstName,
                LastName = "Doe",
                Comments = "New candidate"
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("FirstName"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpsertCandidate_ReturnsBadRequest_WhenLastNameIsInvalid(string lastName)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = lastName,
                Comments = "New candidate"
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("LastName"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpsertCandidate_ReturnsBadRequest_WhenCommentsIsInvalid(string comments)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = comments
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("Comments"));
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
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                BestTimeToCall = bestTimeToCall
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("BestTimeToCall"));
        }

        [Theory]
        [InlineData("test@example.com", "https://www.linkedin.com/in/valid-profile")]
        [InlineData("test@example.com1", null)]
        public async Task UpsertCandidate_AcceptsValidLinkedInProfile(string email,string linkedInProfile)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                LinkedInProfile = linkedInProfile
            };

            var result = service.UpsertCandidate(candidate);

            using var context = new CandidateDbContext(_contextOptions);
            var dbCandidate = await context.Candidates.FirstOrDefaultAsync(c => c.Email == candidate.Email);
            Assert.NotNull(dbCandidate);
            Assert.Equal(candidate.LinkedInProfile, dbCandidate.LinkedInProfile);
        }

        [Theory]
        [InlineData("invalid-url")]
        public void UpsertCandidate_ReturnsBadRequest_WhenLinkedInProfileIsInvalid(string linkedInProfile)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                LinkedInProfile = linkedInProfile
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("LinkedInProfile"));
        }

        [Theory]
        [InlineData("https://github.com/valid-profile")]
        [InlineData(null)]
        public async Task UpsertCandidate_AcceptsValidGitHubProfile(string gitHubProfile)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                GitHubProfile = gitHubProfile
            };

            var result = service.UpsertCandidate(candidate);

            using var context = new CandidateDbContext(_contextOptions);
            var dbCandidate = await context.Candidates.FirstOrDefaultAsync(c => c.Email == candidate.Email);
            Assert.NotNull(dbCandidate);
            Assert.Equal(candidate.GitHubProfile, dbCandidate.GitHubProfile);
        }

        [Theory]
        [InlineData("invalid-url")]
        public void UpsertCandidate_ReturnsBadRequest_WhenGitHubProfileIsInvalid(string gitHubProfile)
        {
            ResetDatabase();
            var service = GetCandidateService();

            var candidate = new Candidate
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comments = "New candidate",
                GitHubProfile = gitHubProfile
            };

            var exception = Assert.Throws<ValidationException>(() => service.UpsertCandidate(candidate));

            Assert.Contains(exception.ValidationResults, v => v.MemberNames.Contains("GitHubProfile"));
        }
    }
}