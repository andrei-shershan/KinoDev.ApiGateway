using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Commands.Movies
{
    public class CreateMovieCommand : IRequest<MovieDto?>
    {
        public IFormCollection Form { get; set; } = null!;
    }

    public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, MovieDto?>
    {
        private readonly IDomainServiceClient _domainServiceClient;
        private readonly IStorageServiceClient _storageServiceClient;
        private ILogger<CreateMovieCommandHandler> _logger;

        public CreateMovieCommandHandler(
            IDomainServiceClient domainServiceClient,
            IStorageServiceClient storageServiceClient,
            ILogger<CreateMovieCommandHandler> logger
            )
        {
            _domainServiceClient = domainServiceClient;
            _storageServiceClient = storageServiceClient;
            _logger = logger;
        }

        public async Task<MovieDto?> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Form == null || request.Form.Count == 0)
                {
                    _logger.LogWarning("No files found in the request.");
                    return null;
                }

                // TODO: Implement validation for the form data
                var movie = new MovieDto()
                {
                    Name = request.Form[nameof(MovieDto.Name)],
                    Description = request.Form[nameof(MovieDto.Description)],
                    Duration = int.Parse(request.Form[nameof(MovieDto.Duration)]),
                    ReleaseDate = DateOnly.Parse(request.Form[nameof(MovieDto.ReleaseDate)])
                };

                using var fileStream = request.Form.Files[0].OpenReadStream();
                byte[] fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes, 0, (int)fileStream.Length);

                // TODO: Implement a more robust file name generation strategy
                var fileName = Guid.NewGuid().ToString() + request.Form.Files[0].FileName;

                var fileRelativePath = await _storageServiceClient.UploadFileAsync(fileName, fileBytes);
                if (fileRelativePath == null)
                {
                    _logger.LogError("File upload failed.");
                    return null;
                }

                movie.Url = fileRelativePath;

                return await _domainServiceClient.CreateMovieAsync(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling CreateMovieCommand");
            }

            return null;
        }
    }
}