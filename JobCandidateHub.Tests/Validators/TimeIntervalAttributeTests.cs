using JobCandidateHub.Validators;

namespace JobCandidateHub.Tests.Validators
{
    public class TimeIntervalAttributeTests
    {
        private readonly TimeIntervalAttribute _attribute = new();

        [Theory]
        [InlineData("08:00-12:00", true)] // Valid interval
        [InlineData("12:00-12:00", false)] // Start and end time are the same
        [InlineData("12:00-08:00", false)] // Start time is after end time
        [InlineData("25:00-26:00", false)] // Out of range
        [InlineData("08:00-24:30", false)] // End time out of range
        [InlineData("24:00-00:00", false)] // Start time out of range
        [InlineData("invalid-interval", false)] // Invalid format
        [InlineData(null, true)] // Null value
        [InlineData("", true)] // Empty string
        [InlineData("  ", true)] // Whitespace string
        public void TimeIntervalValidation_Tests(string timeInterval, bool expected)
        {
            // Act
            var result = _attribute.IsValid(timeInterval);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
