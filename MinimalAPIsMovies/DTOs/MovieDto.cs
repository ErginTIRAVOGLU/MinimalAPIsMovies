namespace MinimalAPIsMovies.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public List<GenreDto> Genres { get; set; } = new List<GenreDto> { };
        public List<ActorMovieDto> Actors { get; set; } = new List<ActorMovieDto>();
    }
}
