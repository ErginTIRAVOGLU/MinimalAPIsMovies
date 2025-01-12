using AutoMapper;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDto>();
            CreateMap<CreateGenreDto, Genre>();

            CreateMap<Actor, ActorDto>();
            CreateMap<CreateActorDto, Actor>()
                .ForMember(p => p.Picture, options => options.Ignore());

            CreateMap<Movie, MovieDto>()
                .ForMember(x => x.Genres, entity =>
                    entity.MapFrom(p => p.GenresMovies
                    .Select(gm => new GenreDto { Id = gm.GenreId, Name = gm.Genre.Name })))
                .ForMember(x=>x.Actors,entity=>
                    entity.MapFrom(p=>p.ActorsMovies.Select(am=> new ActorMovieDto
                    {
                        Id=am.ActorId,
                        Name=am.Actor.Name,
                        Character=am.Character
                    })));

            CreateMap<CreateMovieDto, Movie>()
                .ForMember(p => p.Poster, options => options.Ignore());

            CreateMap<Comment, CommentDto>();
            CreateMap<CreateCommentDto, Comment>();

            CreateMap<AssignActorMovieDto, ActorMovie>();
        }
    }
}
