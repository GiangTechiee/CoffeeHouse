using AutoMapper;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid OrderId, string Status) : IRequest<Result<OrderDto>>;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly HashSet<string> _allowedStatuses = new(StringComparer.OrdinalIgnoreCase) 
    { 
        "Pending", "Confirmed", "Processing", "Completed", "Cancelled", "Refunded" 
    };

    public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OrderDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_allowedStatuses.Contains(request.Status))
        {
            return Result<OrderDto>.Failure($"Invalid order status. Allowed values: {string.Join(", ", _allowedStatuses)}");
        }

        var order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new CoffeeHouse.Domain.Exceptions.NotFoundException(nameof(Domain.Entities.SalesOrder), request.OrderId);
        }

        try
        {
            switch (request.Status.ToUpperInvariant())
            {
                case "CONFIRMED":
                    order.Confirm();
                    break;
                case "PROCESSING":
                    order.Process();
                    break;
                case "COMPLETED":
                    order.Complete();
                    break;
                case "CANCELLED":
                    order.Cancel();
                    break;
                case "REFUNDED":
                    order.Refund();
                    break;
                default:
                    // If it's "Pending", we don't need to do anything as it's the initial state 
                    // and changing back to Pending is usually not allowed via individual methods.
                    if (request.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                         return Result<OrderDto>.Failure("Cannot change status back to Pending");
                    }
                    break;
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderDto>.Failure(ex.Message);
        }
        
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}
