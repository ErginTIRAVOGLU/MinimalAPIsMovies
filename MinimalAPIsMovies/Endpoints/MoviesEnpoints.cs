using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

namespace MinimalAPIsMovies.Endpoints
{
    public static class MoviesEnpoints
    {
        private readonly static string container = "movies";

        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetMovies).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("movies-get"));
            group.MapGet("/{id}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDto>>();
            group.MapPut("/{ id:int}", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDto>>();
            group.MapDelete("/{id:int}", Delete);
            group.MapPost("/{id:int}/assignGenres", AssignGenres);
            group.MapPost("/{id:int}/assignActors", AssignActors);
            return group;
        }

        static async Task<Ok<List<MovieDto>>> GetMovies(IMoviesRepository repository, IMapper mapper, int page = 1, int recordPerPage = 10)
        {
            var pagination = new PaginationDto
            { Page = page, RecordsPerPage = recordPerPage };
            var movies = await repository.GetMovies(pagination);
            var moviesDto = mapper.Map<List<MovieDto>>(movies);
            return TypedResults.Ok(moviesDto);
        }

        static async Task<Results<Ok<MovieDto>, NotFound>> GetById(int id, IMoviesRepository repository, IMapper mapper)
        {
            var movie = await repository.GetById(id);

            if (movie is null)
            {
                return TypedResults.NotFound();
            }

            var movieDto = mapper.Map<MovieDto>(movie);
            return TypedResults.Ok(movieDto);
        }

        static async Task<Created<MovieDto>> Create([FromForm] CreateMovieDto createMovieDto, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper, IMoviesRepository repository)
        {
            var movie = mapper.Map<Movie>(createMovieDto);

            if (createMovieDto.Poster is not null)
            {
                var url = await fileStorage.Store(container, createMovieDto.Poster);
                movie.Poster = url;
            }

            var id = await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);

            var movieDto = mapper.Map<MovieDto>(movie);
            return TypedResults.Created($"/movies/{id}", movieDto);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateMovieDto createMovieDto, IMoviesRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var movieDB = await repository.GetById(id);

            if (movieDB == null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Movie>(movieDB);
            movieForUpdate.Id = id;
            movieForUpdate.Poster = movieDB.Poster;

            if (createMovieDto.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster, container, createMovieDto.Poster);
                movieForUpdate.Poster = url;
            }

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IMoviesRepository repository, IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
        {
            var movieDB = await repository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(movieDB.Poster, container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AssignGenres(int id, List<int> genresIds, IMoviesRepository moviesRepository, IGenresRepository genresRepository)
        {
            if (!await moviesRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var existingGenres = new List<int>();

            if (genresIds.Count != 0)
            {
                existingGenres = await genresRepository.Exists(genresIds);
            }

            if (genresIds.Count != existingGenres.Count)
            {
                var nonExistingGenres = genresIds.Except(existingGenres);

                var nonExistingGenresCSV = string.Join(",", nonExistingGenres);
                return TypedResults.BadRequest($"The genres of id {nonExistingGenresCSV} does not exists");
            }

            await moviesRepository.Assign(id, genresIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AssignActors(int id, List<AssignActorMovieDto> actorsDto, IMoviesRepository moviesRepository, IActorsRepository actorsRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var existingActors = new List<int>();
            var actorsIds = actorsDto.Select(a => a.ActorId).ToList();

            if (actorsDto.Count != 0)
            {
                existingActors = await actorsRepository.Exists(actorsIds);
            }

            if (existingActors.Count != actorsDto.Count)
            {
                var nonExistingActors = actorsIds.Except(existingActors);
                var nonExistingActorsCSV = string.Join(",", nonExistingActors);
                return TypedResults.BadRequest($"The actors of id {nonExistingActorsCSV} do not exists");
            }

            var actors = mapper.Map<List<ActorMovie>>(actorsDto);
            await moviesRepository.Assign(id, actors);
            return TypedResults.NoContent();
        }
    }
}
