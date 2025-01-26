using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.DTOs
{
    public class CreateGenreRequestDto
    {
        public IGenresRepository Repository { get; set; } = null!;
        public IOutputCacheStore OutputCacheStore { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
         
    }
}
