using MinimalAPIsMovies.Utilities;

namespace MinimalAPIsMovies.DTOs
{
    public class PaginationDto
    {
        public const int pageInitialValue = 1;
        public const int recordsPerPageInitialValue = 10;

        public int Page { get; set; } = 1;
        private int recordsPerPage { get; set; } = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                if (value > recordsPerPageMax)
                {
                    recordsPerPage = recordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }
        public static ValueTask<PaginationDto> BindAsync(HttpContext context)
        {
            /*
            var page = context.Request.Query[nameof(Page)];
            var recordsPerPage = context.Request.Query[nameof(RecordsPerPage)];
            
            var pageInt = string.IsNullOrEmpty(page) ? pageInitialValue : int.Parse(page.ToString());
            var recordsPerPageInt = string.IsNullOrEmpty(recordsPerPage) ? recordsPerPageInitialValue : int.Parse(recordsPerPage.ToString());
            */

            var page = context.ExtractValueOrDefault(nameof(Page), pageInitialValue);
            var recordsPerPage = context.ExtractValueOrDefault(nameof(RecordsPerPage), recordsPerPageInitialValue);

            var response = new PaginationDto
            {
                Page = page,
                RecordsPerPage = recordsPerPage
            };

            return ValueTask.FromResult(response);
        }

    }
}
