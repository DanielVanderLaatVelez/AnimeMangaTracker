namespace AnimeMangaApi.Models
{
    public class UserList
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int AnimeMangaEntryId { get; set; }
        public AnimeMangaEntry AnimeMangaEntry { get; set; } = null!;

        public string Status { get; set; } = "Plan to Watch"; // e.g., "Watching", "Completed", "On-Hold", "Dropped", "Plan to Watch"
        public int Progress { get; set; } // Number of episodes/chapters watched/read
        public string Notes { get; set; } = string.Empty; // User's personal notes
    }
}