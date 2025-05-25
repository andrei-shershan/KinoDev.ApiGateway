namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.Movies
{
    using System.Threading;
    using System.Threading.Tasks;
    using KinoDev.ApiGateway.Infrastructure.HttpClients;
    using KinoDev.Shared.DtoModels.Movies;
    using MediatR;

    public class GetMovieByIdQuery : IRequest<MovieDto>
    {
        public int Id { get; set; }
    }

    public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, MovieDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetMovieByIdQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public Task<MovieDto> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
        {
            return _domainServiceClient.GetMovieByIdAsync(request.Id);
        }
    }
}
