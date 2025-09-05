namespace AnimeMangaApi.DTOs
{
    public class AnimeMangaCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = "Anime";//"Manga", or "Light Novel"
        public int Year { get; set; }
    }
}