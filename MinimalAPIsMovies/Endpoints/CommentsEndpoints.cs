using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetComments).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById).WithName("GetCommentById");
            group.MapPost("/", Create);//.AddEndpointFilter<ValidationFilter<CreateCommentDto>>();
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Results<Ok<List<CommentDto>>, NotFound>> GetComments(int movieId, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comments = await commentsRepository.GetComments(movieId);
            var commentsDto = mapper.Map<List<CommentDto>>(comments);
            return TypedResults.Ok(commentsDto);
        }

        static async Task<Results<Ok<CommentDto>, NotFound>> GetById(int movieId, int id, ICommentsRepository commentsRepositories, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = await commentsRepositories.GetById(id);
            if (comment is null)
            {
                return TypedResults.NotFound();
            }

            var commentDto = mapper.Map<CommentDto>(comment);
            return TypedResults.Ok(commentDto);
        }


        static async Task<Results<CreatedAtRoute<CommentDto>, NotFound>> Create(int movieId, CreateCommentDto createCommentDto, ICommentsRepository commentsRepositories, IMoviesRepository moviesRepository, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDto);
            comment.MovieId = movieId;

            var id = await commentsRepositories.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDto = mapper.Map<CommentDto>(comment);
            return TypedResults.CreatedAtRoute(commentDto, "GetCommentById", new { id, movieId });
        }

        static async Task<Results<NoContent, NotFound>> Update(int movieId, int id, CreateCommentDto createCommentDto, IOutputCacheStore outputCacheStore, ICommentsRepository commentsRepositories, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            if (!await commentsRepositories.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDto);
            comment.Id = id;
            comment.MovieId = movieId;
            
            await commentsRepositories.Update(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int movieId, int id, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            if (!await commentsRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await commentsRepository.Delete(id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }
    }
}
