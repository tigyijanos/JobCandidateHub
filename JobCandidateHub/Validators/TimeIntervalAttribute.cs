using System.ComponentModel.DataAnnotations;

namespace JobCandidateHub.Validators
{
    /// <summary>
    /// Validates for time interval (11:00-14:00)
    /// </summary>
    public class TimeIntervalAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            var timeInterval = value.ToString();
            var times = timeInterval!.Split('-');

            if (times.Length != 2 ||
                !TimeSpan.TryParse(times[0], out TimeSpan startTime) ||
                !TimeSpan.TryParse(times[1], out TimeSpan endTime))
            {
                return false;
            }

            if (startTime >= endTime)
            {
                return false;
            }

            if (startTime.TotalHours < 0 || startTime.TotalHours > 24 || endTime.TotalHours < 0 || endTime.TotalHours > 24)
            {
                return false;
            }

            return true;
        }
    }
}
