using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.Shared.DtoModels.ShowTimes;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.ShowTimes
{
    public class GetShowTimesByDateQuery : IRequest<IEnumerable<ShowTimeDetailsDto>>
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class GetShowTimesByDateQueryHandler : IRequestHandler<GetShowTimesByDateQuery, IEnumerable<ShowTimeDetailsDto>>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetShowTimesByDateQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<IEnumerable<ShowTimeDetailsDto>> Handle(GetShowTimesByDateQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
            {
                throw new ArgumentException("Start date cannot be later than end date.");
            }

            return await _domainServiceClient.GetShowTimeDetailsAsync(request.StartDate, request.EndDate);
        }
    }
}