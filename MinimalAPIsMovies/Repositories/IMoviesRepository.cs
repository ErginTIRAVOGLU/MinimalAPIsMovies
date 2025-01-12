using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IMoviesRepository
    {
        Task Assign(int id, List<int> genresIds);
        Task Assign(int id, List<ActorMovie> actors);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<Movie?> GetById(int id);
        Task<List<Movie>> GetMovies(PaginationDto pagination);
        Task Update(Movie movie);
    }
}