using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.ShowingMovies;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies
{
    public class GetShowingMoviesQuery : IRequest<IEnumerable<ShowingMovie>>
    {
        public DateTime Date { get; set; }
    }
    
    public class GetShowingMoviesQueryHandler : IRequestHandler<GetShowingMoviesQuery, IEnumerable<ShowingMovie>>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetShowingMoviesQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<IEnumerable<ShowingMovie>> Handle(GetShowingMoviesQuery request, CancellationToken cancellationToken)
        {
            // Reset time to 00:00:00
            request.Date = request.Date.Date;

            return await _domainServiceClient.GetShowingMoviesAsync(request.Date);
        }
    }
}
