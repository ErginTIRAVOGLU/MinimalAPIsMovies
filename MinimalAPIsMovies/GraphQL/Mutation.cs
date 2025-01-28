using AutoMapper;
using HotChocolate.Authorization;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.GraphQL
{
    public class Mutation
    {
        [Serial]
        [Authorize(Policy = "isadmin")]
        public async Task<GenreDto> InsertGenre([Service] IGenresRepository repository, [Service] IMapper mapper, CreateGenreDto createGenreDto)
        {
            var genre = mapper.Map<Genre>(createGenreDto);
            await repository.Create(genre);
            var genreDto = mapper.Map<GenreDto>(genre);
            return genreDto;
        }
    }
}
