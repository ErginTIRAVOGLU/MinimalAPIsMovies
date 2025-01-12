namespace MinimalAPIsMovies.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Body { get; set; } = null!;
        public int MovieId { get; set; }
    }
}
