using JobCandidateHub.Models;
using JobCandidateHub.Services;

namespace JobCandidateHub.Tests.Services
{
    public class ValidationServiceTests
    {
        private readonly ValidationService _validationService;

        public ValidationServiceTests()
        {
            _validationService = new ValidationService();
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenModelIsValid()
        {
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                BestTimeToCall = "09:00-17:00",
                LinkedInProfile = "https://www.linkedin.com/in/johndoe",
                GitHubProfile = "https://github.com/johndoe",
                Comments = "Great candidate"
            };

            var isValid = _validationService.Validate(candidate, out var validationResults);

            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenRequiredFieldsAreMissing()
        {
            var candidate = new Candidate();

            var isValid = _validationService.Validate(candidate, out var validationResults);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("FirstName"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("LastName"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Comments"));
        }

        [Theory]
        [InlineData("25:00-26:00")] // Invalid time format
        [InlineData("08:00-07:00")] // End time before start time
        public void Validate_ReturnsFalse_WhenBestTimeToCallIsInvalid(string bestTimeToCall)
        {
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                BestTimeToCall = bestTimeToCall,
                Comments = "Great candidate"
            };

            var isValid = _validationService.Validate(candidate, out var validationResults);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("BestTimeToCall"));
        }

        [Theory]
        [InlineData("invalid-url", "LinkedInProfile")]
        [InlineData("invalid-url", "GitHubProfile")]
        public void Validate_ReturnsFalse_WhenUrlIsInvalid(string url, string memberName)
        {
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Comments = "Great candidate"
            };

            if (memberName == "LinkedInProfile")
            {
                candidate.LinkedInProfile = url;
            }
            else if (memberName == "GitHubProfile")
            {
                candidate.GitHubProfile = url;
            }

            var isValid = _validationService.Validate(candidate, out var validationResults);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(memberName));
        }
    }
}