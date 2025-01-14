using AutoMapper;
using FluentValidation;
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
    public static class ActorsEndpoints
    {
        private readonly static string container = "actors";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetActors).CacheOutput(config => config.Expire(TimeSpan.FromSeconds(15)).Tag("actors-get"));
            group.MapGet("getByName/{name}", GetByName);
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateActorDto>>();
            group.MapPut("/{id:int}", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateActorDto>>();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Created<ActorDto>> Create([FromForm] CreateActorDto createActorDto, IActorsRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage   )
        { 
            var actor = mapper.Map<Actor>(createActorDto);

            if (createActorDto.Picture is not null)
            {
                var url = await fileStorage.Store(container, createActorDto.Picture);
                actor.Picture = url;
            }

            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            var actorDto = mapper.Map<ActorDto>(actor);
            return TypedResults.Created($"/actors/{id}", actorDto);
        }

        static async Task<Ok<List<ActorDto>>> GetActors(IActorsRepository repository, IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDto
            {
                Page = page,
                RecordsPerPage = recordsPerPage
            };

            var actors = await repository.GetAll(pagination);
            var actorsDto = mapper.Map<List<ActorDto>>(actors);
            return TypedResults.Ok(actorsDto);
        }

        static async Task<Results<Ok<ActorDto>, NotFound>> GetById(int id, IActorsRepository repository, IMapper mapper)
        {
            var actor = await repository.GetById(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorDto = mapper.Map<ActorDto>(actor);
            return TypedResults.Ok(actorDto);
        }

        static async Task<Ok<List<ActorDto>>> GetByName(string name, IActorsRepository repository, IMapper mapper)
        {
            var actors = await repository.GetByName(name);
            var actorsDto = mapper.Map<List<ActorDto>>(actors);
            return TypedResults.Ok(actorsDto);
        }


        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateActorDto createActorDto, IActorsRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var actorDb = await repository.GetById(id);
            if (actorDb is null)
            {
                return TypedResults.NotFound();
            }

            var actoForUpdate = mapper.Map<Actor>(createActorDto);
            actoForUpdate.Id = id;
            actoForUpdate.Picture = actorDb.Picture;

            if (createActorDto.Picture is not null)
            {

                var url = await fileStorage.Edit(actoForUpdate.Picture, container, createActorDto.Picture);
                actoForUpdate.Picture = url;
            }

            await repository.Update(actoForUpdate);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IActorsRepository repository, IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
        {
            var actorDb = await repository.GetById(id);
            if (actorDb is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(actorDb.Picture, container);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

    }
}
