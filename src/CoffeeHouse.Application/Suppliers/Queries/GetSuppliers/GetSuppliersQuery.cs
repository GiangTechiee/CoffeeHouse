using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Suppliers.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<SupplierDto>>;
