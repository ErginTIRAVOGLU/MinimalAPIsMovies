namespace MinimalAPIsMovies.DTOs
{
    public class AuthenticationResponseDto
    {
        public string Token { get; set; }=null!;
        public DateTime Expiration { get; set; }
    }
}
