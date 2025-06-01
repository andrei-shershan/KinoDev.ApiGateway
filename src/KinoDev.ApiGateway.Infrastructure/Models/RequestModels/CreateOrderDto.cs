namespace KinoDev.ApiGateway.Infrastructure.Models.RequestModels;

public class CreateOrderDto
{
    public int ShowTimeId { get; set; }

    public ICollection<int> SelectedSeatIds { get; set; } = new List<int>();
}