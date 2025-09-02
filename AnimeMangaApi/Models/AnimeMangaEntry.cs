namespace AnimeMangaApi.Models
{
    public class AnimeMangaEntry
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Anime", "Manga", or "Light Novel"
        public int Year { get; set; }

        public List<Rating> Ratings { get; set; } = new List<Rating>();
    }
}