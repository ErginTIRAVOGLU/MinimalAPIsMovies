using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.GraphQL
{
    public class Query
    {
        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Genre> GetGenres([Service] ApplicationDbContext context) => context.Genres;
    }
}
