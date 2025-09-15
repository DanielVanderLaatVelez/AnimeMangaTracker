namespace AnimeMangaApi.DTOs
{
    public class UserListDto
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "Plan to Watch";
        public int Progress { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}