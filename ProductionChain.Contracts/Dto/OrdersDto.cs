namespace ProductionChain.Contracts.Dto;

public class OrdersDto
{
    public int Id { get; set; }

    public int Index { get; set; }

    public required string Customer { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public int Count { get; set; }

    public required string Status { get; set; }

    public DateTime? CreateAt { get; set; }
}
