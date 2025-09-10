namespace MunicipalityApp.Models
{
    /// <summary>
    /// Represents a user-submitted issue report in the municipality system.
    /// Contains details such as title, category, description, attached media, and status.
    /// </summary>
    public class UserIssue
    {
        /// <summary>
        /// The title or short summary of the reported issue.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The category of the issue (e.g., Electricity, Water, Roads, Waste).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// A detailed description of the issue provided by the user.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The file path of any media (image, video, etc.) attached to the report.
        /// </summary>
        public string FilePath { get; set; }

        public DateTime ReportedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// The current status of the issue (e.g., Pending, Resolved).
        /// Defaults to "Pending" when a new issue is created.
        /// </summary>
        public string Status { get; set; } = "Pending"; // Default status
    }
}
