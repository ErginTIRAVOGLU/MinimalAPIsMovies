﻿using AutoMapper;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDto>();
            CreateMap<CreateGenreDto, Genre>();
        }
    }
}
