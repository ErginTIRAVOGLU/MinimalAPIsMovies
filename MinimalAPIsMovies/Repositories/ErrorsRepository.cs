using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class ErrorsRepository(ApplicationDbContext context) : IErrorsRepository
    {
        public async Task Create(Entities.Error error)
        {
            context.Add(error);
            await context.SaveChangesAsync();
        }
    }
}
