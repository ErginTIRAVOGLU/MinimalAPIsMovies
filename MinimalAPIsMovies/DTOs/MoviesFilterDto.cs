using MinimalAPIsMovies.Utilities;

namespace MinimalAPIsMovies.DTOs
{
    public class MoviesFilterDto
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDto PaginationDto
        {
            get
            {
                return new PaginationDto
                {
                    Page = Page,
                    RecordsPerPage = RecordsPerPage
                };
            }
        }
        public string? Title { get; set; }
        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool FutureReleasees { get; set; }
        public string? OrderByField { get; set; }
        public bool OrderByAscending { get; set; } = true;

        public static ValueTask<MoviesFilterDto> BindAsync(HttpContext context)
        {
            var page = context.ExtractValueOrDefault(nameof(Page), PaginationDto.pageInitialValue);
            var recordsPerPage = context.ExtractValueOrDefault(nameof(RecordsPerPage), PaginationDto.recordsPerPageInitialValue);

            var title = context.ExtractValueOrDefault(nameof(Title), string.Empty);
            var genreId = context.ExtractValueOrDefault(nameof(GenreId), 0);
            var inTheaters = context.ExtractValueOrDefault(nameof(InTheaters), false);
            var futureReleases = context.ExtractValueOrDefault(nameof(FutureReleasees), false);
            var orderByField = context.ExtractValueOrDefault(nameof(OrderByField), string.Empty);
            var orderByAscending = context.ExtractValueOrDefault(nameof(OrderByAscending), true);

            var response = new MoviesFilterDto
            {
                Page = page,
                RecordsPerPage = recordsPerPage,
                Title = title,
                GenreId = genreId,
                InTheaters = inTheaters,
                FutureReleasees = futureReleases,
                OrderByField = orderByField,
                OrderByAscending = orderByAscending
            };

            return ValueTask.FromResult(response);
        }
    }
}
