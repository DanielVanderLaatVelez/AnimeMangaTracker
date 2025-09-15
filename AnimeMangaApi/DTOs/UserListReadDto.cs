namespace AnimeMangaApi.DTOs
{
    public class UserListReadDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}