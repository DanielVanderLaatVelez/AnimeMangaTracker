namespace AnimeMangaApi.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Score { get; set; } // e.g., 1 to 10
        public string Comment { get; set; } = string.Empty;

        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int AnimeMangaEntryId { get; set; }
        public AnimeMangaEntry AnimeMangaEntry { get; set; } = null!;
    }
}