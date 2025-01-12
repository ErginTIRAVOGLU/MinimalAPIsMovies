namespace MinimalAPIsMovies.DTOs
{
    public class CreateActorDto
    {
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
