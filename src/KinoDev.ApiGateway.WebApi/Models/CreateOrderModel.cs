namespace KinoDev.ApiGateway.WebApi.Models
{
    public class CreateOrderModel
    {
        public int ShowTimeId { get; set; }

        public ICollection<int> SelectedSeatIds { get; set; } = new List<int>();
    }
}