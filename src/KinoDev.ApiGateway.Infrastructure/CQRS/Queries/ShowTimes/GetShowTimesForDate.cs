using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.Shared.DtoModels.ShowTimes;
using MediatR;

namespace KinoDev.ApiGateway.Infrastructure.CQRS.Queries.ShowTimes
{
    public class GetShowTimesForDateQuery : IRequest<ShowTimeForDateDto>
    {
        public DateTime Date { get; set; }
    }

    public class GetShowTimesForDateQueryHandler : IRequestHandler<GetShowTimesForDateQuery, ShowTimeForDateDto>
    {
        private readonly IDomainServiceClient _domainServiceClient;

        public GetShowTimesForDateQueryHandler(IDomainServiceClient domainServiceClient)
        {
            _domainServiceClient = domainServiceClient;
        }

        public async Task<ShowTimeForDateDto> Handle(GetShowTimesForDateQuery request, CancellationToken cancellationToken)
        {
            return await _domainServiceClient.GetShowTimeForDateDtoAsync(request.Date);
        }
    }
}