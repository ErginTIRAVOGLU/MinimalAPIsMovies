
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly ApplicationDbContext _context;

        public GenresRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Create(Genre genre)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return genre.Id;
        }

        public async Task Delete(int id)
        {
            await _context.Genres.Where(g => g.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await _context.Genres.AnyAsync(g => g.Id != id && g.Name == name);
        }

        public async Task<List<int>> Exists(List<int> ids)
        {
            return await _context.Genres
                .Where(g => ids.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();
        }

        public async Task<List<Genre>> GetAll()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre?> GetById(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Update(Genre genre)
        {
            _context.Update(genre);
            await _context.SaveChangesAsync();
        }
    }
}
