namespace AnimeMangaApi.DTOs
{
    public class RatingCreateDto
    {
        public int AnimeMangaEntryId { get; set; }
        public int Score { get; set; } // 1 to 10
        public string Comment { get; set; } = string.Empty;
    }
}