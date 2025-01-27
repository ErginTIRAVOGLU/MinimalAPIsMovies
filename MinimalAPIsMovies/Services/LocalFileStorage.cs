
namespace MinimalAPIsMovies.Services
{
    public class LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if(string.IsNullOrEmpty(route))
            {
                return Task.CompletedTask;
            }

            var fileName = System.IO.Path.GetFileName(route);
            var fileDirectory = System.IO.Path.Combine(env.WebRootPath, container, fileName);

            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var extension = System.IO.Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = System.IO.Path.Combine(env.WebRootPath, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = System.IO.Path.Combine(folder, fileName);
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content=ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }

            var schema = httpContextAccessor.HttpContext!.Request.Scheme;
            var host = httpContextAccessor.HttpContext!.Request.Host;

            var url = $"{schema}://{host}";
            var urlFile = System.IO.Path.Combine(url, container, fileName).Replace("\\","/");

            return urlFile;
        }
    }
}
