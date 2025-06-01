using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.Movies;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies
{
    public class GetMoviesQuery : IRequest<IEnumerable<MovieDto>>
    {

    }

    public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, IEnumerable<MovieDto>>
    {
        private readonly IDomainServiceClient _domainServiceClient;
       
        public GetMoviesQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<IEnumerable<MovieDto>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetMoviesAsync();
        }
    }
}
