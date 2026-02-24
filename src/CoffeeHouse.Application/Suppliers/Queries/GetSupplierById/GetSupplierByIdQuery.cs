using CoffeeHouse.Application.Suppliers.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(int SupplierId) : IRequest<SupplierDto>;
