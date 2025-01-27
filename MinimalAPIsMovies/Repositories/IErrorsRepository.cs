using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IErrorsRepository
    {
        Task Create(Entities.Error error);
    }
}