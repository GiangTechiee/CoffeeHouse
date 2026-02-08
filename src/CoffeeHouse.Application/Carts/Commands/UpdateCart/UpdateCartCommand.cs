using MediatR;
using System.Collections.Generic;

namespace CoffeeHouse.Application.Carts.Commands.UpdateCart;

public class UpdateCartCommand : IRequest<UpdateCartResult>
{
    public int CustomerId { get; set; }
    public List<UpdateCartItemRequest> Updates { get; set; } = new();
}

public class UpdateCartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
}
