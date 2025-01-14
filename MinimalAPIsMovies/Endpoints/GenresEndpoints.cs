using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).CacheOutput(config => config.Expire(TimeSpan.FromSeconds(15)).Tag("genres-get"))
                .RequireAuthorization();
            group.MapGet("/{id:int}", GetById);//.AddEndpointFilter<TestFilter>();
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateGenreDto>>();
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateGenreDto>>();
            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Ok<List<GenreDto>>> GetGenres(IGenresRepository repository, IMapper mapper)
        {
            var genres = await repository.GetAll();
            var genresDto = mapper.Map<List<GenreDto>>(genres);
            return TypedResults.Ok(genresDto);
        }

        static async Task<Results<Ok<GenreDto>, NotFound>> GetById(int id, IGenresRepository repository, IMapper mapper)
        {
            var genre = await repository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            var genreDto = mapper.Map<GenreDto>(genre);
            return TypedResults.Ok(genreDto);
        }

        static async Task<Created<GenreDto>> Create(CreateGenreDto createGenreDto, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper )
        {
            /*
             * Refactored GenresValidationFilter.cs
            var validationResult = await validator.ValidateAsync(createGenreDto);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            */
            var genre = mapper.Map<Genre>(createGenreDto);
            var id = await repository.Create(genre);

            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDto = mapper.Map<GenreDto>(genre);
            return TypedResults.Created($"/genres/{id}", genreDto);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, CreateGenreDto createGenreDto, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            /*
             * Refactored GenresValidationFilter.cs
            var validationResult = await validator.ValidateAsync(createGenreDto);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            */

            var exist = await repository.Exists(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }

            var genre = mapper.Map<Genre>(createGenreDto);
            genre.Id = id;
            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exist = await repository.Exists(id);
            if (!exist)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
