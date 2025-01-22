using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

namespace MinimalAPIsMovies.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetComments).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById).WithName("GetCommentById");
            group.MapPost("/", Create)
                .AddEndpointFilter<ValidationFilter<CreateCommentDto>>()
                .RequireAuthorization();
            group.MapPut("/{id:int}", Update)
                .AddEndpointFilter<ValidationFilter<CreateCommentDto>>()
                .RequireAuthorization();
            group.MapDelete("/{id:int}", Delete).RequireAuthorization();

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


        static async Task<Results<CreatedAtRoute<CommentDto>, NotFound, BadRequest<string>>> Create(int movieId, CreateCommentDto createCommentDto, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper, IOutputCacheStore outputCacheStore, IUsersService usersService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var user = await usersService.GetUser();
            if (user is null)
            {
                return TypedResults.BadRequest("User not found");
            }




            var comment = mapper.Map<Comment>(createCommentDto);
            comment.MovieId = movieId;
            comment.UserId = user.Id;
            var id = await commentsRepository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDto = mapper.Map<CommentDto>(comment);
            return TypedResults.CreatedAtRoute(commentDto, "GetCommentById", new { id, movieId });
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Update(int movieId, int id, CreateCommentDto createCommentDto, IOutputCacheStore outputCacheStore, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper, IUsersService usersService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var commentFromDB = await commentsRepository.GetById(id);
            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }

            var user = await usersService.GetUser();
            if (user is null)
            {
                return TypedResults.NotFound();
            }

            if (commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }

            commentFromDB.Body = createCommentDto.Body;

            await commentsRepository.Update(commentFromDB);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound,ForbidHttpResult>> Delete(int movieId, int id, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore, IUsersService usersService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var commentFromDB = await commentsRepository.GetById(id);
            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }

            var user = await usersService.GetUser();
            if (user is null)
            {
                return TypedResults.NotFound();
            }

            if (commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }
            await commentsRepository.Delete(id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }
    }
}
