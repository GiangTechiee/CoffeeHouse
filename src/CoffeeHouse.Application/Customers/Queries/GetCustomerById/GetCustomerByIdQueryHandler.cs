using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null) 
            return Result<CustomerDto>.Failure($"Customer with ID {request.Id} not found");

        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }
}
