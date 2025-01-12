using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface ICommentsRepository
    {
        Task<int> Create(Comment comment);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<Comment?> GetById(int id);
        Task<List<Comment>> GetComments(int movieId);
        Task Update(Comment comment);
    }
}