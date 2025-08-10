using System.ComponentModel.DataAnnotations;

namespace ProductionChain.Contracts.Dto.Requests;

public class ProductionOrdersRequest
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "OrderId должен быть больше 0")]
    public int OrderId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ProductId должен быть больше 0")]
    public int ProductId { get; set; }
}
