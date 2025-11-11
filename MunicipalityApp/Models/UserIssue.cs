namespace MunicipalityApp.Models
{
    // User-submitted issue report in the municipality system.


    public class UserIssue
    {
        // Unique identifier assigned to the service request upon creation.
        public Guid Id { get; set; } = Guid.NewGuid();

    // The title or short summary of the reported issue.
    public string Title { get; set; } = string.Empty;

    // The category of the issue
    public string Category { get; set; } = string.Empty;


    // A detailed description of the issue provided by the user.
    public string Description { get; set; } = string.Empty;


    // The file path of any media (image, video) attached to the report.
    public string FilePath { get; set; } = string.Empty;

        public DateTime ReportedDate { get; set; } = DateTime.Now;

        // The current status of the issue
        public string Status { get; set; } = "Pending"; // Default status

        // Progress percentage of the request
        public int Progress { get; set; } = 0;
    }
}
