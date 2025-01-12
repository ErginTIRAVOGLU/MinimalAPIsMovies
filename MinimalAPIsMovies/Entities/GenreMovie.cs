namespace MinimalAPIsMovies.Entities
{
    public class GenreMovie
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
        public Movie Moview { get; set; } = null!;
    }
}
